package improbable.enterprise.routing.graphs;

import improbable.enterprise.extensions.CoordinateExtensions;
import improbable.enterprise.routing.Graph;
import improbable.enterprise.routing.GraphEdge;
import improbable.enterprise.routing.GraphNode;
import improbable.math.Coordinates;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.Optional;

public class SimpleGraphFromString implements Graph {

    private long nextNodeId = 1000;
    public Long[][] nodePositions;
    public long startNodeId;
    public long endNodeId;

    private Map<Long, GraphNode> nodes = new HashMap<>();

    public SimpleGraphFromString(String[] graphAsString) {
        buildFromStrings(graphAsString);
    }

    private void buildFromStrings(String[] graph) {

        nodePositions = new Long[graph.length][];

        for (int y = 0; y < graph.length; y++) {

            String[] steps = graph[y].split(" ");

            nodePositions[y] = new Long[steps.length];

            for (int x = 0; x < steps.length; x++) {

                if (!steps[x].equals("_")) {
                    long entityId = placeEntity(x, y);
                    if (steps[x].equals("S")) {
                        startNodeId = entityId;
                    }

                    if (steps[x].equals("E")) {
                        endNodeId = entityId;
                    }
                }

            }
        }

        connectGraph();
    }

    private long nextEntityId() {
        return ++nextNodeId;
    }

    private long placeEntity(int x, int y) {
        long entityId = nextEntityId();

        this.nodes.put(entityId, new SimpleGraphNode(entityId, new Coordinates(x, 0, y), new ArrayList<>()));

        nodePositions[y][x] = entityId;
        return entityId;
    }

    private void connectGraph() {
        for (int y = 0; y < nodePositions.length; y++) {
            for (int x = 0; x < nodePositions[y].length; x++) {
                connectToAdjacent(x, y);
            }
        }
    }

    private void connectToAdjacent(int x1, int y1) {
        for (int yOffset = -1; yOffset <= 1; yOffset++) {
            for (int xOffset = -1; xOffset <= 1; xOffset++) {

                int x2 = x1 + xOffset;
                int y2 = y1 + yOffset;

                if (hasEntity(x1, y1) && hasEntity(x2, y2)) {
                    connectEntities(x1, y1, x2, y2);
                }
            }
        }
    }

    public void connectEntities(int x1, int y1, int x2, int y2) {

        long from = nodePositions[y1][x1];
        long to = nodePositions[y2][x2];

        if (from == to) {
            return;
        }

        GraphNode fromEntry = getNode(from).get();
        GraphNode toEntry = getNode(to).get();

        float distance = (float) CoordinateExtensions.distance(fromEntry.getPosition(), toEntry.getPosition());

        GraphEdge fromToConnection = new SimpleGraphEdge(to, distance, nextEntityId());

        if (!fromEntry.getEdges().contains(fromToConnection)) {
            fromEntry.getEdges().add(fromToConnection);
        }

        GraphEdge toFromConnection = new SimpleGraphEdge(from, distance, nextEntityId());

        if (!toEntry.getEdges().contains(toFromConnection)) {
            toEntry.getEdges().add(toFromConnection);
        }
    }

    private boolean hasEntity(int x, int y) {
        return x >= 0
                && y >= 0
                && y < nodePositions.length
                && x < nodePositions[y].length
                && nodePositions[y][x] != null;
    }

    @Override
    public Optional<GraphNode> getNode(Long nodeId) {
        return Optional.ofNullable(this.nodes.get(nodeId));
    }

    public long size(){
        return this.nodes.size();
    }
}

