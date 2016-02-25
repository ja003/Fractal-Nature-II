using UnityEngine;
using System.Collections;

public class LocalCoordinates {

    public Vector3 center;
    public Vector3 botLeft;
    public Vector3 topRight;

    public int terrainWidth;
    public int terrainHeight;
    //public GlobalCoordinates gc;

    public LocalCoordinates(Vector3 center, int terrainWidth, int terrainHeight)
    {
        //gc = globalCoordinates;
        this.center = center;
        botLeft = new Vector3(center.x - terrainWidth / 2, 0, center.z - terrainHeight / 2);
        topRight = new Vector3(center.x + terrainWidth / 2, 0, center.z + terrainHeight / 2);

        this.terrainWidth = terrainWidth;
        this.terrainHeight = terrainHeight;

    }

    public bool IsDefined(int x, int z, GlobalCoordinates gc)
    {
        if (GetLocalValue(x, z, gc) == 666)
            return false;
        else
            return true;
    }

    /// <summary>
    /// maps given local coordinates to global
    /// returns value on given coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public float GetLocalValue(int x, int z, GlobalCoordinates gc)
    {
        //Debug.Log("getting " + x + "," + z);
        //Debug.Log("= " + (x + (int)center.x - terrainWidth) + "," + (z + (int)center.z - terrainHeight / 2));
        return gc.GetValue(x + (int)center.x - terrainWidth / 2, z + (int)center.z - terrainHeight / 2);
    }
    /// <summary>
    /// sets height to global terrain
    /// maps given local coordinates on global
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void SetLocalValue(int x, int z, float height, bool overwrite, GlobalCoordinates gc)
    {
        if (!overwrite && IsDefined(x, z, gc))
            return;
        gc.SetValue(x + (int)center.x - terrainWidth / 2, z + (int)center.z - terrainHeight / 2, height, overwrite);
    }

}
