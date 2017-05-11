package improbable.enterprise.routing.graphs;

import improbable.enterprise.routing.GraphEdge;
import improbable.enterprise.routing.GraphNode;
import improbable.math.Coordinates;

import java.util.List;

public class SimpleGraphNode implements GraphNode {

    private long id;
    private Coordinates position;
    private List<GraphEdge> edges;

    public SimpleGraphNode(long id, Coordinates position, List<GraphEdge> edges) {
        this.id = id;
        this.position = position;
        this.edges = edges;
    }

    @Override
    public long getId() {
        return id;
    }

    @Override
    public Coordinates getPosition() {
        return position;
    }

    @Override
    public List<GraphEdge> getEdges() {
        return edges;
    }
}
