using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CorridorGraph : MonoBehaviour
{
    [SerializeField]
    private bool m_autoCreate = true;
    [SerializeField]
    private int m_initialMapSize = 100;
    [SerializeField]
    public CorridorTheme[] corridorThemes;

    private readonly string[] cardinalDirections = { "up", "right", "down", "left", };

    public GraphNode<Node, Edge> root;
    
    private Transform m_levelRoot;
    private Transform m_levelNavmesh;
    private Transform m_levelOther;
    private NavMeshManager m_navMeshManager;

    private void Start()
    {
        m_levelRoot = new GameObject("LevelMain").transform;
        m_levelNavmesh = new GameObject("LevelNavmesh").transform;
        m_levelOther = new GameObject("LevelOther").transform;
        m_levelNavmesh.SetParent(m_levelRoot);
        m_levelOther.SetParent(m_levelRoot);

        m_navMeshManager = m_levelNavmesh.gameObject.AddComponent<NavMeshManager>();

        root = new GraphNode<Node, Edge>(new Node(Vector3.zero));
        
        if (m_autoCreate)
        {
            for (int i = 0; i < m_initialMapSize; i++)
            {
                addSingleNode();
            }
            root.traverse(null, SkinNode);
            m_navMeshManager.BuildNavMesh();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("b"))
        {
            addSingleNode();
        }
        if (Input.GetKeyDown("p"))
        {
            InvokeRepeating("addSingleNode", 0, 0.03f);
        }
        if (Input.GetKeyDown("o"))
        {
            CancelInvoke();
        }
        if (Input.GetKeyDown("y"))
        {
            root.traverse(null, SkinNode);
        }
    }

    private void addSingleNode()
    {
        GraphNode<Node, Edge> validNode;
        string validInstruction;

        findValidNodeAndInstructionToAdd(out validNode, out validInstruction);
        if (validNode == null)
        {
            print("Couldn't find a node to add on to!");
            return;
        }
        
        GraphEdge<Node, Edge> validEdge;
        addNodeUsingDirection(validNode, validInstruction, out validEdge);
    }

    private void findValidNodeAndInstructionToAdd(out GraphNode<Node, Edge> validNode, out string validInstruction)
    {
        LinkedList<GraphNode<Node, Edge>> possibleNodes = new LinkedList<GraphNode<Node, Edge>>();
        root.addGraphToList(null, possibleNodes);

        //possibleNodes = new LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge> >(possibleNodes.Skip(possibleNodes.Count - 1));
        //possibleNodes = new LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge> >(possibleNodes.Where (((a)=>a.adjacentBipartiteNodes.Count <= 1)));

        GraphNode<Node, Edge> nodeToAppendTo = null;
        string goodInstruction = null;

        while (possibleNodes.Count > 0)
        {
            GraphNode<Node, Edge> nodeToTryToAppendTo = pickNodeToAppendTo(possibleNodes);
            string foundInstruction = findValidInstructionToAdd(nodeToTryToAppendTo);

            if (foundInstruction != null)
            {
                nodeToAppendTo = nodeToTryToAppendTo;
                goodInstruction = foundInstruction;
                break;
            }
            possibleNodes.Remove(nodeToTryToAppendTo);
        }

        validNode = nodeToAppendTo;
        validInstruction = goodInstruction;
    }

    private string findValidInstructionToAdd(GraphNode<Node, Edge> node)
    {
        // TODO: Remember which instructions you've used up, or proven invalid, for this node.
        LinkedList<string> instructionsToTry = new LinkedList<string>(cardinalDirections);
        //LinkedList<string> instructionsToTry = new LinkedList<string>();
        //instructionsToTry.AddLast("up");
        string chosenInstruction;

        LinkedList<string> directionsAttatched = new LinkedList<string>();

        foreach (GraphEdge<Node, Edge> neighbour in node.adjacentBipartiteNodes)
        {
            //directionsAttatched.AddLast(nodeDir(node, neighbour));
            directionsAttatched.AddLast(nodeDir(neighbour.adjacentBipartiteNodes.First(((otherNode) => otherNode != node)), neighbour));
        }

        while (instructionsToTry.Count > 0)
        {
            //pass in a list instead:
            //chosenInstruction = pickInstructionToTest(node.neibours.directions, instructionsToTry);
            //chosenInstruction = pickInstructionToTest(nodeDir(node, node.adjacentBipartiteNodes.First.Value), instructionsToTry);
            chosenInstruction = pickInstructionToTest(directionsAttatched, instructionsToTry);

            GraphEdge<Node, Edge> addedEdge;
            //GraphNode<PathInstructionNode, PathInstructionEdge> addedNode = node.addNode(new PathInstructionEdge("manhattan", chosenInstruction, null), new PathInstructionNode(new Vector3()), out addedEdge);
            GraphNode<Node, Edge> addedNode = addNodeUsingDirection(node, chosenInstruction, out addedEdge);
            //spooky!
            //addedEdge.data.upperRightNode = null;
            //node.traverse(parent, computePosition);
            //node.traverse(null, computePosition);
            //computePosition(node);

            bool valid = true;
            LinkedList<GraphNode<Node, Edge>> nodesToValidateAgainst = new LinkedList<GraphNode<Node, Edge>>();
            root.addGraphToList(null, nodesToValidateAgainst);

            while (valid && nodesToValidateAgainst.Count > 0)
            {
                GraphNode<Node, Edge> nodeToCheck = nodesToValidateAgainst.First.Value;
                valid = validateNodePositions(addedNode, nodeToCheck);
                nodesToValidateAgainst.RemoveFirst();
            }

            node.removeNode(addedEdge, addedNode);
            //spooky!
            addedEdge.data.upperOrRightNode = null;

            //print(chosenInstruction);
            //print(valid);
            if (valid)
            {
                return chosenInstruction;
                //return new PathInstructionEdge("manhattan", chosenInstruction, null);
            }

            instructionsToTry.Remove(chosenInstruction);
            //print(instructionsToTry.Count);
        }
        return null;
    }

    private bool validateNodePositions(GraphNode<Node, Edge> nodeA, GraphNode<Node, Edge> nodeB)
    {
        Vector3 relativePosition = nodeA.data.position - nodeB.data.position;

        return (
            relativePosition.sqrMagnitude > 4.1f ||
            nodeA.addGraphToListWithDepth(null, 1, new LinkedList<GraphNode<Node, Edge>>()).Contains(nodeB)
            ) && (
            relativePosition.sqrMagnitude > 8.1f ||
            nodeA.addGraphToListWithDepth(null, 2, new LinkedList<GraphNode<Node, Edge>>()).Contains(nodeB)
            );
    }

    private string pickInstructionToTest(LinkedList<string> directionsAttatched, LinkedList<string> instructionsToTry)
    {
        string bestInstruction = "fail";
        float bestInstructionScore = Mathf.NegativeInfinity;

        foreach (string possibleDirection in instructionsToTry)
        {
            float score = calculateInstructionPenaltyScore(directionsAttatched, possibleDirection);
            if (bestInstructionScore < score)
            {
                bestInstructionScore = score;
                bestInstruction = possibleDirection;
            }
        }
        return bestInstruction;
    }

    private float calculateInstructionPenaltyScore(LinkedList<string> directionsAttatched, string possibleDirection)
    {

#if true
        float totalScore = 0.0f;
        foreach (string neighbourDirection in directionsAttatched)
        {
            totalScore += -Mathf.Pow((2 - (((Mathf.Abs(dirIndex(neighbourDirection) -
                                    dirIndex(possibleDirection))
                          - 1) % 2) + 1)), 2);
        }

        return directionsAttatched.Count == 0 ? 0 : (totalScore / directionsAttatched.Count) + 1.9f * Random.value;
#endif

#if false
		return directionsAttatched.Count == 0 ? 0 : -Mathf.Pow((((Mathf.Abs(	dirIndex(directionsAttatched.First.Value) -
										dirIndex(possibleDirection))
							- 1) % 2) + 1), 2) + 1.9f * Random.value;
#endif

#if false
		return -Mathf.Pow((((Mathf.Abs(	dirIndex(nodeDirection) -
		                               dirIndex(possibleDirection))
		                     - 1) % 2) + 1), 2) + 1.9f * Random.value;
#endif
    }

    private void computePosition(GraphNode<Node, Edge> node, GraphEdge<Node, Edge> edgeToComputeOver)
    {
        Dictionary<string, Vector3> directions = new Dictionary<string, Vector3>();
        directions.Add("up", Vector3.forward);
        directions.Add("down", Vector3.back);
        directions.Add("left", Vector3.left);
        directions.Add("right", Vector3.right);
        
        GraphNode<Node, Edge> referenceNode = edgeToComputeOver.adjacentBipartiteNodes.First(otherNode => otherNode != node);
        node.data.position = referenceNode.data.position + 2 * directions[nodeDir(node, edgeToComputeOver)];
    }

    private float calculateNodePenaltyScore(GraphNode<Node, Edge> node)
    {
        //if (node.parent == null && node.edges.Count < 3) {return 10;} //useful to force a child count
        return -node.adjacentBipartiteNodes.Count + 3.5f * Random.value;
    }

    private GraphNode<Node, Edge> pickNodeToAppendTo(LinkedList<GraphNode<Node, Edge>> nodesToChooseFrom)
    {
        GraphNode<Node, Edge> bestNode = root;
        float bestScore = Mathf.NegativeInfinity;

        foreach (GraphNode<Node, Edge> node in nodesToChooseFrom)
        {
            float score = calculateNodePenaltyScore(node);
            if (bestScore < score)
            {
                bestScore = score;
                bestNode = node;
            }
        }
        return bestNode;
    }

    private void SkinNode(GraphNode<Node, Edge> node)
    {
        CorridorTheme theme = corridorThemes.First();

        LinkedList<GraphEdge<Node, Edge>> neighbours = node.adjacentBipartiteNodes;
        List<string> directionsAttatched = new List<string>();
        List<Transform> reps = node.data.representations;

        foreach (GraphEdge<Node, Edge> neighbour in neighbours)
        {
            directionsAttatched.Add(nodeDir(neighbour.adjacentBipartiteNodes.First(otherNode => otherNode != node), neighbour));
        }

        // only update nodes that have changed since they were last skinned
        if (node.data.neighboursWhenLastPlaced == null || !directionsAttatched.SequenceEqual(node.data.neighboursWhenLastPlaced))
        {
            foreach (Transform representation in reps)
            {
                Destroy(representation.gameObject);
            }
            reps.Clear();
        }
        else
        {
            return;
        }

        node.data.neighboursWhenLastPlaced = directionsAttatched;

        Vector3 tilePosition = node.data.position;
        Quaternion faceFirstNeighbour = Quaternion.Euler(0, dirIndex(neighbours.Count > 0 ? nodeDir(node, neighbours.First.Value) : "up") * 90, 0);

        // place lighting
        if (Random.value < theme.lightChance)
        {
            Transform lightPrefab = Utils.PickRandom(Random.value < theme.spinLightProportion ? theme.spinLight : theme.light);
            reps.Add(Instantiate(lightPrefab, tilePosition, faceFirstNeighbour, m_levelOther) as Transform);
        }
        
        bool isDeadEnd = neighbours.Count == 1;
        bool isCorridor = neighbours.Count == 2 && !((((Mathf.Abs(dirIndex(node, neighbours.First.Value) - dirIndex(node, neighbours.Last.Value)) - 1) % 2) == 0));

        // place flooring
        Transform floor = Utils.PickRandom(isCorridor || isDeadEnd ? theme.floorCorridor : theme.floorJunction);
        reps.Add(Instantiate(floor, tilePosition, faceFirstNeighbour, m_levelNavmesh) as Transform);

        // place roof and walls
        foreach (string direction in cardinalDirections)
        {
            Quaternion faceDirection = Quaternion.Euler(0, dirIndex(direction) * 90, 0);

            if (!directionsAttatched.Contains(direction))
            {
                Transform wall = Utils.PickRandom(theme.wall);
                if (isDeadEnd && (directionsAttatched.Contains(cardinalDirections[(dirIndex(direction) + 2) % 4])))
                {
                    wall = Utils.PickRandom(theme.wallDeadEnd);
                }
                reps.Add(Instantiate(wall, tilePosition, faceDirection, m_levelNavmesh) as Transform);
                reps.Add(Instantiate(Utils.PickRandom(theme.roofToWall), tilePosition, faceDirection, m_levelOther) as Transform);
            }
            else
            {
                reps.Add(Instantiate(Utils.PickRandom(theme.roofToCorridor), tilePosition, faceDirection, m_levelOther) as Transform);
            }

            if (directionsAttatched.Contains(direction) == directionsAttatched.Contains(cardinalDirections[(dirIndex(direction) + 1) % 4]))
            {
                reps.Add(Instantiate(Utils.PickRandom(theme.junctionCorner), tilePosition, faceDirection, m_levelNavmesh) as Transform);
            }
        }
    }

    GraphNode<Node, Edge> addNodeUsingDirection(GraphNode<Node, Edge> node, string direction, out GraphEdge<Node, Edge> addedEdge)
    {
        string edgeDirection;
        if (direction == "up" || direction == "down")
        {
            edgeDirection = "up/down";
        }
        else if (direction == "left" || direction == "right")
        {
            edgeDirection = "left/right";
        }
        else
        {
            edgeDirection = "WARNING";
            print("WARNING");
        }

        GraphNode<Node, Edge> addedNode = node.addNode(new Edge(edgeDirection, null), new Node(new Vector3(9, 9, 9)), out addedEdge);

        if (direction == "up" || direction == "right")
        {
            addedEdge.data.upperOrRightNode = addedNode;
        }
        else if (direction == "down" || direction == "left")
        {
            addedEdge.data.upperOrRightNode = node;
        }
        else
        {
            print("WARNING");
        }
        
        computePosition(addedNode, addedEdge);

        return addedNode;
    }

    private int dirIndex(GraphNode<Node, Edge> node, GraphEdge<Node, Edge> edge)
    {
        return dirIndex(nodeDir(node, edge));
    }

    private int dirIndex(string direction)
    {
        return System.Array.IndexOf(cardinalDirections, direction);
    }

    private string nodeDir(GraphNode<Node, Edge> node, GraphEdge<Node, Edge> edge)
    {
        if (edge.data.direction == "up/down")
        {
            return (edge.data.upperOrRightNode == node) ? "up" : "down";
        }
        else if (edge.data.direction == "left/right")
        {
            return (edge.data.upperOrRightNode == node) ? "right" : "left";
        }
        else
        {
            print("WARNING");
            return "WARNING";
        }
    }
}

public class Node
{
    public Vector3 position;
    public List<string> neighboursWhenLastPlaced;
    public List<Transform> representations;

    public Node(Vector3 position)
    {
        this.position = position;
        neighboursWhenLastPlaced = null;
        representations = new List<Transform>();
    }
}

public class Edge
{
    public string direction;
    public GraphNode<Node, Edge> upperOrRightNode;

    public Edge(string direction, GraphNode<Node, Edge> upperRightNode)
    {
        this.direction = direction;
        this.upperOrRightNode = upperRightNode;
    }
}