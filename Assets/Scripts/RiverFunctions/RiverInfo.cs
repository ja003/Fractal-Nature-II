using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiverInfo  {

    public List<Vertex> riverPath;

    public Vertex topVertex;
    public Vertex rightVertex;
    public Vertex botVertex;
    public Vertex leftVertex;

    public Vertex lowestPoint;
    
    public FunctionRiverPlanner frp;
    public FunctionDebugger fd;
    public FunctionTerrainManager ftm;
    public FunctionMathCalculator fmc;

    public List<Direction> reachedSides;

    public GlobalCoordinates globalRiverC;

    public int gridStep;
    public float threshold;

    public float width;
    public float areaEffect;//not used right now
    public float depth;
    public RiverShape shape;

    public string errorMessage;

    public RiverInfo(RiverGenerator rg)
    {
        riverPath = new List<Vertex>();
        //reachTop = false;
        //reachRight = false;
        //reachBot = false;
        //reachLeft = false;

        fd = rg.fd;
        frp = rg.frp;
        ftm = rg.ftm;
        fmc = rg.fmc;
        reachedSides = new List<Direction>();
        shape = RiverShape.atan;

        lowestPoint = new Vertex(666, 666, 666);

        globalRiverC = new GlobalCoordinates(100);
    }

    /// <summary>
    /// returns closest vertex from path to given coordinates
    /// </summary>
    public Vertex GetClosestVertexTo(Vertex point)
    {
        float distance = 666;
        Vertex closestVertex = riverPath[0];
        foreach(Vertex v in riverPath)
        {
            if(Vector3.Distance(v, point) < distance)
            {
                closestVertex = v;
            }
            distance = Vector3.Distance(v, point);
        }
        return closestVertex;
    }

    /// <summary>
    /// checks if point has lower height that river's lowest point
    /// </summary>
    public void UpdateLowestPoint(Vertex point)
    {
        if(lowestPoint.height > point.height)
        {
            lowestPoint = point;
        }
    }
    
    
    public Vertex GetLastVertex()
    {
        return riverPath[riverPath.Count - 1];
    }

    /// <summary>
    /// set default values for this river
    /// values are determined only by experience
    /// </summary>
    public void SetDefaultValues()
    {
        width = 15;
        areaEffect = 2;
        depth = 0.15f;
        gridStep = 30;
    }
    

    
    //connect 2 rivers into 1
    public void ConnectWith(RiverInfo river2)
    {
        if (riverPath[0].EqualsCoordinates(river2.riverPath[0]))
        {
            river2.riverPath.RemoveAt(0);
            riverPath.Reverse();
            riverPath.AddRange(river2.riverPath);
            //riverPath.Reverse();
        }
        else if (GetLastVertex().EqualsCoordinates(river2.riverPath[0]))
        {
            river2.riverPath.RemoveAt(0);
            riverPath.AddRange(river2.riverPath);
        }
        else if (riverPath[0].EqualsCoordinates(river2.GetLastVertex()))
        {
            riverPath.RemoveAt(0);
            riverPath.Reverse();
            river2.riverPath.Reverse();
            riverPath.AddRange(river2.riverPath);
            riverPath.Reverse();
        }
        else if (GetLastVertex().EqualsCoordinates(river2.GetLastVertex()))
        {
            river2.riverPath.RemoveAt(river2.riverPath.Count - 1);
            river2.riverPath.Reverse();
            riverPath.AddRange(river2.riverPath);
        }
        else
        {
            Debug.Log("RIVERS DONT HAVE COMMON END POINT");
        }
        

        //reachTop = reachTop || river2.reachTop;
        //reachRight = reachRight || river2.reachRight;
        //reachBot = reachBot || river2.reachBot;
        //reachLeft = reachLeft || river2.reachLeft;
    }
   
    public void ResetRiver()
    {
        riverPath = new List<Vertex>();
    }

    public override string ToString()
    {
        string info = "";
        //info += "reachTop: " + reachTop + "\n";
        //info += "reachRight: " + reachRight + "\n";
        //info += "reachBot: " + reachBot + "\n";
        //info += "reachLeft: " + reachLeft + "\n";
        foreach(Vertex v in riverPath)
        {
            info += riverPath.IndexOf(v)+": " + v + "\n";
        }
        info += "topVertex:" + topVertex+"\n";
        info += "rightVertex:" + rightVertex + "\n";
        info += "botVertex:" + botVertex+"\n";
        info += "leftVertex:" + leftVertex+"\n";

        return info;
    }

    public void DrawRiver()
    {
        foreach(Vertex v in riverPath)
        {
            //int localX = (int)frp.rg.localCoordinates.GetLocalCoordinates(v).x;
            //int localZ = (int)frp.rg.localCoordinates.GetLocalCoordinates(v).z;
            //fd.ColorPixel(localX, localZ, 3, fd.pinkColor);
            fd.ColorPixel(v.x, v.z, 3, fd.pinkColor);
        }
        for(int i = 0; i < riverPath.Count-1; i++)
        {
            //fd.ColorLine(riverPath[i], riverPath[i + 1], 2, fd.redColor);
            Vector3 dir = ((Vector3)riverPath[i + 1] - riverPath[i]).normalized;
            float dist = ftm.fmc.GetDistance(riverPath[i + 1], riverPath[i]);
            for(int j = 0; j < dist; j++)
            {
                fd.ColorPixel(riverPath[i].x + (int)(dir.x * j), riverPath[i].z + (int)(dir.z * j), 2, fd.redColor);
            }

        }

    }

    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        RiverInfo r = obj as RiverInfo;
        if ((System.Object)r == null)
        {
            return false;
        }
        
        for(int i = 0; i < riverPath.Count;i++)
        {
            if (riverPath[i] != r.riverPath[i])
                return false;
        }
        return true;
    }
}
