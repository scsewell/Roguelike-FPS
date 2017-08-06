using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Framework;

namespace LevelGen
{
    public static class Skinner
    {
        private const float LEVEL_SCALE = 2.0f;

        /// <summary>
        /// Instantiates assets for the generated level.
        /// </summary>
        public  static void Skin(Level level, Transform navParent, Transform otherParent, GenerationParameters levelParams)
        {
            foreach (Room room in level.Rooms)
            {
                RoomTheme roomTheme = Utils.PickRandom(levelParams.RoomThemes);

                foreach (Tile tile in room.Tiles)
                {
                    Vector3 tileCenter = tile.Position * LEVEL_SCALE;

                    Transform floor = Utils.PickRandom(roomTheme.Floors);
                    Object.Instantiate(floor, tileCenter, Quaternion.identity, navParent);

                    Transform roof = Utils.PickRandom(roomTheme.Roofs);
                    Object.Instantiate(roof, tileCenter, Quaternion.identity, otherParent);

                    for (int i = 0; i < 4; i++)
                    {
                        Quaternion dir = Quaternion.Euler(0, i * 90, 0);
                        Tile dirTile = level.GetTile(tile.Position + (dir * Vector3.forward));

                        if (dirTile == null || dirTile.Type == TileType.Wall)
                        {
                            Transform wall = Utils.PickRandom(roomTheme.Walls);
                            Object.Instantiate(wall, tileCenter, dir, navParent);
                        }
                    }
                }
            }
           
            CorridorTheme corridorTheme = Utils.PickRandom(levelParams.CorridorThemes);

            foreach (Tile tile in level.CorridorTiles)
            {
                Vector3 tileCenter = tile.Position * LEVEL_SCALE;

                List<Tile> adjacent = new List<Tile>();
                level.GetAdjacent(tile, adjacent);
                List<Tile> adjacentCorridors = adjacent.Where(t => t.Type == TileType.Corridor).ToList();
                List<Tile> adjacentRooms = adjacent.Where(t => t.Type == TileType.Room).ToList();

                Quaternion floorDir = Quaternion.identity;
                if (adjacentCorridors.Count > 0)
                {
                    floorDir = Quaternion.LookRotation(adjacentCorridors.First().Position - tile.Position);
                }
                else if (adjacentRooms.Count > 0)
                {
                    floorDir = Quaternion.LookRotation(adjacentRooms.First().Position - tile.Position);
                }

                bool isJunction = adjacentCorridors.Any(t => Mathf.Abs(Vector3.Dot(t.Position - tile.Position, floorDir * Vector3.forward)) < 0.9f);

                Transform floor = Utils.PickRandom(isJunction ? corridorTheme.JunctionFloors : corridorTheme.CorridorFloors);
                Object.Instantiate(floor, tileCenter, floorDir, navParent);

                for (int i = 0; i < 4; i++)
                {
                    Quaternion dir1 = Quaternion.Euler(0, i * 90, 0);
                    Tile dir1Tile = level.GetTile(tile.Position + (dir1 * Vector3.forward));
                    TileType dir1Type = Tile.GetNullSafeType(dir1Tile);

                    Quaternion dir2 = Quaternion.Euler(0, (i + 1) * 90, 0);
                    Tile dir2Tile = level.GetTile(tile.Position + (dir2 * Vector3.forward));
                    TileType dir2Type = Tile.GetNullSafeType(dir2Tile);

                    Transform roof;
                    if (dir1Type == TileType.Corridor)
                    {
                        roof = Utils.PickRandom(corridorTheme.RoofToCorridors);
                    }
                    else
                    {
                        roof = Utils.PickRandom(corridorTheme.RoofToWalls);

                        if (dir1Type == TileType.Wall)
                        {
                            Transform wall = Utils.PickRandom(corridorTheme.Walls);
                            Object.Instantiate(wall, tileCenter, dir1, navParent);
                        }
                    }
                    Object.Instantiate(roof, tileCenter, dir1, otherParent);

                    if ((dir1Type == dir2Type) || (dir1Type == TileType.Room || dir2Type == TileType.Room))
                    {
                        Transform corner = Utils.PickRandom(corridorTheme.JunctionCorners);
                        Object.Instantiate(corner, tileCenter, dir1, navParent);
                    }
                }

                if (Random.value < corridorTheme.LightChance)
                {
                    Transform light;
                    if (Random.value < corridorTheme.AltLightProportion)
                    {
                        light = Utils.PickRandom(corridorTheme.AltLights);
                    }
                    else
                    {
                        light = Utils.PickRandom(corridorTheme.Lights);
                    }
                    Object.Instantiate(light, tileCenter, floorDir, otherParent);
                }
            }
        }

        public static Vector3 TransformPoint(Vector3 point)
        {
            return point * LEVEL_SCALE;
        }

        public static void DrawDebug(Level level)
        {
            if (level != null)
            {
                level.DrawDebug(LEVEL_SCALE);
            }
        }
    }
}
