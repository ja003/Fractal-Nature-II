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
}
