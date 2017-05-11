package improbable.enterprise.routing;

import improbable.math.Coordinates;

import java.util.List;

public interface GraphNode {

    long getId();

    Coordinates getPosition();

    List<GraphEdge> getEdges();
}
