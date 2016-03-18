using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MountainPeaks {

    MountainPeaksManager mpm;
    public Vertex gridCoordinates;
    public Area area;
    public List<Vertex> peaks;
    /*
    public MountainPeaks()
    {
        gridCoordinates = new Vertex(666,666);
        peaks = new List<Vertex>();
    }*/

    /// <summary>
    /// structure holds x peaks in certain area
    /// </summary>
    public MountainPeaks(int gridX, int gridZ, Area area, MountainPeaksManager mountainPeaksManager)
    {
        gridCoordinates = new Vertex(gridX, gridZ);
        this.area = area;
        peaks = new List<Vertex>();
        mpm = mountainPeaksManager;
        //GeneratePeaks();
    }

    /// <summary>
    /// add new peaks to current one
    /// checks if new peaks should be added (if they have higher influence than current ones)
    /// and if old ones shoukd be kept
    /// </summary>
    /// <param name="peaks"></param>
    public void AddPeaks(List<Vertex> peaks)
    {
        if(this.peaks.Count == 0 && peaks.Count > 0)
        {
            this.peaks.Add(peaks[0]);
        }
        if(peaks.Count == 0)
        {
            return;
        }

        foreach(Vertex p in peaks)
        {
            bool addNewPeak = true;
            for (int i = this.peaks.Count - 1; i >= 0; i--)
            {
                if (this.peaks.Contains(p))
                {
                    addNewPeak = false;
                    break;
                }

                int influence = CompareInfluence(p, this.peaks[i]);
                if(influence == 1)
                {
                    this.peaks.RemoveAt(i);
                }
                else if(influence == -1)
                {
                    addNewPeak = false;
                    break;
                }
            }
            if (addNewPeak)
            {
                this.peaks.Add(p);
            }
        }
    }

    /// <summary>
    /// compares 2 points and determines which one has bigger influence on current area
    /// 1 = newPoint, old is deleted, new is added
    /// 1 = newPoint, new is not added
    /// 0 = both has some influence, both are added
    /// </summary>
    public int CompareInfluence(Vertex newPoint, Vertex oldPoint)
    {
        float distBotLeftN = Vector3.Distance(newPoint, area.botLeft);
        float distTopLeftN = Vector3.Distance(newPoint, area.topLeft);
        float distTopRightN = Vector3.Distance(newPoint, area.topRight);
        float distBotRightN = Vector3.Distance(newPoint, area.botRight);

        float distBotLeftO = Vector3.Distance(oldPoint, area.botLeft);
        float distTopLeftO = Vector3.Distance(oldPoint, area.topLeft);
        float distTopRightO = Vector3.Distance(oldPoint, area.topRight);
        float distBotRightO = Vector3.Distance(oldPoint, area.botRight);

        int newPointInfluence = 0;
        int oldPointInfluence = 0;

        if (distBotLeftN < distBotLeftO)
            newPointInfluence++;
        else
            oldPointInfluence++;

        if (distTopLeftN < distTopLeftO)
            newPointInfluence++;
        else
            oldPointInfluence++;

        if (distTopRightN < distTopRightO)
            newPointInfluence++;
        else
            oldPointInfluence++;

        if (distBotRightN < distBotRightO)
            newPointInfluence++;
        else
            oldPointInfluence++;

        if (newPointInfluence == 4)
            return 1;
        else if (oldPointInfluence == 4)
            return -1;
        else
            return 0;

    }

    /// <summary>
    /// generates 1 peak on random position of area
    /// </summary>
    public void GeneratePeaks()
    {
        //int x = (int)(area.botLeft.x + area.topRight.x) / 2;
        //int z = (int)(area.botLeft.z + area.topRight.z) / 2;

        int offset = 10;
        int randX = Random.Range(area.botLeft.x + offset, area.topRight.x - offset);
        int randZ = Random.Range(area.botLeft.z + offset, area.topRight.z - offset);

        //randX = (int)(area.botLeft.x);
        //randZ = (int)(area.botLeft.z);

        List<Vertex> newPeaks = new List<Vertex>();
        newPeaks.Add(new Vertex(randX, randZ));
        AddPeaks(newPeaks);
        mpm.UpdateNeighbourhood(gridCoordinates.x, gridCoordinates.z, newPeaks);
    }

    public override string ToString()
    {
        string peak = "";
        if (peaks.Count > 0)
            peak = peaks[0].ToString();
        return gridCoordinates + "on: " + area + " | " + peak;
    }
}
