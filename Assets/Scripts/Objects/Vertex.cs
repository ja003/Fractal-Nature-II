using UnityEngine;
using System.Collections;

public class Vertex
{
    public int x { get; set; }
    public int z { get; set; }
    public float height { get; set; }
    public Direction side { get; set; } //stores information on which side is vertex located

    public Vertex(int x, int z, float height)
    {
        this.x = x;
        this.z = z;
        this.height = height;
        side = Direction.none;
    }
    public Vertex(int x, int z)
    {
        this.x = x;
        this.z = z;
        this.height = 0;
        side = Direction.none;
    }
    public void Rewrite(int x, int z, float height)
    {
        this.x = x;
        this.z = z;
        this.height = height;
    }
    
    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        Vertex v = obj as Vertex;
        if ((System.Object)v == null)
        {
            return false;
        }

        // Return true if the fields match:
        return x == v.x && z == v.z;// && height == v.height; //!!! TODO: is ot ok?
    }

    /// <summary>
    /// ignores height
    /// </summary>
    public bool EqualsCoordinates(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        Vertex v = obj as Vertex;
        if ((System.Object)v == null)
        {
            return false;
        }

        // Return true if the fields match:
        return x == v.x && z == v.z;
    }

    public bool CoordinatesEquals(Vertex v2)
    {
        return x == v2.x && z == v2.z;
    }

    public bool CoordinatesEquals(Vertex v2, int e)
    {
        return x - e <= v2.x && v2.x <= x + e && z - e <= v2.z && v2.z <= z + e;
    }

    /* not sure if good/necessary
    public override int GetHashCode()
    {
        return x * 98411 + z * 98507;
    }*/

    public Vertex Clone()
    {
        return new Vertex(x, z, height);
    }

    // User-defined conversion from Digit to double
    public static implicit operator Vector3 (Vertex vertex)
    {
        return new Vector3(vertex.x, 0, vertex.z);
    }
    //  User-defined conversion from double to Digit
    public static implicit operator Vertex(Vector3 vector)
    {
        return new Vertex((int)vector.x, (int)vector.z);
    }

    public override string ToString()
    {
        return "[" + x + "," + z + "]=" + height + " (" + side + ")";
    }
}