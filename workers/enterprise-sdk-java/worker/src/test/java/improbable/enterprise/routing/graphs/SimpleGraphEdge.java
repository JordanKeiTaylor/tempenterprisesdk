package improbable.enterprise.routing.graphs;

import improbable.enterprise.routing.GraphEdge;

public class SimpleGraphEdge implements GraphEdge {

    private long nodeId;
    private double weight;
    private long id;

    public SimpleGraphEdge(long nodeId, double weight, long id) {
        this.nodeId = nodeId;
        this.weight = weight;
        this.id = id;
    }

    @Override
    public long getId() {
        return id;
    }

    @Override
    public long getNodeId() {
        return nodeId;
    }

    @Override
    public double getWeight() {
        return weight;
    }
}
