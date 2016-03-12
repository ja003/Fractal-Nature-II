﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text;

public class ObjExporter
{

    public static string MeshToString(MeshFilter mf)
    {
        Mesh m = mf.mesh;
        //Material[] mats = mf.GetComponent<Renderer>().sharedMaterials;

        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append(mf.name).Append("\n");
        foreach (Vector3 v in m.vertices)
        {
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in m.normals)
        {
            sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in m.uv)
        {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }

        sb.Append("\n");
        int[] triangles = m.GetTriangles(0);
        for (int i = 0; i < triangles.Length; i += 3)
        {
            sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
        }

        return sb.ToString();
    }

    public static string TerrainToString(GlobalTerrain t)
    {
        StringBuilder sb = new StringBuilder();

        Vertex botLeft;
        Vertex topRight;

        int x_min = -10;
        int x_max = 80;
        int z_min = -20;
        int z_max = 70;

        for (int x = x_min; x< x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                sb.Append(string.Format("v {0} {1} {2}\n", x, t.GetHeight(x, z) * 15, z));
            }
        }

        sb.Append("\n");

        int w = x_max - x_min; //width
        int h = z_max - z_min; //height

        for (int x = 0; x < w - 1; x++)
        {
            for (int z = 0; z < h - 1; z++)
            {
                sb.Append(string.Format("f {0} {1} {2} {3}\n", x*h+z+1, x*h+z+2, x*h+h+z+2, x*h+h+z+1));
            }
        }

        /*
        for(int i = 1; i < 20*20 - 20; i++)
        {
            sb.Append(string.Format("f {0} {1} {2} {3}\n", i, i+1, i + 21, i + 20));
        }*/
        
        return sb.ToString();
    }

    public static void TerrainToFile(GlobalTerrain gt, string filename)
    {
        using (StreamWriter sw = new StreamWriter(filename))
        {
            sw.Write(TerrainToString(gt));
        }
    }

    public static void MeshToFile(MeshFilter mf, string filename)
    {
        using (StreamWriter sw = new StreamWriter(filename))
        {
            sw.Write(MeshToString(mf));
        }
    }
}
