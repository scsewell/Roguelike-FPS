using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Framework;
using LevelGen;

public class LevelGenerator : ComponentSingleton<LevelGenerator>
{
    [SerializeField]
    private bool m_useRandomSeed = false;
    [SerializeField]
    private int m_randomSeed = 0;
    [SerializeField]
    private GenerationParameters m_params;

    private Transform m_levelRoot;
    private Transform m_otherParent;
    private Transform m_navParent;
    private NavMeshManager m_navMeshManager;

    private Level m_level;
    private float m_noiseOffsetX;
    private float m_noiseOffsetY;
    private float m_noiseScale;

    protected override void Awake()
    {
        base.Awake();
    }

    public void GenerateLevel()
    {
        InitGeneration();
        CreateRooms();
        CreateCorridors();

        Skinner.Skin(m_level, m_navParent, m_otherParent, m_params);
        m_navMeshManager.BuildNavMesh();

        PlacePlayer();
        PlaceEnemies();

        GC.Collect();
    }

    private void InitGeneration()
    {
        // Clear any level assets in the scene
        if (m_levelRoot != null)
        {
            Destroy(m_levelRoot.gameObject);
        }

        m_levelRoot = new GameObject("Level").transform;
        m_navParent = new GameObject("Navmeshed").transform;
        m_navParent.SetParent(m_levelRoot);
        m_otherParent = new GameObject("Other").transform;
        m_otherParent.SetParent(m_levelRoot);

        m_navMeshManager = m_navParent.gameObject.AddComponent<NavMeshManager>();

        // Set the random seed if required
        UnityEngine.Random.InitState(m_useRandomSeed ? m_randomSeed : UnityEngine.Random.Range(0, int.MaxValue));

        // Generate a new noise profile
        m_noiseOffsetX = UnityEngine.Random.value * 100000;
        m_noiseOffsetY = UnityEngine.Random.value * 100000;
        m_noiseScale = Mathf.Pow(1 - (m_params.NoiseScale / 100), 2);
    }

    /// <summary>
    /// Places the rooms that will be in the level.
    /// </summary>
    private void CreateRooms()
    {
        List<Room> rooms = new List<Room>();

        int roomCount = UnityEngine.Random.Range(m_params.MinRoomCount, m_params.MaxRoomCount);

        float levelHalfWidth = m_params.MaxLevelWidth / 2;
        float levelHalfLength = m_params.MaxLevelLength / 2;

        int failedTries = 0;
        while (rooms.Count < roomCount)
        {
            if (failedTries > 10000)
            {
                Debug.LogWarning("Failed to generate target number of rooms! (" + rooms.Count + " of " + roomCount + ")");
                break;
            }
            
            // get a center position for the room
            int x = Mathf.RoundToInt(UnityEngine.Random.Range(-levelHalfWidth, levelHalfWidth));
            int z = Mathf.RoundToInt(UnityEngine.Random.Range(-levelHalfLength, levelHalfLength));

            if (SampleNoise(x, z) < m_params.NoiseThreshold)
            {
                failedTries++;
                continue;
            }

            // if creating an elliptical level shape, reject any position outside the ellipse
            if (m_params.levelIsElliptical)
            {
                float xFac = (x / levelHalfWidth);
                float zFac = (z / levelHalfLength);

                float xEllipse = xFac * xFac;
                float zEllipse = zFac * zFac;

                if (xEllipse + zEllipse > 1)
                {
                    failedTries++;
                    continue;
                }
            }

            // generate a new room size
            int xSize = Mathf.RoundToInt(ValueFromDistribution(m_params.RoomSizeDistribution, m_params.MinRoomSize, m_params.MaxRoomSize));
            int zSize = Mathf.RoundToInt(ValueFromDistribution(m_params.RoomSizeDistribution, m_params.MinRoomSize, m_params.MaxRoomSize));

            Room newRoom = new Room(x, z, xSize, zSize, 0, 1);

            if (rooms.Any(r => r.Bounds.Intersects(newRoom.Bounds)))
            {
                failedTries++;
                continue;
            }

            rooms.Add(newRoom);
        }

        m_level = new Level(rooms);
    }

    /// <summary>
    /// Creates corridors that connect all rooms.
    /// </summary>
    private void CreateCorridors()
    {
        // Sort rooms by floor area
        List<Room> rooms = m_level.Rooms.OrderByDescending(r => r.Bounds.size.x * r.Bounds.size.z).ToList();

        List<Room> connected = new List<Room>();
        List<Room> unconnected = new List<Room>(rooms);
        
        while (unconnected.Count > 0)
        {
            Room startRoom = unconnected.First();

            List<Room> otherRooms = ((connected.Count > 0) ? connected : unconnected).OrderBy(
                        r => Vector3.Distance(r.Bounds.center, startRoom.Bounds.center)
                        ).ToList();

            Room endRoom = otherRooms[(int)(Mathf.Pow(UnityEngine.Random.value, 1 + m_params.CloseRoomBias) * otherRooms.Count)];
            
            if (CarveCorridor(startRoom, endRoom, connected, unconnected))
            {
                unconnected.Remove(startRoom);
                connected.Add(startRoom);

                if (!connected.Contains(endRoom))
                {
                    connected.Add(endRoom);
                }
            }
        }
    }

    private bool CarveCorridor(Room startRoom, Room endRoom, List<Room> connected, List<Room> unconnected)
    {
        Tile start = m_level.GetTile(startRoom.GetRandomPointInsideRoom());
        Tile goal = m_level.GetTile(endRoom.GetRandomPointInsideRoom());
        Vector3 lastMoveDir = Vector3.zero;

        List<Tile> path = AStar(start, goal, (current, fromMap) => GetCorridorCost(current, fromMap, start, goal));
        if (path != null)
        {
            foreach (Tile node in path)
            {
                if (node.Type == TileType.Wall)
                {
                    node.MakeCorridor();
                }
                else if (node.Type == TileType.Room && !connected.Contains(node.Room))
                {
                    unconnected.Remove(node.Room);
                    connected.Add(node.Room);
                }
            }
            return true;
        }
        return false;
    }

    public float GetCorridorCost(Tile current, Dictionary<Tile, Tile> fromMap, Tile start, Tile goal)
    {
        float directionWeight = 0;
        if (current != start)
        {
            Tile previous = fromMap[current];
            
            if (fromMap.ContainsKey(previous))
            {
                Tile morePrevious = fromMap[previous];
                
                Vector3 moveDirection = current.Position - previous.Position;
                Vector3 previousMoveDirection = previous.Position - morePrevious.Position;
                directionWeight = -Vector3.Dot(moveDirection, previousMoveDirection * m_params.ChangeDirectionCost);
            }
        }
        
        Vector3 startDisp = current.Position - start.Position;
        Vector3 goalDisp = goal.Position - current.Position;

        float g = Mathf.Abs(startDisp.x) + Mathf.Abs(startDisp.y) + Mathf.Abs(startDisp.z);
        float h = Mathf.Abs(goalDisp.x) + Mathf.Abs(goalDisp.y) + Mathf.Abs(goalDisp.z);

        float noiseCost = SampleNoise(current.x, current.z) * m_params.NoiseCost;

        float carveCost = 0;
        switch (current.Type)
        {
            case TileType.Wall: carveCost = m_params.WallCarveCost; break;
            case TileType.Room: carveCost = m_params.RoomCarveCost; break;
        }

        return g + h + carveCost + directionWeight + noiseCost;
    }
    
    private List<Tile> AStar(Tile start, Tile goal, Func<Tile, Dictionary<Tile, Tile>, float> costFunc)
    {
        List<Tile> open = new List<Tile>();
        HashSet<Tile> closed = new HashSet<Tile>();
        Dictionary<Tile, Tile> movedFromTile = new Dictionary<Tile, Tile>();
        Dictionary<Tile, float> tileCost = new Dictionary<Tile, float>();
        List<Tile> adjacent = new List<Tile>();

        open.Add(start);
        tileCost.Add(start, costFunc(start, movedFromTile));

        // While there is a node to explore
        while (open.Count > 0)
        {
            // Find the unvisited node with the least cost
            Tile currentTile = null;
            float minCost = float.MaxValue;
            foreach (Tile tile in open)
            {
                float cost = tileCost[tile];
                if (cost < minCost)
                {
                    currentTile = tile;
                    minCost = cost;
                }
            }

            // If we found the goal, construct the path and return it
            if (currentTile == goal)
            {
                List<Tile> finalPath = new List<Tile>();
                finalPath.Add(currentTile);

                while (movedFromTile.ContainsKey(currentTile))
                {
                    currentTile = movedFromTile[currentTile];
                    finalPath.Add(currentTile);
                }

                finalPath.Reverse();
                return finalPath;
            }
            
            // Find adjacent nodes that we have not yet searched
            m_level.GetAdjacent(currentTile, adjacent);
            for (int i = 0; i < adjacent.Count; i++)
            {
                Tile tile = adjacent[i];
                if (!closed.Contains(tile) && !movedFromTile.ContainsKey(tile))
                {
                    movedFromTile.Add(tile, currentTile);
                    open.Add(tile);
                    tileCost.Add(tile, costFunc(tile, movedFromTile));
                }
            }

            open.Remove(currentTile);
            closed.Add(currentTile);
        }
        return null;
    }

    private void PlacePlayer()
    {
        Room spawnRoom = null;
        float bestSuitability = Mathf.NegativeInfinity;
        foreach (Room room in m_level.Rooms)
        {
            float suitability = room.Bounds.center.magnitude - room.GetSize();
            if (suitability > bestSuitability)
            {
                spawnRoom = room;
                bestSuitability = suitability;
            }
        }

        if (spawnRoom != null)
        {
            Vector3 spawnPos = spawnRoom.Tiles.First(t => (t.y == spawnRoom.Floor)).Position;

            GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
            if (existingPlayer != null)
            {
                Destroy(existingPlayer);
            }

            Player player = Instantiate(Resources.Load<Player>("Player/Player"));
            player.transform.SetParent(m_otherParent, true);
            player.Spawn(Skinner.TransformPoint(spawnPos), Quaternion.identity);
        }
        else
        {
            Debug.LogError("No room found to spawn Player in!");
        }
    }

    private void PlaceEnemies()
    {
        foreach (var enemyParams in m_params.EnemyParams)
        {
            int count = UnityEngine.Random.Range(enemyParams.MinCount, enemyParams.MaxCount + 1);

            for (int i = 0; i < count; i++)
            {
                Room spawnRoom = Utils.PickRandom(m_level.Rooms);
                Vector3 spawnPos = spawnRoom.Tiles.First(t => (t.y == spawnRoom.Floor)).Position;

                Instantiate(enemyParams.Prefab, Skinner.TransformPoint(spawnPos), Quaternion.identity, m_otherParent);
            }
        }
    }

    private float ValueFromDistribution(AnimationCurve distribution, float minValue, float maxValue)
    {
        return distribution.Evaluate(UnityEngine.Random.value) * (maxValue - minValue) + minValue;
    }

    private float SampleNoise(float x, float z)
    {
        return Mathf.PerlinNoise((x * m_noiseScale) + m_noiseOffsetX, (z * m_noiseScale) + m_noiseOffsetY);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Skinner.DrawDebug(m_level);
        }
    }
}
