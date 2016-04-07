﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;


public class GUIExport {

    int menuWidth;
    int rightMenuOffset;
    int topOffset;
    int buttonHeight;
    public float yPos;
    public float yPos2;
    int sideOffset;

    public float scaleY;

    bool exportMenuVisible;

    bool exportRiver;
    bool exportFilter;
    bool exportTerrain;

    string path;
    string name;

    GUIManager gm;
    TerrainGenerator tg;

    public GUIExport(GUIManager gm)
    {
        this.gm = gm;
        tg = gm.cm.terrainGenerator;

        menuWidth = gm.menuWidth;
        rightMenuOffset = gm.rightOffset;
        topOffset = gm.topOffset;
        buttonHeight = gm.menuButtonHeight;
        sideOffset = 10;
        scaleY = gm.scaleY;

        path = "C:\\Users\\Vukmir\\Dropbox\\ŠKOLA\\SBAPR\\Fractal Nature II\\Assets\\Export";
        path = "C:\\Users\\Vukmir\\Desktop\\obj";
        name = "";
    }

    public void OnGui(float yPosition)
    {
        float buttonWidth = menuWidth - 2* sideOffset;

        yPos = yPosition;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, buttonHeight + 4), " ");

        yPos += 2;

        if (GUI.Button(new Rect(Screen.width - menuWidth + 10, yPos, buttonWidth, buttonHeight), "EXPORT MESH"))
        {
            //Export();
            exportMenuVisible = !exportMenuVisible;
        }


        if (exportMenuVisible)
        {
            yPos2 = 0;
            float menu2Width = 2 * menuWidth;

            GUI.Box(new Rect(Screen.width - menu2Width, yPos2, menuWidth - rightMenuOffset, buttonHeight * 5), "Export");
            yPos2 += buttonHeight/2;

            //"C:\\Users\\Vukmir\\Dropbox\\ŠKOLA\\SBAPR\\Fractal Nature II\\Assets\\Export"
            GUI.TextField(new Rect(Screen.width - menu2Width + sideOffset, yPos2, buttonWidth, buttonHeight/2), path);
            yPos2 += buttonHeight/2 + 2;
            if (GUI.Button(new Rect(Screen.width - menu2Width + sideOffset, yPos2, buttonWidth, buttonHeight/2), "..."))
            {
                path = EditorUtility.SaveFolderPanel("Select destination","","");
            }
            yPos2 += buttonHeight / 2 + 2;
            GUI.Label(new Rect(Screen.width - menu2Width + sideOffset, yPos2, buttonWidth/2, buttonHeight/2),"name: ");
            name = GUI.TextArea(new Rect(Screen.width - menu2Width + buttonWidth/2, yPos2, buttonWidth/2, buttonHeight / 2), name);
            



            yPos2 += buttonHeight/2 + 2;
            exportTerrain = GUI.Toggle(new Rect(Screen.width - menu2Width + sideOffset, yPos2, menuWidth - rightMenuOffset, buttonHeight/2), gm.onFlyGeneration, "  terrain");
            yPos2 += buttonHeight / 2 + 2;

            exportRiver = GUI.Toggle(new Rect(Screen.width - menu2Width + sideOffset, yPos2, menuWidth - rightMenuOffset, buttonHeight / 2), gm.onFlyGeneration, "  river");
            yPos2 += buttonHeight / 2 + 2;

            exportFilter = GUI.Toggle(new Rect(Screen.width - menu2Width + sideOffset, yPos2, menuWidth - rightMenuOffset, buttonHeight / 2), gm.onFlyGeneration, "  filter");
            yPos2 += buttonHeight / 2 + 3;

            if (GUI.Button(new Rect(Screen.width - menu2Width + sideOffset, yPos2, buttonWidth, buttonHeight), "EXPORT"))
            {
                Export();
            }

        }
    }

    private void Export()
    {
        //gm.cm.objExporter.t
        //string s = ObjExporter.TerrainToString(gm.cm.localTerrain.globalTerrainC);
        //string s = ObjExporterMy.MeshToString(tg.myMesh[0]);

        //string fileName = Application.persistentDataPath + "/" + FILE_NAME; fileWriter = File.CreateText(fileName); fileWriter.WriteLine("Hello world"); fileWriter.Close();


        //ObjExporter.MeshToFile(tg.myTerrain[0].GetComponent<MeshFilter>(), "C:\\Users\\Vukmir\\Desktop\\obj\\myTerrain_0.obj");
        //ObjExporter.MeshToFile(tg.myTerrain[1].GetComponent<MeshFilter>(), "C:\\Users\\Vukmir\\Desktop\\obj\\myTerrain_1.obj");
        List<Layer> layers = new List<Layer>();
        if (tg.terrainLayer)
            layers.Add(Layer.terrain);
        if (tg.riverLayer)
            layers.Add(Layer.river);

        if (tg.filterAverageLayer)
            layers.Add(Layer.filterAverage);
        if (tg.filterMedianLayer)
            layers.Add(Layer.filterMedian);
        if (tg.filterSpikeLayer)
            layers.Add(Layer.filterSpike);
        if (tg.filterGaussianLayer)
            layers.Add(Layer.filterGaussian);
        if (tg.filterMinThresholdLayer)
            layers.Add(Layer.filterMinThreshold);
        if (tg.filterMaxThresholdLayer)
            layers.Add(Layer.filterMaxThreshold);


        if (tg.erosionHydraulicLayer)
            layers.Add(Layer.erosionHydraulic);

        Debug.Log("exporting");
        foreach(Layer l in layers)
        {
            Debug.Log(l);
        }

        if (name.Length == 0)
        {
            name = "default_name";
        }

        ObjExporter.TerrainToFile(gm.cm.layerManager, layers, path + "\\" + name + ".obj");
    }
}
