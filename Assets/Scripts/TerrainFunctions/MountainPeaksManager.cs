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
        mountainPeaks = new MountainPeaksCoordinates(100, gridManager);
        this.gridManager = gridManager;
    }

    /// <summary>
    /// generates new peaks in given neighbourhood
    /// </summary>
    public void GeneratePeaks(int x, int z)
    {
        MountainPeaks peaks = new MountainPeaks(x, z, 
            gridManager.GetPointAreaOnGrid(gridManager.GetRealCoordinates(new Vector3(x, 0, z))));
        peaks.GeneratePeaks();

        UpdateNeighbourhood(x, z, peaks.peaks);
        

        //Debug.Log("generated: " + peaks);
    }

    /// <summary>
    /// when new peak is added, neighbouring peaks have to be updated 
    /// new peak might have influence on neighbouring areas
    /// </summary>
    public void UpdateNeighbourhood(int _x, int _z, List<Vertex> peaks)
    {
        for(int x = _x - 1; x <= _x+1; x++)
        {
            for (int z = _z - 1; z <= _z + 1; z++)
            {
                mountainPeaks.UpdateValue(x, z, peaks);
            }
        }
    }

    /// <summary>
    /// returns closest peak from neigbouring areas
    /// TODO: make constant 
    /// </summary>
    public List<Vertex> GetClosestPeaks(Vector3 center)
    {
        Vertex centerCoordOnGrid = gridManager.GetCoordinatesOnGrid(center); 
        List<Vertex> closestPeaks = new List<Vertex>();
        int x = centerCoordOnGrid.x;
        int z = centerCoordOnGrid.z;

        GeneratePeaks(x, z);
        closestPeaks.AddRange(mountainPeaks.GetValue(x, z).peaks);

        /*if (!mountainPeaks.IsDefined(x - 1, z - 1))
            GeneratePeaks(x - 1, z - 1);
        closestPeaks.AddRange(mountainPeaks.GetValue(x - 1, z - 1).peaks);
        if (!mountainPeaks.IsDefined(x, z - 1))
            GeneratePeaks(x, z - 1);
        closestPeaks.AddRange(mountainPeaks.GetValue(x, z - 1).peaks);
        if (!mountainPeaks.IsDefined(x + 1, z - 1))
            GeneratePeaks(x + 1, z - 1);
        closestPeaks.AddRange(mountainPeaks.GetValue(x + 1, z - 1).peaks);


        if (!mountainPeaks.IsDefined(x - 1, z))
            GeneratePeaks(x - 1, z);
        closestPeaks.AddRange(mountainPeaks.GetValue(x - 1, z).peaks);
        if (!mountainPeaks.IsDefined(x, z))
            GeneratePeaks(x, z);
        closestPeaks.AddRange(mountainPeaks.GetValue(x, z).peaks);
        if (!mountainPeaks.IsDefined(x + 1, z))
            GeneratePeaks(x + 1, z);
        closestPeaks.AddRange(mountainPeaks.GetValue(x + 1, z).peaks);


        if (!mountainPeaks.IsDefined(x - 1, z + 1))
            GeneratePeaks(x - 1, z + 1);
        closestPeaks.AddRange(mountainPeaks.GetValue(x - 1, z + 1).peaks);
        if (!mountainPeaks.IsDefined(x, z + 1))
            GeneratePeaks(x, z + 1);
        closestPeaks.AddRange(mountainPeaks.GetValue(x, z + 1).peaks);
        if (!mountainPeaks.IsDefined(x + 1, z + 1))
            GeneratePeaks(x + 1, z + 1);
        closestPeaks.AddRange(mountainPeaks.GetValue(x + 1, z + 1).peaks);
        */
        return closestPeaks;
    }
}
