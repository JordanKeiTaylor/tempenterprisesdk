package improbable.enterprise.routing;

import improbable.enterprise.routing.graphs.SimpleGraphFromString;
import org.junit.Before;
import org.junit.Test;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

import static junit.framework.TestCase.assertEquals;
import static org.hamcrest.MatcherAssert.assertThat;
import static org.hamcrest.core.Is.is;
import static org.junit.Assert.assertTrue;

public class AStartRouterTest {

    private AStarRouter router;

    @Before
    public void setup() {
        router = new AStarRouter();
    }

    @Test
    public void GIVEN_diamond_graph_THEN_find_route_to_end() {
        SimpleGraphFromString testGraph = new SimpleGraphFromString(new String[]{
                "_ S _",
                "X X _",
                "_ E _"
        });

        List<PathStep> route = router.findRoute(
                testGraph,
                testGraph.startNodeId,
                testGraph.endNodeId
        );

        // last id should be end nav point id
        assertEquals(route.get(route.size() - 1).getEndNodeId(), testGraph.endNodeId);
    }


    @Test
    public void GIVEN_diamond_graph_THEN_find_shortest_route_to_end() {

        SimpleGraphFromString testGraph = new SimpleGraphFromString(new String[]{
                "_ S _",
                "X X _",
                "_ E _"
        });

        List<PathStep> route = router.findRoute(
                testGraph,
                testGraph.startNodeId,
                testGraph.endNodeId
        );

        List<Long> shortestPath = new ArrayList<>();
        shortestPath.add(testGraph.nodePositions[1][1]);
        shortestPath.add(testGraph.endNodeId);

        List<Long> routeNavIds = route.stream()
                .map(PathStep::getEndNodeId)
                .collect(Collectors.toList());

        // last id should be end nav point id
        assertEquals(routeNavIds, shortestPath);
        assertTrue(isPathValid(testGraph, route));
    }

    @Test
    public void GIVEN_complex_graph_THEN_find_route_to_end() {
        SimpleGraphFromString testGraph = new SimpleGraphFromString(new String[]{
                "S X _ _ _ _ _",
                "X X _ X X _ _",
                "_ X X _ _ _ _",
                "X _ _ X _ X E",
                "X _ _ _ _ X _",
                "X X X X X _ _",
        });

        List<PathStep> route = router.findRoute(
                testGraph,
                testGraph.startNodeId,
                testGraph.endNodeId
        );

        assertThat(route.size(), is(10));
        assertEquals(route.get(route.size() - 1).getEndNodeId(), testGraph.endNodeId);
        assertTrue(isPathValid(testGraph, route));
    }

    @Test
    public void GIVEN_complex_graph_THEN_find_shortest_route_to_end() {
        SimpleGraphFromString testGraph = new SimpleGraphFromString(new String[]{
                "S X _ _ _ _ _",
                "X X _ X X _ _",
                "_ X X _ _ _ _",
                "X _ _ X X X E",
                "X _ _ _ _ X _",
                "X X X X X _ _",
        });

        List<PathStep> route = router.findRoute(
                testGraph,
                testGraph.startNodeId,
                testGraph.endNodeId
        );

        assertThat(route.size(), is(6));
        assertEquals(route.get(route.size() - 1).getEndNodeId(), testGraph.endNodeId);
        assertTrue(isPathValid(testGraph, route));
    }

    @Test
    public void GIVEN_disconnected_graph_THEN_route_to_a_nearby_point() {
        SimpleGraphFromString testGraph = new SimpleGraphFromString(new String[]{
                "S X X X X X X",
                "X X X X X X X",
                "X X X X X _ _",
                "X X X X X _ E",
                "X X X X X _ _",
                "X X X X X X X",
        });

        List<PathStep> route = router.findRoute(
                testGraph,
                testGraph.startNodeId,
                testGraph.endNodeId
        );


        System.out.println(route);
        assertEquals(route.size(), 4);
    }

    private boolean isPathValid(Graph graph, List<PathStep> pathSteps) {

        return pathSteps.stream().allMatch(pathStep -> {
            Optional<Long> endNavPointId = getConnectedEntity(graph, pathStep);
            return endNavPointId.isPresent() && endNavPointId.get().equals(pathStep.getEndNodeId());
        });
    }

    private Optional<Long> getConnectedEntity(Graph graph, PathStep pathStep) {
        return graph.getNode(pathStep.getStartNodeId()).get().getEdges().stream()
                .filter(connection -> connection.getId() == pathStep.getEdgeId())
                .map(GraphEdge::getNodeId)
                .findFirst();
    }

}
