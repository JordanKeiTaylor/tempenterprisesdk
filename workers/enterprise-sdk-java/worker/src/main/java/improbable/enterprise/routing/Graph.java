package improbable.enterprise.routing;

import java.util.Optional;

public interface Graph {

    Optional<GraphNode> getNode(Long nodeId);

}
