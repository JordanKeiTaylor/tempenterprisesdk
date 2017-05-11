package improbable.enterprise.routing;

public class PathStep {
    private long startNodeId;
    private long endNodeId;
    private long edgeId;

    public PathStep(long startNodeId, long endNodeId, long edgeId) {
        this.startNodeId = startNodeId;
        this.endNodeId = endNodeId;
        this.edgeId = edgeId;
    }

    public long getStartNodeId() {
        return startNodeId;
    }

    public void setStartNodeId(long startNodeId) {
        this.startNodeId = startNodeId;
    }

    public long getEndNodeId() {
        return endNodeId;
    }

    public void setEndNodeId(long endNodeId) {
        this.endNodeId = endNodeId;
    }

    public long getEdgeId() {
        return edgeId;
    }

    public void setEdgeId(long edgeId) {
        this.edgeId = edgeId;
    }
}
