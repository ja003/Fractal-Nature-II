using UnityEngine;
using System.Collections;

public class GridManager {

    Vector3 globalCenter;
    int stepX;
    int stepZ;

    //TODO: delete stepZ? DS has to have squared area
    public GridManager(Vector3 globalCenter,int stepX, int stepZ)
    {
        this.globalCenter = globalCenter;
        this.stepX = stepX;
        this.stepZ = stepZ;
    }

    /// <summary>
    /// returns projection of point on the grid defined by center, width and height
    /// </summary>
    public Vector3 GetPointOnGrid(Vector3 point)
    {
        int xVal = (int)(point.x / (stepX / 2));
        int zVal = (int)(point.z / (stepZ / 2));

        Vector3 closestCenter = new Vector3(666, 0, 666);
        for(int xi = -1; xi <= 1; xi++)
        {
            for (int zi = -1; zi <= 1; zi++)
            {
                Vector3 centerOnGrid = 
                    new Vector3(
                        globalCenter.x + (xVal + xi) * stepX/2,
                        0,
                        globalCenter.z + (zVal + zi) * stepZ/2);

                //Debug.Log(centerOnGrid);
                if (Vector3.Distance(point, centerOnGrid) <
                    Vector3.Distance(point, closestCenter))
                {
                    closestCenter = centerOnGrid;
                }
            }
        }

        return closestCenter;
    }

    /// <summary>
    /// returns coordinates of closest grid node to the point
    /// in vector form (x,0,z)
    /// </summary>
    public Vertex GetCoordinatesOnGrid(Vector3 point)
    {
        int x = (int)(point.x / stepX);
        int z = (int)(point.z / stepZ);
        return new Vertex(x, z);
    }

    /// <summary>
    /// maps grid point to real coordinates
    /// </summary>
    public Vector3 GetRealCoordinates(Vector3 gridPoint)
    {
        return new Vector3(gridPoint.x * stepX, 0, gridPoint.z * stepZ);
    }

    public Area GetPointArea(int gridX, int gridZ)
    {
        return GetPointAreaOnGrid(GetRealCoordinates(new Vector3(gridX, 0, gridZ)));
    }

    /// <summary>
    /// returns area of point projected on grid
    /// </summary>
    public Area GetPointAreaOnGrid(Vector3 point)
    {
        Vector3 pointOnGrid = GetPointOnGrid(point);
        Vector3 botLeft = new Vector3(pointOnGrid.x - stepX/2, 0, pointOnGrid.z - stepZ / 2);
        Vector3 topRight = new Vector3(pointOnGrid.x + stepX/2, 0, pointOnGrid.z + stepZ / 2);

        return new Area(botLeft, topRight);
    }
}
