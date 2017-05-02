package improbable.enterprise.extensions;


import improbable.math.Coordinates;

public class CoordinateExtensions {

    public static double distance(Coordinates fromCoordinates, Coordinates toCoordinates) {
        double dx = fromCoordinates.getX() - toCoordinates.getX();
        double dy = fromCoordinates.getY() - toCoordinates.getY();
        double dz = fromCoordinates.getZ() - toCoordinates.getZ();
        return Math.sqrt((dx * dx) + (dy * dy) + (dz * dz));
    }

}