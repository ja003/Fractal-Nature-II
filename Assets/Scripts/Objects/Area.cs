using UnityEngine;
using System.Collections;

public class Area {

    public Vector3 botLeft;
    public Vector3 topRight;

    public Vector3 topLeft;
    public Vector3 botRight;


    public Area(Vector3 botLeft,Vector3 topRight)
    {
        this.botLeft = botLeft;
        this.topRight = topRight;

        topLeft = new Vector3(botLeft.x, 0, topRight.z);
        botRight = new Vector3(topRight.x, 0, botLeft.z);
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
