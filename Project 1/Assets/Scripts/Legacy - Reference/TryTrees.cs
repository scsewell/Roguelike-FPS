using UnityEngine;
using System.Collections.Generic;
using System.Linq;

delegate void TreeVisitor<T>(NTree<T> nodeData);

class NTree<T>
{
    public T data;
	public NTree<T> parent;
    public LinkedList< NTree<T> > children;

    public NTree(T data)
    {
        this.data = data;
		this.parent = null;
        children = new LinkedList<NTree<T>>();
    }

    public NTree<T> addChild(T data)
    {
		NTree<T> newChild = children.AddFirst(new NTree<T>(data)).Value;
		newChild.parent = this;
        return newChild;
    }
	
    public NTree<T> removeChild(NTree<T> child)
    {
		//boolean, hmm
		children.Remove(child);
		child.parent = null;
        return child;
    }

    public NTree<T> getChild(int i)
    {
        foreach (NTree<T> n in children)
            if (i-- == 0) return n;
        return null;
    }

    public void traverse(TreeVisitor<T> visitor)
    {
        visitor(this);
        foreach (NTree<T> kid in children)
            kid.traverse(visitor);
    }

    public void addTreeToList(LinkedList<NTree<T> > list)
    {
        list.AddLast(this);
        foreach (NTree<T> kid in children)
            kid.addTreeToList(list);
    }
	
	public int getSize()
	{
		int size = 0;
		traverse((NTree<T> node) => {size++;});
		
		return size;
	}
}

class PathInstruction
{
	// shouldn't have 2...
	private readonly string[] cardinalDirections2 = {"up", "right", "down", "left"};
	
	public string direction;
	public string pathType;
	public Vector3 position;
	
	public LinkedList<string> neighboursWhenLastPlaced;
	public LinkedList<Transform> representations;
	
	public PathInstruction (string pathType, string direction, Vector3 position)
	{
		this.pathType = pathType;
		this.direction = direction;
		this.position = position;
		
		this.neighboursWhenLastPlaced = null;
		this.representations = new LinkedList<Transform>();
	}
	
	public override string ToString() {
		return pathType + direction + position;
	}
	
	// DO NOT TRUST POSITION YET, NEEDS THOUGHT (currently enforcing by using junk vector)
	public PathInstruction makeInverse() {
		return new PathInstruction(pathType, cardinalDirections2[(System.Array.IndexOf(cardinalDirections2, direction) + 2) % 4], new Vector3(9, 9, 9));
	}
}

public class TryTrees : MonoBehaviour {

	public Transform[] prefabFloorCorridor = new Transform[1];
	public Transform[] prefabFloorJunction = new Transform[1];
	public Transform[] prefabWallCorridor = new Transform[1];
	public Transform[] prefabWallDeadEnd = new Transform[1];
	public Transform[] prefabRoofUnlighted = new Transform[1];
	public Transform[] prefabCornerPillar = new Transform[1];
	public Transform[] prefabRoofJunctionToWall = new Transform[1];
	public Transform[] prefabRoofJunctionToCorridor = new Transform[1];
	
	NTree<PathInstruction> root;
	
	bool autoCreate;
	
	private readonly string[] cardinalDirections = {"up", "right", "down", "left"};

	void Start () {
		
		root = new NTree<PathInstruction>(new PathInstruction("root", "", new Vector3(0,0,0)));
		//createTilesFromPositions(root);
		
		autoCreate = false;
		for(int i=0; i < 0; i++) {
			addSinglePathInstruction();
		}
		root.traverse(createTilesFromPositions);
		autoCreate = true;
	}
	
	void addSinglePathInstruction () {
		NTree<PathInstruction> validNode;
		PathInstruction validInstruction;
		
		findValidNodeAndInstructionToAdd(out validNode, out validInstruction);
		if (validNode == null) {
			print("Couldn't find a node to add on to!");
			return;
		}
	
		/*NTree<PathInstruction> addedNode = */validNode.addChild(validInstruction);
		validNode.traverse(computePosition);
		//root.traverse(computePosition);
		
		//deleteAllTiles(); //give nodes refs to game objects to optimize this?
		//root.traverse(createTilesFromPositions);
		//createTilesFromPositions(addedNode);
		if (autoCreate) {
			root.traverse(createTilesFromPositions);
		}
		print (root.getSize() + ":" + validInstruction.direction);
	}
	
	void findValidNodeAndInstructionToAdd(out NTree<PathInstruction> validNode, out PathInstruction validInstruction) {
		LinkedList<NTree<PathInstruction> > possibleNodes = new LinkedList<NTree<PathInstruction>>();
		root.addTreeToList(possibleNodes);
		
		NTree<PathInstruction> nodeToAppendTo = null;
		PathInstruction goodInstruction = null;
		
		while (possibleNodes.Count > 0) {
			NTree<PathInstruction> nodeToTryToAppendTo = pickNodeToAppendTo(possibleNodes);
			PathInstruction foundInstruction = findValidInstructionToAdd(nodeToTryToAppendTo);
			
			if(foundInstruction != null) {
				nodeToAppendTo = nodeToTryToAppendTo;
				goodInstruction = foundInstruction;
				break;
			}
			possibleNodes.Remove(nodeToTryToAppendTo);
		}
		
		validNode = nodeToAppendTo;
		validInstruction = goodInstruction;
	}
	
	PathInstruction findValidInstructionToAdd(NTree<PathInstruction> node) {
		// TODO: Remember which instructions you've used up, or proven invalid, for this node.
		LinkedList<string> instructionsToTry = new LinkedList<string>(cardinalDirections);
		string chosenInstruction;
		
		while (instructionsToTry.Count > 0) {
			chosenInstruction = pickInstructionToTest(node.data.direction, instructionsToTry);
			
			NTree<PathInstruction> addedNode = node.addChild(new PathInstruction("manhattan", chosenInstruction, new Vector3()));
			node.traverse(computePosition);
			
			bool valid = true;
			LinkedList<NTree<PathInstruction> > nodesToValidateAgainst = new LinkedList<NTree<PathInstruction>>();
			root.addTreeToList(nodesToValidateAgainst);
			
			while (valid && nodesToValidateAgainst.Count > 0) {
				NTree<PathInstruction> nodeToCheck = nodesToValidateAgainst.First.Value;
				valid = validateNodePositions(addedNode, nodeToCheck);
				nodesToValidateAgainst.RemoveFirst();
			}
			
			node.removeChild(addedNode);
			
			if(valid) {
				return new PathInstruction("manhattan", chosenInstruction, new Vector3());
			}
			
			instructionsToTry.Remove(chosenInstruction);
		}
		return null;
	}

 	bool validateNodePositions(NTree<PathInstruction> nodeA, NTree<PathInstruction> nodeB) {
		bool valid = true;
		
		Vector3 relativePosition = nodeA.data.position - nodeB.data.position;
		if (!(relativePosition.sqrMagnitude > 4.1f ||
		//if (relativePosition.magnitude > 2.5f ||
					nodeA == nodeB ||
					nodeA.parent == nodeB || nodeB.parent == nodeA
			)) {
			//don't touch sides (dist <= sqrt(2^2 + 0^2))
			valid = false;
		}
		if (!(relativePosition.sqrMagnitude > 8.1f ||
		//if (relativePosition.magnitude > 3.0f ||
					nodeA == nodeB ||
					nodeA.parent == nodeB || nodeB.parent == nodeA ||
					(nodeA.parent != null &&/* nodeB.parent != null &&*/ nodeA.parent == nodeB.parent) ||
					(nodeA.parent != null && nodeA.parent.parent == nodeB) ||
					(nodeB.parent != null && nodeB.parent.parent == nodeA)
			)) {
			//don't touch corners (sqrt(2^2 + 0^2) < dist <= sqrt(2^2 + 2^2))
			valid = false;
		}
		return valid;
	}
	
	string pickInstructionToTest(string nodeDirection, LinkedList<string> instructionsToTry) {
		string bestInstruction = "fail";
		float bestInstructionScore = Mathf.NegativeInfinity;
		
		foreach (string possibleDirection in instructionsToTry) {
			float score = calculateInstructionPenaltyScore(nodeDirection, possibleDirection);
			if (bestInstructionScore < score) {
				bestInstructionScore = score;
				bestInstruction = possibleDirection;
			}
		}
		return bestInstruction;
	}
	
	float calculateInstructionPenaltyScore(string nodeDirection, string possibleDirection) {
		return -Mathf.Pow((((Mathf.Abs(	dirIndex(nodeDirection) -
										dirIndex(possibleDirection))
							- 1) % 2) + 1), 2) + 1.9f * Random.value;
	}
	
	void deleteAllTiles() {
		prefabFloorCorridor.GetType();
		Transform[] allObjects = FindObjectsOfType(typeof(Transform)) as Transform[];
		foreach (Transform obj in allObjects) {
			foreach (Transform floorCorridor in prefabFloorCorridor) {
				if (obj.name == floorCorridor.name + "(Clone)") {
					Destroy(obj.gameObject);
				}
			}
        }
	}
	
	void printNode(NTree<PathInstruction> node) {
		print(node.children.Count.ToString() + node.data);
	}

	void computePosition(NTree<PathInstruction> node) {
		Dictionary<string, Vector3> directions = new Dictionary<string, Vector3>();
		directions.Add("up", new Vector3(0, 0, 2));
		directions.Add("down", new Vector3(0, 0, -2));
		directions.Add("left", new Vector3(-2, 0, 0));
		directions.Add("right", new Vector3(2, 0, 0));
		
		//use parent for performance 
		foreach (NTree<PathInstruction> kid in node.children) {
			kid.data.position = node.data.position + directions[kid.data.direction];// + new Vector3(0,0.01f,0);
		}
	}
	
	float calculateNodePenaltyScore(NTree<PathInstruction> node) {
		//if (node.parent == null && node.children.Count < 3) {return 10;} //useful to force a child count
		return -determineNeighbours(node).Count + 3.5f * Random.value;
	}
	
	NTree<PathInstruction> pickNodeToAppendTo(LinkedList< NTree<PathInstruction> > nodesToChooseFrom) {
		NTree<PathInstruction> bestNode = root;
		float bestScore = Mathf.NegativeInfinity;
		
		foreach (NTree<PathInstruction> node in nodesToChooseFrom) {
			float score = calculateNodePenaltyScore(node);
			if (bestScore < score) {
				bestScore = score;
				bestNode = node;
			}
		}
		return bestNode;
	}
	
	void createTilesFromPositions(NTree<PathInstruction> node) {
		LinkedList<Transform> reps = node.data.representations;
		LinkedList<PathInstruction> neighbours = determineNeighbours(node);
		LinkedList<string> directionsAttatched = new LinkedList<string>();
		
		foreach (PathInstruction neighbour in neighbours) {
			directionsAttatched.AddLast(neighbour.direction);
		}
		
		//add deadEnd support
		bool isJunction = true;
		if (neighbours.Count == 2
				&& !((((Mathf.Abs(	dirIndex(neighbours.First.Value.direction) -
									dirIndex(neighbours.Last.Value.direction))
					- 1) % 2) + 1) == 1)) {
			isJunction = false;
		}
		
		if (node.data.neighboursWhenLastPlaced == null || !directionsAttatched.SequenceEqual(node.data.neighboursWhenLastPlaced)) {
			foreach (Transform representation in reps) {
				Destroy(representation.gameObject);
			}
			reps.Clear();
		} else {
			return;
		}
		
		node.data.neighboursWhenLastPlaced = directionsAttatched;
		
		Vector3 tilePosition = node.data.position;
	
		if (isJunction == true) {
			
			Transform junctionFloor = choosePrefab(prefabFloorJunction);
			Transform junctionFarWall = choosePrefab(prefabWallCorridor);
			if (neighbours.Count == 1) {
				junctionFloor = choosePrefab(prefabFloorCorridor);
				junctionFarWall = choosePrefab(prefabWallDeadEnd);
			}
			
			reps.AddLast(Instantiate(junctionFloor, tilePosition
				, Quaternion.Euler(270, dirIndex(neighbours.First != null ? neighbours.First.Value.direction : "up") * 90, 0)) as Transform);
			
			foreach (string direction in cardinalDirections) {
				if (!directionsAttatched.Contains(direction)) {
					
					Transform junctionToUse = choosePrefab(prefabWallCorridor);
					if (neighbours.Count == 1
							&& (directionsAttatched.Contains(cardinalDirections[(dirIndex(direction) + 2) % 4]))) {
						junctionToUse = junctionFarWall;
					}
					reps.AddLast(Instantiate(junctionToUse,
						tilePosition + new Vector3(0, 1.0f, 0) +
						Quaternion.Euler(0, (dirIndex(direction) + 1) * 90, 0) * new Vector3(-1.0f, 0, 0),
						Quaternion.Euler(270, (dirIndex(direction) + 1) * 90, 0)) as Transform);
					
					reps.AddLast(Instantiate(choosePrefab(prefabRoofJunctionToWall),
						tilePosition + new Vector3(0, 1.5f, 0),
						Quaternion.Euler(270, (dirIndex(direction) + 1) * 90, 0)) as Transform);
					
					
				} else {
					
					reps.AddLast(Instantiate(choosePrefab(prefabRoofJunctionToCorridor),
						tilePosition + new Vector3(0, 1.5f, 0),
						Quaternion.Euler(270, (dirIndex(direction) + 0) * 90, 0)) as Transform);
				}
				
				if (directionsAttatched.Contains(direction) == directionsAttatched.Contains(cardinalDirections[(dirIndex(direction) + 1) % 4])) {
					
					reps.AddLast(Instantiate(choosePrefab(prefabCornerPillar),
						tilePosition + new Vector3(0, 1.0f, 0)
						+ Quaternion.Euler(0, (dirIndex(direction) + 0) * 90, 0) * new Vector3(1.0f, 0, 1.0f),
						Quaternion.Euler(270, (dirIndex(direction) + 1) * 90, 0)) as Transform);
				}
			}
		} else {
			int corridorDirIndex = dirIndex(neighbours.First.Value.direction);
			//string corridorDirIndex = dirIndex(neighbours.Last.Value.direction); //parent?
			
			reps.AddLast(Instantiate(choosePrefab(prefabFloorCorridor), tilePosition
				, Quaternion.Euler(270, corridorDirIndex * 90, 0)) as Transform);
			
			reps.AddLast(Instantiate(choosePrefab(prefabWallCorridor), tilePosition + new Vector3(0, 1.0f, 0)
				+ Quaternion.Euler(0, (corridorDirIndex + 0) * 90, 0) * new Vector3(-1.0f, 0, 0)
				, Quaternion.Euler(270, (corridorDirIndex + 0) * 90, 0)) as Transform);
			
			reps.AddLast(Instantiate(choosePrefab(prefabWallCorridor), tilePosition + new Vector3(0, 1.0f, 0)
				+ Quaternion.Euler(0, (corridorDirIndex + 2) * 90, 0) * new Vector3(-1.0f, 0, 0)
				, Quaternion.Euler(270, (corridorDirIndex + 2) * 90, 0)) as Transform);
			
			reps.AddLast(Instantiate(choosePrefab(prefabRoofUnlighted), tilePosition + new Vector3(0, 1.5f, 0)
				, Quaternion.Euler(270, corridorDirIndex * 90, 0)) as Transform);
			
		}
		// borked huzzah reps.First.renderer.material.color = colors[System.Math.Max(0,dirIndex(node))];
	}
	
	LinkedList<PathInstruction> determineNeighbours(NTree<PathInstruction> node) {
		LinkedList<PathInstruction> neighboursList = new LinkedList<PathInstruction>();
		
		foreach (NTree<PathInstruction> childNode in node.children) {
			neighboursList.AddLast(childNode.data);
		}
		
		if (node.parent != null) {
			neighboursList.AddLast(node.data.makeInverse());
		}
		
		return neighboursList;
	}
	
	int dirIndex(NTree<PathInstruction> node) {
		return dirIndex(node.data.direction);
	}
	
	int dirIndex(string direction) {
		return System.Array.IndexOf(cardinalDirections, direction);
	}
	
	Transform choosePrefab(Transform[] prefabs) {   
		int randomPrefab = Random.Range(0, prefabs.Length);
		return (Transform)prefabs.GetValue(randomPrefab);
	}

	void Update () {
		if(Input.GetKeyDown("b")) {
			addSinglePathInstruction();
		}
		
		if(Input.GetKeyDown("p")) {
			InvokeRepeating("addSinglePathInstruction", 0, 0.03f);
		}
	
		if(Input.GetKeyDown("o")) {
			CancelInvoke();
		}
	
		if(Input.GetKeyDown("y")) {
			root.traverse(createTilesFromPositions);
		}
	
		if(Input.GetKeyDown("t")) {
			autoCreate = !autoCreate;
		}
	}
}
