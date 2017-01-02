using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CorridorGraph : MonoBehaviour
{
    public bool autoCreate = true;
    public int initialMapSize = 100;

    public Transform[] prefabFloorCorridor;
    public Transform[] prefabFloorJunction;
    public Transform[] prefabWallCorridor;
    public Transform[] prefabWallDeadEnd;
    public Transform[] prefabRoofUnlighted;
    public Transform[] prefabCornerPillar;
    public Transform[] prefabRoofJunctionToWall;
    public Transform[] prefabRoofJunctionToCorridor;
    public Transform[] prefabRoofLightFixture;
    public Transform[] prefabRoofSpinLightFixture;

    public float chanceToPlaceRoofLight = 0.1f;
    public float chanceForRotatingLight = 0.5f;

    public GraphNode<Node, Edge> root;

    private readonly string[] cardinalDirections = { "up", "right", "down", "left", };

    private GameObject levelRoot;
    private GameObject levelNavmesh;
    private GameObject levelOther;

    private void Start()
    {
        levelRoot = new GameObject("LevelMain");
        levelNavmesh = new GameObject("LevelNavmesh");
        levelOther = new GameObject("LevelOther");

        levelNavmesh.transform.SetParent(levelRoot.transform);
        levelOther.transform.SetParent(levelRoot.transform);

        root = new GraphNode<Node, Edge>(new Node(Vector3.zero));
        
        for (int i = 0; i < initialMapSize; i++)
        {
            addSinglePathInstructionNode(i == initialMapSize - 1);
        }
        root.traverse(null, SkinNode);
    }

    private void Update()
    {
        if (Input.GetKeyDown("b"))
        {
            addSinglePathInstructionNode(true);
        }
        if (Input.GetKeyDown("p"))
        {
            InvokeRepeating("addSinglePathInstructionNodeNoPrint", 0, 0.03f);
        }
        if (Input.GetKeyDown("o"))
        {
            CancelInvoke();
            addSinglePathInstructionNode(true);
        }
        if (Input.GetKeyDown("y"))
        {
            root.traverse(null, SkinNode);
        }
        if (Input.GetKeyDown("t"))
        {
            autoCreate = !autoCreate;
        }
    }

    void addSinglePathInstructionNodeNoPrint()
    {
        addSinglePathInstructionNode(false);
    }

    void addSinglePathInstructionNode(bool printInfo)
    {
        GraphNode<Node, Edge> validNode;
        //PathInstructionEdge validInstructionEdge;
        string validInstruction;

        findValidNodeAndInstructionToAdd(out validNode, out validInstruction);
        if (validNode == null)
        {
            print("Couldn't find a node to add on to!");
            return;
        }

        //validNode.addChild(validInstructionEdge);
        GraphEdge<Node, Edge> validEdge;
        //validNode.addNode(validInstructionEdge, new PathInstructionNode(new Vector3(9, 9, 9)), out validEdge);
        GraphNode<Node, Edge> newNode = addNodeUsingDirection(validNode, validInstruction, out validEdge);
        //validNode.traverse(parent, computePosition);
        //validNode.traverse(null, computePosition);
        //computePosition(validNode);
        //root.traverse(computePosition);

        //deleteAllTiles(); //give nodes refs to game objects to optimize this?
        //root.traverse(createTilesFromPositions);
        //createTilesFromPositions(addedNode);
        if (autoCreate)
        {
            //root.traverse(null, createTilesFromPositions);
            SkinNode(validNode);
            SkinNode(newNode);
        }
        //print (root.getSize(null) + ":" + validInstruction);
        total++;
        if (printInfo)
        {
            print(total + ":" + validInstruction + ":" + newNode.data.position);
        }
        else
        //if((total++) % 10 == 0)
        //if((total++) % 1 == 0)
        if (total == totalPrintValue)
        {
            print(total + ":" + validInstruction + ":" + newNode.data.position);
            totalPrintValue *= 2;
        }
    }
    int total = 0;
    int totalPrintValue = 1;

    void findValidNodeAndInstructionToAdd(out GraphNode<Node, Edge> validNode, out string validInstruction)
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

    string findValidInstructionToAdd(GraphNode<Node, Edge> node)
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

    bool validateNodePositions(GraphNode<Node, Edge> nodeA, GraphNode<Node, Edge> nodeB)
    {
        Vector3 relativePosition = nodeA.data.position - nodeB.data.position;
#if false
		print(nodeA.data.position);
		print(nodeB.data.position);
		print(relativePosition.sqrMagnitude);
		print(nodeA.addGraphToListWithDepth(null, 0, new LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>>()).Contains(nodeB));
		print(nodeA.addGraphToListWithDepth(null, 1, new LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>>()).Contains(nodeB));
		print(nodeA.addGraphToListWithDepth(null, 2, new LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>>()).Contains(nodeB));
		print(nodeA.addGraphToListWithDepth(null, 3, new LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>>()).Contains(nodeB));
		print(nodeA.addGraphToListWithDepth(null, 4, new LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>>()).Contains(nodeB));
#endif

        return (
            relativePosition.sqrMagnitude > 4.1f ||
            nodeA.addGraphToListWithDepth(null, 1, new LinkedList<GraphNode<Node, Edge>>()).Contains(nodeB)
            ) && (
            relativePosition.sqrMagnitude > 8.1f ||
            nodeA.addGraphToListWithDepth(null, 2, new LinkedList<GraphNode<Node, Edge>>()).Contains(nodeB)
            );
    }

    bool validateNodePositionsOld(GraphNode<Node, Edge> nodeA, GraphNode<Node, Edge> nodeB)
    {
        bool valid = true;

        Vector3 relativePosition = nodeA.data.position - nodeB.data.position;
        //if (relativePosition.magnitude > 2.5f ||
        if (!(relativePosition.sqrMagnitude > 4.1f ||
              nodeA.addGraphToListWithDepth(null, 1, new LinkedList<GraphNode<Node, Edge>>()).Contains(nodeB)
            //nodeA == nodeB ||
            //nodeA.parent == nodeB || nodeB.parent == nodeA
            ))
        {
            //don't touch sides (dist <= sqrt(2^2 + 0^2))
            valid = false;
        }
        //if (relativePosition.magnitude > 3.0f ||
        if (!(relativePosition.sqrMagnitude > 8.1f ||
              nodeA.addGraphToListWithDepth(null, 2, new LinkedList<GraphNode<Node, Edge>>()).Contains(nodeB)
#if false
					nodeA == nodeB ||
					nodeA.parent == nodeB || nodeB.parent == nodeA ||
					//(nodeA.parent != null && nodeB.parent != null && nodeA.parent == nodeB.parent) ||
					(nodeA.parent != null && nodeA.parent == nodeB.parent) ||
					(nodeA.parent != null && nodeA.parent.parent == nodeB) ||
					(nodeB.parent != null && nodeB.parent.parent == nodeA)
#endif
            ))
        {
            //don't touch corners (sqrt(2^2 + 0^2) < dist <= sqrt(2^2 + 2^2))
            valid = false;
        }
        return valid;
    }

    string pickInstructionToTest(LinkedList<string> directionsAttatched, LinkedList<string> instructionsToTry)
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

    float calculateInstructionPenaltyScore(LinkedList<string> directionsAttatched, string possibleDirection)
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

    void deleteAllTiles()
    {
        prefabFloorCorridor.GetType();
        Transform[] allObjects = FindObjectsOfType(typeof(Transform)) as Transform[];
        foreach (Transform obj in allObjects)
        {
            foreach (Transform floorCorridor in prefabFloorCorridor)
            {
                if (obj.name == floorCorridor.name + "(Clone)")
                {
                    Destroy(obj.gameObject);
                }
            }
        }
    }

    void printNode(GraphNode<Node, Edge> node)
    {
        print(node.adjacentBipartiteNodes.Count.ToString() + node.data);
    }

    void computePosition(GraphNode<Node, Edge> node, GraphEdge<Node, Edge> edgeThatWeTraversedFrom)
    {
        Dictionary<string, Vector3> directions = new Dictionary<string, Vector3>();
        directions.Add("up", new Vector3(0, 0, 2));
        directions.Add("down", new Vector3(0, 0, -2));
        directions.Add("left", new Vector3(-2, 0, 0));
        directions.Add("right", new Vector3(2, 0, 0));

        //use parent for performance 
        foreach (GraphEdge<Node, Edge> kid in node.adjacentBipartiteNodes)
        {

            if (kid != edgeThatWeTraversedFrom)
            {
                GraphNode<Node, Edge> nodeToMove = kid.adjacentBipartiteNodes.First(((otherNode) => otherNode != node));
                //print("pushDir:" + nodeDir(nodeToMove, kid));
                //print("push:" + node.data.position + directions[nodeDir(nodeToMove, kid)]);
                nodeToMove.data.position = node.data.position + directions[nodeDir(nodeToMove, kid)];// + new Vector3(0,0.01f,0);
            }
        }
    }

    void computePositionOverEdge(GraphNode<Node, Edge> node, GraphEdge<Node, Edge> edgeToComputeOver)
    {
        Dictionary<string, Vector3> directions = new Dictionary<string, Vector3>();
        directions.Add("up", new Vector3(0, 0, 2));
        directions.Add("down", new Vector3(0, 0, -2));
        directions.Add("left", new Vector3(-2, 0, 0));
        directions.Add("right", new Vector3(2, 0, 0));

        //use parent for performance 
        GraphNode<Node, Edge> nodeToMove = edgeToComputeOver.adjacentBipartiteNodes.First(((otherNode) => otherNode != node));
        //print("pushDir:" + nodeDir(nodeToMove, edgeToComputeOver));
        //print("push:" + node.data.position + directions[nodeDir(nodeToMove, edgeToComputeOver)] + nodeDir(nodeToMove, edgeToComputeOver));
        nodeToMove.data.position = node.data.position + directions[nodeDir(nodeToMove, edgeToComputeOver)];// + new Vector3(0,0.01f,0);
    }

#if false
	void computePosition(GraphNode<PathInstructionNode, PathInstructionEdge> node) {
		Dictionary<string, Vector3> directions = new Dictionary<string, Vector3>();
		directions.Add("up", new Vector3(0, 0, 2));
		directions.Add("down", new Vector3(0, 0, -2));
		directions.Add("left", new Vector3(-2, 0, 0));
		directions.Add("right", new Vector3(2, 0, 0));

		//use parent for performance 
		foreach (GraphEdge<PathInstructionNode, PathInstructionEdge> kid in node.adjacentBipartiteNodes) {

			kid.adjacentBipartiteNodes.First(((otherNode)=>otherNode!=node)).data.position = node.data.position + directions[nodeDir(node, kid)];// + new Vector3(0,0.01f,0);
			print(node.data.position + directions[nodeDir(node, kid)]);// + new Vector3(0,0.01f,0);
		}
	}
#endif

    float calculateNodePenaltyScore(GraphNode<Node, Edge> node)
    {
        //if (node.parent == null && node.edges.Count < 3) {return 10;} //useful to force a child count
        return -node.adjacentBipartiteNodes.Count + 3.5f * Random.value;
    }

    GraphNode<Node, Edge> pickNodeToAppendTo(LinkedList<GraphNode<Node, Edge>> nodesToChooseFrom)
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
        List<Transform> reps = node.data.representations;
        LinkedList<GraphEdge<Node, Edge>> neighbours = node.adjacentBipartiteNodes;
        List<string> directionsAttatched = new List<string>();

        foreach (GraphEdge<Node, Edge> neighbour in neighbours)
        {
            directionsAttatched.Add(nodeDir(neighbour.adjacentBipartiteNodes.First(otherNode => otherNode != node), neighbour));
        }

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

        if (Random.value < chanceToPlaceRoofLight)
        {
            Transform lightPrefab = Utils.PickRandom(Random.value < chanceForRotatingLight ? prefabRoofSpinLightFixture : prefabRoofLightFixture);
            Quaternion rotation = Quaternion.Euler(0, dirIndex(neighbours.Count > 0 ? nodeDir(node, neighbours.First.Value) : "up") * 90, 0);
            reps.Add(Instantiate(lightPrefab, tilePosition, rotation) as Transform);
        }

        //add deadEnd support
        bool isJunction = true;
        if (neighbours.Count == 2
            && !((((Mathf.Abs(dirIndex(node, neighbours.First.Value) -
                                    dirIndex(node, neighbours.Last.Value))
                    - 1) % 2) + 1) == 1))
        {
            isJunction = false;
        }

        if (isJunction)
        {
            Transform junctionFloor = Utils.PickRandom(prefabFloorJunction);
            Transform junctionFarWall = Utils.PickRandom(prefabWallCorridor);
            if (neighbours.Count == 1)
            {
                junctionFloor = Utils.PickRandom(prefabFloorCorridor);
                junctionFarWall = Utils.PickRandom(prefabWallDeadEnd);
            }

            reps.Add(Instantiate(junctionFloor, tilePosition
                                     , Quaternion.Euler(270, dirIndex(neighbours.Count > 0 ? nodeDir(node, neighbours.First.Value) : "up") * 90, 0)) as Transform);
            
            foreach (string direction in cardinalDirections)
            {
                if (!directionsAttatched.Contains(direction))
                {
                    Transform junctionToUse = Utils.PickRandom(prefabWallCorridor);
                    if (neighbours.Count == 1
                            && (directionsAttatched.Contains(cardinalDirections[(dirIndex(direction) + 2) % 4])))
                    {
                        junctionToUse = junctionFarWall;
                    }
                    reps.Add(Instantiate(junctionToUse,
                        tilePosition + new Vector3(0, 1.0f, 0) +
                        Quaternion.Euler(0, (dirIndex(direction) + 1) * 90, 0) * new Vector3(-1.0f, 0, 0),
                        Quaternion.Euler(270, (dirIndex(direction) + 1) * 90, 0)) as Transform);

                    reps.Add(Instantiate(Utils.PickRandom(prefabRoofJunctionToWall),
                        tilePosition + new Vector3(0, 1.5f, 0),
                        Quaternion.Euler(270, (dirIndex(direction) + 1) * 90, 0)) as Transform);

                }
                else
                {
                    reps.Add(Instantiate(Utils.PickRandom(prefabRoofJunctionToCorridor),
                        tilePosition + new Vector3(0, 1.5f, 0),
                        Quaternion.Euler(270, (dirIndex(direction) + 0) * 90, 0)) as Transform);
                }

                if (directionsAttatched.Contains(direction) == directionsAttatched.Contains(cardinalDirections[(dirIndex(direction) + 1) % 4]))
                {
                    reps.Add(Instantiate(Utils.PickRandom(prefabCornerPillar),
                        tilePosition + new Vector3(0, 1.0f, 0)
                        + Quaternion.Euler(0, (dirIndex(direction) + 0) * 90, 0) * new Vector3(1.0f, 0, 1.0f),
                        Quaternion.Euler(270, (dirIndex(direction) + 1) * 90, 0)) as Transform);
                }
            }
        }
        else
        {
            int corridorDirIndex = dirIndex(node, neighbours.First.Value);
            //string corridorDirIndex = dirIndex(neighbours.Last.Value.direction); //parent?

            reps.Add(Instantiate(Utils.PickRandom(prefabFloorCorridor), tilePosition
                , Quaternion.Euler(270, corridorDirIndex * 90, 0)) as Transform);

            reps.Add(Instantiate(Utils.PickRandom(prefabWallCorridor), tilePosition + new Vector3(0, 1.0f, 0)
   + Quaternion.Euler(0, (corridorDirIndex + 0) * 90, 0) * new Vector3(-1.0f, 0, 0)
   , Quaternion.Euler(270, (corridorDirIndex + 0) * 90, 0)) as Transform);

            reps.Add(Instantiate(Utils.PickRandom(prefabWallCorridor), tilePosition + new Vector3(0, 1.0f, 0)
   + Quaternion.Euler(0, (corridorDirIndex + 2) * 90, 0) * new Vector3(-1.0f, 0, 0)
   , Quaternion.Euler(270, (corridorDirIndex + 2) * 90, 0)) as Transform);

            reps.Add(Instantiate(Utils.PickRandom(prefabRoofUnlighted), tilePosition + new Vector3(0, 1.5f, 0)
   , Quaternion.Euler(270, corridorDirIndex * 90, 0)) as Transform);

        }

        foreach (Transform instance in reps)
        {
            if (instance.parent == null)
            {
                instance.SetParent(levelRoot.transform, true);
            }
        }
    }

#if false
	LinkedList<PathInstructionNode> determineNeighbours(GraphNode<PathInstructionNode, PathInstructionEdge> node) {
		LinkedList<PathInstructionNode> neighboursList = new LinkedList<PathInstructionNode>();
		
		foreach (GraphNode<PathInstructionNode, PathInstructionEdge> childNode in node.edges) {
			neighboursList.AddLast(childNode.data);
		}
		
		if (node.parent != null) {
			neighboursList.AddLast(node.data.makeInverse());
		}
		
		return neighboursList;
	}
#endif

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

        //computePosition(node, null);
        computePositionOverEdge(node, addedEdge);

        return addedNode;
    }

    string nodeDir(GraphNode<Node, Edge> node, GraphEdge<Node, Edge> edge)
    {
        if (edge.data.direction == "up/down")
        {
            return (edge.data.upperOrRightNode == node) ? "up" : "down";
        }
        else if (edge.data.direction == "left/right")
        {
            //return (edge.data.upperOrRightNode == node) ? "left" : "right";
            return (edge.data.upperOrRightNode == node) ? "right" : "left";
        }
        else
        {
            print("WARNING");
            return "WARNING";
        }
    }

    private int dirIndex(GraphNode<Node, Edge> node, GraphEdge<Node, Edge> edge)
    {
        return dirIndex(nodeDir(node, edge));
    }

    private int dirIndex(string direction)
    {
        return System.Array.IndexOf(cardinalDirections, direction);
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