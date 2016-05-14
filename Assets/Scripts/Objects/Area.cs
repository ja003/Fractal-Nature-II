using UnityEngine;
using System.Collections;

public class Area {

    public Vertex botLeft;
    public Vertex topRight;

    public Vertex topLeft;
    public Vertex botRight;


    public Area(Vertex botLeft, Vertex topRight)
    {
        this.botLeft = botLeft;
        this.topRight = topRight;

        topLeft = new Vertex(botLeft.x, topRight.z);
        botRight = new Vertex(topRight.x, botLeft.z);
    }

    /// <summary>
    /// calculates whether Area contains given point (or lies on border)
    /// </summary>
    public bool Contains(Vertex point)
    {
        return botLeft.x <= point.x && botLeft.z <= point.z &&
            point.x <= topRight.x && point.z <= topRight.z;
    }

    /// <summary>
    /// returns min of width and height
    /// </summary>
    public int GetSize()
    {
        return Mathf.Min(topRight.x - botLeft.x, topRight.z - botLeft.z);
    }
    
    public override string ToString()
    {
        return "[" + botLeft.x + "," + botLeft.z + "],[" + topRight.x + "," + topRight.z + "]";
    }
}
