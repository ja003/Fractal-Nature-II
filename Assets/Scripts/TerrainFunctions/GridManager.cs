using UnityEngine;
using System.Collections;

public class GridManager {

    public Vertex globalCenter;
    public int stepX;
    public int stepZ;

    //TODO: delete stepZ? DS has to have squared area
    public GridManager(Vertex globalCenter,int stepX, int stepZ)
    {
        this.globalCenter = globalCenter;
        this.stepX = stepX;
        this.stepZ = stepZ;
    }

    /// <summary>
    /// returns projection of point on the grid defined by center, width and height
    /// </summary>
    public Vertex GetPointOnGrid(Vertex point)
    {
        int xVal = point.x / stepX;
        int zVal = point.z / stepZ;

        Vertex closestCenter = new Vertex(666, 666);
        for(int xi = -1; xi <= 1; xi++)
        {
            for (int zi = -1; zi <= 1; zi++)
            {
                Vertex centerOnGrid = 
                    new Vertex(
                        globalCenter.x + (xVal + xi) * stepX,
                        globalCenter.z + (zVal + zi) * stepZ);

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
    /// </summary>
    public Vertex GetGridCoordinates(Vertex point)
    {
        int x = Mathf.RoundToInt((float)point.x / stepX);
        int z = Mathf.RoundToInt((float)point.z / stepZ);
        return new Vertex(x, z);
    }

    public void UpdateSteps(int stepX, int stepZ)
    {
        this.stepX = stepX;
        this.stepZ = stepZ;
    }

    /// <summary>
    /// maps grid point to real coordinates
    /// </summary>
    public Vertex GetRealCoordinates(Vertex gridPoint)
    {
        return new Vertex(gridPoint.x * stepX, gridPoint.z * stepZ);
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
