using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class EnemyAi : MonoBehaviour {

	private CorridorGraph map;
	private EnemyMovement enemyMovement;
	
	void Start () {
		map = GameObject.FindGameObjectWithTag("GameController").GetComponent<CorridorGraph>();
		enemyMovement = GetComponent<EnemyMovement>();
	}
	
	void Update () {

		foreach(var ztransform in GameObject.FindObjectsOfType<Transform>().Where(((etransform)=>{return etransform.name.Contains("Floor");})))
		{
			ztransform.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 1);
		}

/*
-use enemy position to pick closest graph node to it <GABE>
-use graph pathfinding algorithm to pick a set of potential graph paths to the goal <GABE>
-calculate the position resulting from a representative bunch of physics sweeps or alternative simulator/predictors
-decide, maybe for each sweep, which is the node in the path furthest from the goal that is not reachable/interactable by this sweep
-pick that node and some nodes immediatly preceding it; we consider them nodes whose distance from the sweep is a reliable signal of progress
-for each path, find the sweep that gives the furthest progress along that path
-decide which path/sweep pair to attempt this frame
-leave memory problems or blockage reasoning as the graph's responsibility
-[VVM]
*/


		Transform player = GameObject.FindGameObjectWithTag("Player").transform;
		Vector3 enemyPosition = transform.position;
		Vector3 playerPosition = player.position;

		var closestNodeToEnemy = closestGraphNode(enemyPosition, map.root);
		var closestNodeToPlayer = closestGraphNode(playerPosition, map.root);

		if (closestNodeToEnemy.data.representations.Count != 0)
		{
			Material enemyFloorMaterial = getFloorMaterialOfNode (closestNodeToEnemy);
						enemyFloorMaterial.color = new Color(
				(enemyFloorMaterial.color.r + 0.01f) % 1,
				enemyFloorMaterial.color.g,
				enemyFloorMaterial.color.b,
				enemyFloorMaterial.color.a);
		}
		
		if (closestNodeToPlayer.data.representations.Count != 0)
		{
			Material playerFloorMaterial = getFloorMaterialOfNode (closestNodeToPlayer);

			playerFloorMaterial.color = new Color(
				playerFloorMaterial.color.r,
				(playerFloorMaterial.color.g + 0.01f) % 1,
				playerFloorMaterial.color.b,
				playerFloorMaterial.color.a);
		}

		LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>> pathNodesToEnemy = null;

		closestNodeToPlayer.traverseWithPath (null, ((currentNode, nodeList, edgeList) =>  {
			if (currentNode == closestNodeToEnemy) {
				pathNodesToEnemy = nodeList;
			}
		}));

#if false
		foreach (var pathNode in pathNodesToEnemy)
		{
			if (pathNode.data.representations.Count != 0)
			{
				Material pathFloorMaterial = getFloorMaterialOfNode (pathNode);

				pathFloorMaterial.color = new Color(
					pathFloorMaterial.color.r,
					pathFloorMaterial.color.g,
					(pathFloorMaterial.color.b + 0.01f) % 1,
					pathFloorMaterial.color.a);
			}
		}
#endif

		LinkedList<Vector3> sweepPositions = getHackishPhysicsSweeps(enemyPosition, 1, 24);
		
		float minSweepPositionSignalReading = Mathf.Infinity;
		float maxSweepPositionSignalReading = Mathf.NegativeInfinity;
		Vector3 minimizingSweepPosition = Vector3.zero;

		int signalNodesCount = 1;

		//foreach (Vector3 sweepPosition in sweepPositions.Take(1))
		foreach (Vector3 sweepPosition in sweepPositions)
		{
			//Debug.DrawLine(enemyPosition, sweepPosition);

			LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>> truncatedReliablePathNodesToEnemy = new LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>>(pathNodesToEnemy.Reverse().SkipWhile(((pathNode)=>{return !getNodeSignalReliability(pathNode, sweepPosition);})).Reverse());

			LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>> signalPathNodesToEnemy = new LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>>(truncatedReliablePathNodesToEnemy.Skip(truncatedReliablePathNodesToEnemy.Count - signalNodesCount));

			#if true
			foreach (var pathNode in signalPathNodesToEnemy)
			{
				if (pathNode.data.representations.Count != 0)
				{
					Material pathFloorMaterial = getFloorMaterialOfNode (pathNode);
					
					#if true
					pathFloorMaterial.color = new Color(
						pathFloorMaterial.color.r,
						pathFloorMaterial.color.g,
						(pathFloorMaterial.color.b + 0.01f) % 1,
						pathFloorMaterial.color.a);
					#endif
					#if false
					pathFloorMaterial.color = new Color(1, 1, 0, 1);
					#endif
				}
			}
			#endif

			float sweepPositionSignalReading = 0;
			float signalWeight = 1;

			foreach (var pathNode in signalPathNodesToEnemy) {

				int pathIndex = pathNodesToEnemy.Select((item, inx) => new { item, inx })
					.First(x=> x.item == pathNode).inx;

				
				//pass in corridor scale
				sweepPositionSignalReading += signalWeight * ((2 * pathIndex) + getNodeBubbleDistance(pathNode, sweepPosition));
				signalWeight *= 2;
			}

			if (maxSweepPositionSignalReading < sweepPositionSignalReading) {
				maxSweepPositionSignalReading = sweepPositionSignalReading;
			}
			
			if (minSweepPositionSignalReading > sweepPositionSignalReading) {
				minSweepPositionSignalReading = sweepPositionSignalReading;
				minimizingSweepPosition = sweepPosition;
			}
		}

#if true
		foreach (Vector3 sweepPosition in sweepPositions)
		{
			LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>> truncatedReliablePathNodesToEnemy = new LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>>(pathNodesToEnemy.Reverse().SkipWhile(((pathNode)=>{return !getNodeSignalReliability(pathNode, sweepPosition);})).Reverse());

			LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>> signalPathNodesToEnemy = new LinkedList<GraphNode<PathInstructionNode, PathInstructionEdge>>(truncatedReliablePathNodesToEnemy.Skip(truncatedReliablePathNodesToEnemy.Count - signalNodesCount));

			float sweepPositionSignalReading = 0;
			float signalWeight = 1;

			foreach (var pathNode in signalPathNodesToEnemy) {
				
				int pathIndex = pathNodesToEnemy.Select((item, inx) => new { item, inx })
					.First(x=> x.item == pathNode).inx;
				
				
				//pass in corridor scale
				sweepPositionSignalReading += signalWeight * ((2 * pathIndex) + Vector3.Distance(sweepPosition, pathNode.data.position));
				signalWeight *= 2;
			}

			sweepPositionSignalReading = (sweepPositionSignalReading - minSweepPositionSignalReading) / (maxSweepPositionSignalReading - minSweepPositionSignalReading);
			//print (sweepPositionSignalReading);

			Debug.DrawLine(enemyPosition, sweepPosition, new Color(sweepPositionSignalReading, 0, sweepPositionSignalReading));
			//GUILayout.Label(""+sweepPositionSignal);
			//print(""+sweepPositionSignal);
		}
#endif
		
		Debug.DrawLine(enemyPosition, minimizingSweepPosition, new Color(0, 1, 1));

		enemyMovement.Move(minimizingSweepPosition);

	}

	//floats returns here could be critical later, think about
	//this depends a lot on the structure or phsical shape of a node; currently manhatteeeen corridors
	public bool getNodeSignalReliability (GraphNode<PathInstructionNode, PathInstructionEdge> testNode, Vector3 startPosition) 
	{
		//Box test?
		//pass in corridor scale
		//return Vector3.Distance(testNode.data.position, startPosition) > 2 * 2;
		return getNodeBubbleDistance(testNode, startPosition) > 2 * 1.5f;
	}

	public float getNodeBubbleDistance (GraphNode<PathInstructionNode, PathInstructionEdge> node, Vector3 enemyPosition) 
	{
		Vector3 nodePosition = node.data.position;
		return Mathf.Sqrt(Mathf.Pow(Mathf.Max( Mathf.Abs(nodePosition.x - enemyPosition.x) - 1, 0), 2) + Mathf.Pow(Mathf.Max( Mathf.Abs(nodePosition.z - enemyPosition.z) - 1, 0), 2));
	}
	
	public LinkedList<Vector3> getHackishPhysicsSweeps (Vector3 startPosition, float radius, int sweepResolution) 
	{
		LinkedList<Vector3> sweepPositions = new LinkedList<Vector3>();

		for (int sweepNumber = 0; sweepNumber < sweepResolution; sweepNumber++)
		{
			sweepPositions.AddLast(Quaternion.Euler(0, (sweepNumber/(float)sweepResolution) * 360, 0) *  new Vector3(radius, 0, 0) + startPosition);
		}

		return sweepPositions;
	}

	GraphNode<PathInstructionNode, PathInstructionEdge> closestGraphNode (Vector3 enemyPosition, GraphNode<PathInstructionNode, PathInstructionEdge> rootNode)
	{
		GraphNode<PathInstructionNode, PathInstructionEdge> closestNode = null;
		float closestNodeDistance = Mathf.Infinity;

		rootNode.traverse (null, ((currentNode) =>  {
			float currentNodeDistance = Vector3.Distance (currentNode.data.position, enemyPosition);
			if (currentNodeDistance < closestNodeDistance) {
				closestNodeDistance = currentNodeDistance;
				closestNode = currentNode;
			}
		}));
		return closestNode;
	}

	static Material getFloorMaterialOfNode (GraphNode<PathInstructionNode, PathInstructionEdge> node)
	{
		return node.data.representations.First (someTransform =>  {
			return someTransform.gameObject.name.Contains ("Floor");
		}).GetComponent<MeshRenderer> ().material;
	}
}