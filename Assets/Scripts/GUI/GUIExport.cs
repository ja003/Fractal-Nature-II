using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEngine;
#endif


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

        //path = "C:\\Users\\Vukmir\\Dropbox\\ŠKOLA\\SBAPR\\Fractal Nature II\\Assets\\Export";
        path = "C:";
        name = "";
    }

    public void OnGui(float yPosition)
    {
        float buttonWidth = menuWidth - 2* sideOffset;

        yPos = yPosition;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, buttonHeight + 6), " ");

        yPos += 3;

        if (GUI.Button(new Rect(Screen.width - menuWidth + 10, yPos, buttonWidth, buttonHeight), "EXPORT MESH"))
        {
            //Export();
            exportMenuVisible = !exportMenuVisible;
        }


        if (exportMenuVisible)
        {
            yPos2 = 5;
            float menu2Width = 2 * menuWidth;

            GUI.Box(new Rect(Screen.width - menu2Width, yPos2, menuWidth - rightMenuOffset, buttonHeight * 3.5f), "Export");
            yPos2 += buttonHeight/2;

            //"C:\\Users\\Vukmir\\Dropbox\\ŠKOLA\\SBAPR\\Fractal Nature II\\Assets\\Export"

            path = GUI.TextField(new Rect(Screen.width - menu2Width + sideOffset, yPos2, buttonWidth, buttonHeight/2), path);
            yPos2 += buttonHeight/2 + 2;
            if (GUI.Button(new Rect(Screen.width - menu2Width + sideOffset, yPos2, buttonWidth, buttonHeight/2), "..."))
            {
                path = "";
                // "C:\\Users\\Vukmir\\Desktop\\obj"; //personal default path
                path = UnityEditor.EditorUtility.SaveFolderPanel("Select destination","","");
                //  --- can't be used outside of UNITY
            }
            yPos2 += buttonHeight / 2 + 2;
            GUI.Label(new Rect(Screen.width - menu2Width + sideOffset, yPos2, buttonWidth/2, buttonHeight/2),"name: ");
            name = GUI.TextArea(new Rect(Screen.width - menu2Width + buttonWidth/2, yPos2, buttonWidth/2, buttonHeight / 2), name);
            
            
            yPos2 += buttonHeight / 2 + 3;
            
            if (GUI.Button(new Rect(Screen.width - menu2Width + sideOffset, yPos2, buttonWidth, buttonHeight), "EXPORT"))
            {
                Export();
            }

        }
    }

    private void Export()
    {
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


        //if (tg.waterLayer)
        //    layers.Add(Layer.water);

        if (tg.erosionHydraulicLayer)
            layers.Add(Layer.erosionHydraulic);
        if (tg.erosionThermalLayer)
            layers.Add(Layer.erosionThermal);




        Debug.Log("exporting");
        foreach(Layer l in layers)
        {
            //Debug.Log(l);
        }

        if (name.Length == 0)
        {
            name = "default_name";
        }
        try {
            ObjExporter.TerrainToFile(gm.cm.layerManager, layers, path + "\\" + name + ".obj");
            gm.message.ShowMessage("TERRAIN EXPORTED", 5*30);
        }
        catch (Exception ex)
        {
            if (ex is UnauthorizedAccessException)
            {
                gm.message.ShowMessage("terrain could not be saved \n UnauthorizedAccessException", 5 * 30);
            }
            else if (ex is DirectoryNotFoundException)
            {
                gm.message.ShowMessage("terrain could not be saved \n directory was not found", 5 * 30);
            }
            else
            {
                gm.message.ShowMessage("terrain could not be saved \n" + ex.GetType(), 5 * 30);
            }
        }


    }
}
