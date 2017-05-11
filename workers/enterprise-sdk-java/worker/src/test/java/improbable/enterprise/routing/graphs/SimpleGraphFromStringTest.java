package improbable.enterprise.routing.graphs;

import improbable.enterprise.routing.GraphEdge;
import org.junit.Test;

import java.util.Set;
import java.util.stream.Collectors;

import static junit.framework.TestCase.assertEquals;
import static org.junit.Assert.assertTrue;

public class SimpleGraphFromStringTest {

    @Test
    public void GIVEN_graph_as_string_THEN_a_connected_graph_is_constructed() {
        SimpleGraphFromString testGraph = new SimpleGraphFromString(new String[]{
                "S _ X",
                "_ X _",
                "X _ E",
        });

        assertTrue(testGraph.nodePositions[0][0] == testGraph.startNodeId);
        assertTrue(testGraph.nodePositions[2][2] ==  testGraph.endNodeId);
        assertEquals(5, testGraph.size());

        Set<Long> connectedToStart = testGraph.getNode(testGraph.startNodeId).get().getEdges().stream()
                .map(GraphEdge::getNodeId).collect(Collectors.toSet());

        assertEquals(1, connectedToStart.size());
        assertTrue(connectedToStart.contains(testGraph.nodePositions[1][1]));

        Set<Long> connectedToMiddle = testGraph.getNode(testGraph.nodePositions[1][1]).get().getEdges().stream()
                .map(GraphEdge::getNodeId).collect(Collectors.toSet());

        assertEquals(4, connectedToMiddle.size());
        assertTrue(connectedToMiddle.contains(testGraph.endNodeId));
    }


}
