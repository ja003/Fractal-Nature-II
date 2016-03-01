using UnityEngine;
using System.Collections;

public class GridManager {

    Vector3 globalCenter;
    int terrainWidth;
    int terrainHeight;

    public GridManager(Vector3 globalCenter,int terrainWidth,int terrainHeight)
    {
        this.globalCenter = globalCenter;
        this.terrainWidth = terrainWidth;
        this.terrainHeight = terrainHeight;
    }

    /// <summary>
    /// returns projection of point on the grid defined by center, width and height
    /// </summary>
    public Vector3 GetCenterOnGrid(Vector3 point)
    {
        int xVal = (int)(point.x / (terrainWidth / 2));
        int zVal = (int)(point.z / (terrainHeight / 2));

        Vector3 closestCenter = new Vector3(666, 0, 666);
        for(int xi = -1; xi <= 1; xi++)
        {
            for (int zi = -1; zi <= 1; zi++)
            {
                Vector3 centerOnGrid = 
                    new Vector3(
                        globalCenter.x + (xVal + xi) * terrainWidth/2,
                        0,
                        globalCenter.z + (zVal + zi) * terrainHeight/2);

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

}
