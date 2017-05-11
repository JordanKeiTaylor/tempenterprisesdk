package improbable.enterprise.routing;

import improbable.enterprise.extensions.CoordinateExtensions;
import improbable.math.Coordinates;

import java.util.*;

/**
 * Algorithm taken from https://en.wikipedia.org/wiki/A*_search_algorithm
 */
public class AStarRouter {

    public List<PathStep> findRoute(Graph graph, long startingNodeId, long endingNodeId) {

        if (startingNodeId == endingNodeId) {
            return Collections.emptyList();
        }

        // For each node, which node it can most efficiently be reached from.
        // If a node can be reached from many nodes, cameFrom will eventually contain the
        // most efficient previous step.
        Map<Long, PathStep> cameFrom = new HashMap<>();

        // For each node, the cost of getting from the start node to that node.
        Map<Long, Double> gScore = new HashMap<>();

        // The cost of going from start to start is zero.
        gScore.put(startingNodeId, 0d);

        // For each node, the total cost of getting from the start node to the goal
        // by passing by that node. That value is partly known, partly heuristic.
        final Map<Long, Double> fScore = new HashMap<>();

        // For the first node, that value is completely heuristic.
        fScore.put(startingNodeId, heuristicCostEstimate(graph, startingNodeId, endingNodeId));

        // The set of nodes already evaluated.
        Set<Long> closedSet = new HashSet<>();

        // The queue of currently discovered nodes that are not evaluated yet.
        // Prioritized by their fScore
        // Initially, only the start node is known.
        Queue<Long> openQueue = new PriorityQueue<>(Comparator.comparing(fScore::get));
        openQueue.add(startingNodeId);

        while (!openQueue.isEmpty()) {

            //the node in openQueue having the lowest fScore[] value
            Long current = openQueue.poll();

            if (current.equals(endingNodeId)) {
                return reconstructPath(cameFrom, current);
            }

            Optional<GraphNode> currentNode = graph.getNode(current);

            if (!currentNode.isPresent()) {
                // we don't have any information about this entry
                continue;
            }

            closedSet.add(current);

            for (GraphEdge neighborConnection : currentNode.get().getEdges()) {

                long neighborNode = neighborConnection.getNodeId();

                if (!graph.getNode(neighborNode).isPresent()) {
                    // We don't know about a neighbouring point (e.g. near the edge of checkout radius)
                    continue;
                }

                if (closedSet.contains(neighborNode)) {
                    // Ignore the neighbor which is already evaluated.
                    continue;
                }

                // The distance from start to a neighbor
                double tentativeGScore = gScore.get(current) + neighborConnection.getWeight();

                boolean openSetContainsNeighbor = !closedSet.contains(neighborNode) && gScore.containsKey(neighborNode);

                if (!openSetContainsNeighbor || tentativeGScore < gScore.get(neighborNode)) {

                    setCameFrom(cameFrom, neighborNode, current, neighborConnection.getId());
                    gScore.put(neighborNode, tentativeGScore);
                    fScore.put(neighborNode, tentativeGScore + heuristicCostEstimate(graph, neighborNode, endingNodeId));

                    openQueue.add(neighborNode);
                }

            }

        }

        // We have considered all routes we know about, and no path exists with these nodes in the graph.
        // It's possible that a path does exist, but extends beyond what this worker knows about.
        // We can try to send the route to the closest point we know we can actually reach instead.
        long closestReachablePoint = Collections.min(gScore.keySet(), new Comparator<Long>() {
            @Override
            public int compare(Long o1, Long o2) {
                double d1 = heuristicCostEstimate(graph, o1, endingNodeId);
                double d2 = heuristicCostEstimate(graph, o2, endingNodeId);
                if (d1 > d2) {
                    return 1;
                } else if (d1 == d2) {
                    // break ties based on gscore
                    if (gScore.get(o1) > gScore.get(o2)) {
                        return 1;
                    }
                }
                return 0;
            }
        });

        return reconstructPath(cameFrom, closestReachablePoint);
    }

    private double heuristicCostEstimate(Graph graph, long from, long to) {
        // if our graph doesn't know about one of the entities we're asking for, then we assume that the cost is
        // very large. (Note: this is could happen with concurrent modification of the graph removing an entry)
        try {
            Coordinates fromCoordinates = graph.getNode(from).get().getPosition();
            Coordinates toCoordinates = graph.getNode(to).get().getPosition();
            return CoordinateExtensions.distance(fromCoordinates, toCoordinates);
        } catch (NoSuchElementException e) {
            return Double.MAX_VALUE;
        }
    }

    private List<PathStep> reconstructPath(Map<Long, PathStep> cameFrom, long currentNode) {
        List<PathStep> totalPath = new LinkedList<>();

        PathStep currentNodeCameFrom = cameFrom.get(currentNode);
        totalPath.add(0, currentNodeCameFrom);
        currentNode = currentNodeCameFrom.getStartNodeId();

        while (cameFrom.containsKey(currentNode)) {
            currentNodeCameFrom = cameFrom.get(currentNode);
            totalPath.add(0, currentNodeCameFrom);
            currentNode = currentNodeCameFrom.getStartNodeId();
        }

        return totalPath;
    }

    private void setCameFrom(Map<Long, PathStep> cameFrom, long nowAtNodeId, long cameFromNodeId, long cameViaEdgeId) {
        PathStep pathStep = cameFrom.get(nowAtNodeId);
        if (pathStep != null) {
            pathStep.setStartNodeId(cameFromNodeId);
            pathStep.setEndNodeId(nowAtNodeId);
            pathStep.setEdgeId(cameViaEdgeId);
        } else {
            cameFrom.put(nowAtNodeId, new PathStep(cameFromNodeId, nowAtNodeId, cameViaEdgeId));
        }
    }
}
