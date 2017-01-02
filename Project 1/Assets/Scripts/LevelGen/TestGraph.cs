using System.Collections.Generic;
using System.Linq;
using System;

public class GraphBipartiteNode<SimilarData, AdjacentData, SimilarBipartiteNode, AdjacentBipartiteNode>
    where AdjacentBipartiteNode : GraphBipartiteNode<AdjacentData, SimilarData, AdjacentBipartiteNode, SimilarBipartiteNode>
    where SimilarBipartiteNode : GraphBipartiteNode<SimilarData, AdjacentData, SimilarBipartiteNode, AdjacentBipartiteNode>
{
    public SimilarData data;
    public LinkedList<AdjacentBipartiteNode> adjacentBipartiteNodes;

    public GraphBipartiteNode(SimilarData data)
    {
        this.data = data;
        adjacentBipartiteNodes = new LinkedList<AdjacentBipartiteNode>();
    }

    /*
	*/
    /*
	public AdjacentBipartiteNode addSibling(AdjacentData newData, Func<AdjacentData, AdjacentBipartiteNode> adjacentBipartiteNodeMaker)
	{
		return connectSibling(adjacentBipartiteNodeMaker(newData));
	}
	/*
	*/

    public AdjacentBipartiteNode connectSibling(AdjacentBipartiteNode newSibling)
    {
        adjacentBipartiteNodes.AddFirst(newSibling);
        newSibling.adjacentBipartiteNodes.AddFirst((SimilarBipartiteNode)this);
        return newSibling;
    }

    public AdjacentBipartiteNode disconnectSibling(AdjacentBipartiteNode newSibling)
    {
        adjacentBipartiteNodes.Remove(newSibling);
        //boolean, hmm
        newSibling.adjacentBipartiteNodes.Remove((SimilarBipartiteNode)this);
        return newSibling;
    }

    public AdjacentBipartiteNode getSiblingAtIndex(int i)
    {
        foreach (AdjacentBipartiteNode adjacentBipartiteNode in adjacentBipartiteNodes)
        {
            if (i-- == 0)
            {
                return adjacentBipartiteNode;
            }
        }
        return null;
    }

    public void traverse(AdjacentBipartiteNode adjacentBipartiteNodeThatWeTraversedFrom,
                         Action<SimilarBipartiteNode> similarVisitor,
                         Action<AdjacentBipartiteNode> adjacentVisitor)
    {
        traverse(adjacentBipartiteNodeThatWeTraversedFrom, similarVisitor, adjacentVisitor, ((_) => true), ((_) => true));
    }

    public void traverse(AdjacentBipartiteNode adjacentBipartiteNodeThatWeTraversedFrom,
                         Action<SimilarBipartiteNode> similarVisitor,
                         Action<AdjacentBipartiteNode> adjacentVisitor,
                         Predicate<SimilarBipartiteNode> testToVisitSimilar,
                         Predicate<AdjacentBipartiteNode> testToVisitAdjacent)
    {
        similarVisitor((SimilarBipartiteNode)this);
        foreach (AdjacentBipartiteNode adjacentBipartiteNode in adjacentBipartiteNodes)
        {
            if (adjacentBipartiteNode != adjacentBipartiteNodeThatWeTraversedFrom && testToVisitAdjacent(adjacentBipartiteNode))
            {
                adjacentBipartiteNode.traverse((SimilarBipartiteNode)this, adjacentVisitor, similarVisitor, testToVisitAdjacent, testToVisitSimilar);
            }
        }
    }

    public void traverse(AdjacentBipartiteNode adjacentBipartiteNodeThatWeTraversedFrom,
                         Action<SimilarBipartiteNode, AdjacentBipartiteNode> similarVisitor,
                         Action<AdjacentBipartiteNode, SimilarBipartiteNode> adjacentVisitor)
    {
        similarVisitor((SimilarBipartiteNode)this, adjacentBipartiteNodeThatWeTraversedFrom);
        foreach (AdjacentBipartiteNode adjacentBipartiteNode in adjacentBipartiteNodes)
        {
            if (adjacentBipartiteNode != adjacentBipartiteNodeThatWeTraversedFrom)
            {
                adjacentBipartiteNode.traverse((SimilarBipartiteNode)this, adjacentVisitor, similarVisitor);
            }
        }
    }

    public void traverseWithDepth(AdjacentBipartiteNode adjacentBipartiteNodeThatWeTraversedFrom,
                                  int maxSimilarDepth,
                                  int maxAdjacentDepth,
                                  Action<SimilarBipartiteNode> similarVisitor,
                                  Action<AdjacentBipartiteNode> adjacentVisitor)
    {
        traverseWithDepth(adjacentBipartiteNodeThatWeTraversedFrom, 0, 0, maxSimilarDepth, maxAdjacentDepth, similarVisitor, adjacentVisitor, ((_) => true), ((_) => true));
    }

    public void traverseWithDepth(AdjacentBipartiteNode adjacentBipartiteNodeThatWeTraversedFrom,
                                  int currentSimilarDepth,
                                  int currentAdjacentDepth,
                                  int maxSimilarDepth,
                                  int maxAdjacentDepth,
                                  Action<SimilarBipartiteNode> similarVisitor,
                                  Action<AdjacentBipartiteNode> adjacentVisitor,
                                  Predicate<SimilarBipartiteNode> testToVisitSimilar,
                                  Predicate<AdjacentBipartiteNode> testToVisitAdjacent)
    {
        similarVisitor((SimilarBipartiteNode)this);
        foreach (AdjacentBipartiteNode adjacentBipartiteNode in adjacentBipartiteNodes)
        {
            if (adjacentBipartiteNode != adjacentBipartiteNodeThatWeTraversedFrom && currentSimilarDepth < maxSimilarDepth && testToVisitAdjacent(adjacentBipartiteNode))
            {
                adjacentBipartiteNode.traverseWithDepth((SimilarBipartiteNode)this, currentAdjacentDepth, currentSimilarDepth + 1, maxSimilarDepth, maxAdjacentDepth, adjacentVisitor, similarVisitor, testToVisitAdjacent, testToVisitSimilar);
            }
        }
    }

    public void traverseWithPath(AdjacentBipartiteNode adjacentBipartiteNodeThatWeTraversedFrom,
                                 Action<SimilarBipartiteNode, LinkedList<SimilarBipartiteNode>, LinkedList<AdjacentBipartiteNode>> similarVisitor,
                                 Action<AdjacentBipartiteNode, LinkedList<AdjacentBipartiteNode>, LinkedList<SimilarBipartiteNode>> adjacentVisitor)
    {
        traverseWithPath(adjacentBipartiteNodeThatWeTraversedFrom, new LinkedList<SimilarBipartiteNode>(), new LinkedList<AdjacentBipartiteNode>(), similarVisitor, adjacentVisitor, ((_) => true), ((_) => true));
    }

    //doesn't stop HAS NO BRAKES
    public void traverseWithPath(AdjacentBipartiteNode adjacentBipartiteNodeThatWeTraversedFrom,
                                 LinkedList<SimilarBipartiteNode> similarBipartiteNodesInPath,
                                 LinkedList<AdjacentBipartiteNode> adjacentBipartiteNodesInPath,
                                 Action<SimilarBipartiteNode, LinkedList<SimilarBipartiteNode>, LinkedList<AdjacentBipartiteNode>> similarVisitor,
                                 Action<AdjacentBipartiteNode, LinkedList<AdjacentBipartiteNode>, LinkedList<SimilarBipartiteNode>> adjacentVisitor,
                                 Predicate<SimilarBipartiteNode> testToVisitSimilar,
                                 Predicate<AdjacentBipartiteNode> testToVisitAdjacent)
    {
        similarVisitor((SimilarBipartiteNode)this, similarBipartiteNodesInPath, adjacentBipartiteNodesInPath);
        foreach (AdjacentBipartiteNode adjacentBipartiteNode in adjacentBipartiteNodes)
        {
            if (adjacentBipartiteNode != adjacentBipartiteNodeThatWeTraversedFrom && testToVisitAdjacent(adjacentBipartiteNode))
            {
                LinkedList<SimilarBipartiteNode> newList = new LinkedList<SimilarBipartiteNode>(similarBipartiteNodesInPath);
                newList.AddLast((SimilarBipartiteNode)this);
                adjacentBipartiteNode.traverseWithPath((SimilarBipartiteNode)this, new LinkedList<AdjacentBipartiteNode>(adjacentBipartiteNodesInPath), newList, adjacentVisitor, similarVisitor, testToVisitAdjacent, testToVisitSimilar);
            }
        }
    }

    public void traverse1(AdjacentBipartiteNode adjacentBipartiteNodeThatWeTraversedFrom,
                          Action<SimilarBipartiteNode> similarVisitor,
                          Action<AdjacentBipartiteNode> adjacentVisitor,
                          Predicate<SimilarBipartiteNode> testToVisitSimilar,
                          Predicate<AdjacentBipartiteNode> testToVisitAdjacent)
    {
        Func<AdjacentBipartiteNode, Predicate<AdjacentBipartiteNode>> testToVisitAdjacentMaker = ((b) => ((a) => a != b && testToVisitAdjacent(a)));
        traverse2(similarVisitor, adjacentVisitor, testToVisitAdjacentMaker(adjacentBipartiteNodeThatWeTraversedFrom), ((b) => ((a) => a != b && testToVisitSimilar(a))), testToVisitAdjacentMaker);
    }

    public void traverse2(Action<SimilarBipartiteNode> similarVisitor,
                          Action<AdjacentBipartiteNode> adjacentVisitor,
                          Predicate<AdjacentBipartiteNode> testToVisitAdjacent,
                          Func<SimilarBipartiteNode, Predicate<SimilarBipartiteNode>> testToVisitSimilarMaker,
                          Func<AdjacentBipartiteNode, Predicate<AdjacentBipartiteNode>> testToVisitAdjacentMaker)
    {
        similarVisitor((SimilarBipartiteNode)this);
        foreach (AdjacentBipartiteNode adjacentBipartiteNode in adjacentBipartiteNodes)
        {
            if (testToVisitAdjacent(adjacentBipartiteNode))
            {
                adjacentBipartiteNode.traverse2(adjacentVisitor, similarVisitor, testToVisitSimilarMaker((SimilarBipartiteNode)this), testToVisitAdjacentMaker, testToVisitSimilarMaker);
            }
        }
    }

}

public class GraphNode<NodeData, EdgeData> : GraphBipartiteNode<NodeData, EdgeData, GraphNode<NodeData, EdgeData>, GraphEdge<NodeData, EdgeData>>
{
    public GraphNode(NodeData data) : base(data) {}

    public GraphNode<NodeData, EdgeData> addNode(EdgeData newEdgeData, NodeData newNodeData, out GraphEdge<NodeData, EdgeData> newEdge)
    {
        //GraphEdge<NodeData, EdgeData> newEdge = connectSibling(new GraphEdge<NodeData, EdgeData>(newEdgeData));
        newEdge = connectSibling(new GraphEdge<NodeData, EdgeData>(newEdgeData));
        GraphNode<NodeData, EdgeData> newNode = newEdge.connectSibling(new GraphNode<NodeData, EdgeData>(newNodeData));
        return newNode;
    }

    public GraphNode<NodeData, EdgeData> removeNode(GraphEdge<NodeData, EdgeData> edgeToRemove, GraphNode<NodeData, EdgeData> nodeToRemove)
    {
        edgeToRemove.disconnectSibling(this);
        edgeToRemove.disconnectSibling(nodeToRemove);
        return nodeToRemove;
    }

    public void traverseWithDepth(GraphEdge<NodeData, EdgeData> edgeThatWeTraversedFrom, int maxDepth,
                         Action<GraphNode<NodeData, EdgeData>> thisVisitor)
    {
        traverseWithDepth(edgeThatWeTraversedFrom, maxDepth, int.MaxValue, thisVisitor, ((GraphEdge<NodeData, EdgeData> edge) => { }));

        //traverse2(edgeThatWeTraversedFrom, thisVisitor, ((GraphEdge<NodeData, EdgeData> edge) => {}), ((GraphNode<NodeData, EdgeData> edge) => {return true;}), ((GraphEdge<NodeData, EdgeData> edge) => {return true;}));
    }

    public void traverseWithPath(GraphEdge<NodeData, EdgeData> edgeThatWeTraversedFrom,
                                 Action<GraphNode<NodeData, EdgeData>, LinkedList<GraphNode<NodeData, EdgeData>>, LinkedList<GraphEdge<NodeData, EdgeData>>> thisVisitor)
    {
        traverseWithPath(edgeThatWeTraversedFrom, thisVisitor, ((edge, edgeList, nodeList) => { }));
    }

    public void traverse(GraphEdge<NodeData, EdgeData> edgeThatWeTraversedFrom,
                         Action<GraphNode<NodeData, EdgeData>> thisVisitor)
    {
        traverse(edgeThatWeTraversedFrom, thisVisitor, ((GraphEdge<NodeData, EdgeData> edge) => { }));
    }

    public void traverse(GraphEdge<NodeData, EdgeData> edgeThatWeTraversedFrom,
                         Action<GraphNode<NodeData, EdgeData>, GraphEdge<NodeData, EdgeData>> thisVisitor)
    {
        traverse(edgeThatWeTraversedFrom, thisVisitor, ((GraphEdge<NodeData, EdgeData> edge, GraphNode<NodeData, EdgeData> node) => { }));
    }

    public LinkedList<GraphNode<NodeData, EdgeData>> addGraphToListWithDepth(GraphEdge<NodeData, EdgeData> edgeThatWeTraversedFrom, int maxDepth, LinkedList<GraphNode<NodeData, EdgeData>> list)
    {
        //traverse(edgeThatWeTraversedFrom, ((GraphNode<NodeData, EdgeData> node) => {list.AddLast(node);}), ((GraphEdge<NodeData, EdgeData> edge) => {}));
        traverseWithDepth(edgeThatWeTraversedFrom, maxDepth, ((GraphNode<NodeData, EdgeData> node) => { list.AddLast(node); }));
        return list;
    }

    public LinkedList<GraphNode<NodeData, EdgeData>> addGraphToList(GraphEdge<NodeData, EdgeData> edgeThatWeTraversedFrom, LinkedList<GraphNode<NodeData, EdgeData>> list)
    {
        return addGraphToListWithDepth(edgeThatWeTraversedFrom, int.MaxValue, list);
    }

    public int getSizeWithDepth(GraphEdge<NodeData, EdgeData> edgeThatWeTraversedFrom, int maxDepth)
    {
        int size = 0;
        //traverse(edgeThatWeTraversedFrom, ((GraphNode<NodeData, EdgeData> node) => {size++;}), ((GraphEdge<NodeData, EdgeData> edge) => {}));
        traverseWithDepth(edgeThatWeTraversedFrom, maxDepth, ((GraphNode<NodeData, EdgeData> node) => { size++; }));

        return size;
    }

    public int getSize(GraphEdge<NodeData, EdgeData> edgeThatWeTraversedFrom)
    {
        return getSizeWithDepth(edgeThatWeTraversedFrom, int.MaxValue);
    }
}

public class GraphEdge<NodeData, EdgeData> : GraphBipartiteNode<EdgeData, NodeData, GraphEdge<NodeData, EdgeData>, GraphNode<NodeData, EdgeData>>
{
    public GraphEdge(EdgeData data) : base(data) {}
}
