using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MountainPeaksManager {

    public MountainPeaksCoordinates mountainPeaks;
    public GridManager gridManager;

    /// <summary>
    /// class manages all mountain peaks
    /// </summary>
    public MountainPeaksManager(GridManager gridManager)
    {
        this.gridManager = gridManager;
        mountainPeaks = new MountainPeaksCoordinates(20, gridManager, this);
        mountainPeaks.InitializePeaks(20);
        //Debug.Log(mountainPeaks);
    }
    

    /// <summary>
    /// when new peak is added, neighbouring peaks have to be updated 
    /// new peak might have influence on neighbouring areas
    /// </summary>
    public void UpdateNeighbourhood(int _x, int _z, List<Vertex> peaks)
    {
        //Debug.Log("!");

        ////TODO: TOO COMPLEX!!!
        
        for(int x = _x - 1; x <= _x+1; x++)
        {
            for (int z = _z - 1; z <= _z + 1; z++)
            {
                mountainPeaks.GetValue(x, z).AddPeaks(peaks);
            }
        }
    }

    /// <summary>
    /// returns closest peak from neigbouring areas
    /// </summary>
    public List<Vertex> GetClosestPeaks(Vector3 center)
    {
        Vertex centerCoordOnGrid = gridManager.GetGridCoordinates(center); 
        List<Vertex> closestPeaks = new List<Vertex>();
        int x = centerCoordOnGrid.x;
        int z = centerCoordOnGrid.z;
       
        closestPeaks.AddRange(mountainPeaks.GetValue(x-1, z-1).peaks);
        closestPeaks.AddRange(mountainPeaks.GetValue(x-1, z).peaks);
        closestPeaks.AddRange(mountainPeaks.GetValue(x-1, z+1).peaks);
        closestPeaks.AddRange(mountainPeaks.GetValue(x, z-1).peaks);
        closestPeaks.AddRange(mountainPeaks.GetValue(x, z).peaks);
        closestPeaks.AddRange(mountainPeaks.GetValue(x, z+1).peaks);
        closestPeaks.AddRange(mountainPeaks.GetValue(x+1, z-1).peaks);
        closestPeaks.AddRange(mountainPeaks.GetValue(x+1, z).peaks);
        closestPeaks.AddRange(mountainPeaks.GetValue(x+1, z+1).peaks);
        if (closestPeaks.Count == 0)
        {
            Debug.Log("!");
            closestPeaks.Add(center);
        }
        return closestPeaks;
    }
}
