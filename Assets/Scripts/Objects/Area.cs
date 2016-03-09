using UnityEngine;
using System.Collections;

public class Area {

    public Vector3 botLeft;
    public Vector3 topRight;

    public Area(Vector3 botLeft,Vector3 topRight)
    {
        this.botLeft = botLeft;
        this.topRight = topRight;
    }

    /// <summary>
    /// calculates whether Area contains given point (or lies on border)
    /// </summary>
    public bool Contains(Vector3 point)
    {
        return botLeft.x <= point.x && botLeft.z <= point.z &&
            point.x <= topRight.x && point.z <= topRight.z;
    }

    public override string ToString()
    {
        return "[" + botLeft.x + "," + botLeft.z + "],[" + topRight.x + "," + topRight.z + "]";
    }
}
