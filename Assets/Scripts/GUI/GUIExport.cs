using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIExport : MonoBehaviour {

    int menuWidth;
    int rightMenuOffset;
    int topOffset;
    int buttonHeight;
    public float yPos;
    int sideOffset;

    public float scaleY;
    public float visibleArea;

    GUIManager gm;

    public GUIExport(GUIManager gm)
    {
        this.gm = gm;

        menuWidth = gm.menuWidth;
        rightMenuOffset = gm.rightOffset;
        topOffset = gm.topOffset;
        buttonHeight = gm.menuButtonHeight;
        sideOffset = 10;
        scaleY = gm.scaleY;
        visibleArea = gm.visibleArea;
    }

    public void OnGui(float yPosition)
    {
        float buttonWidth = menuWidth - 2* sideOffset;

        yPos = yPosition;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, buttonHeight + 4), " ");

        yPos += 2;

        if (GUI.Button(new Rect(Screen.width - menuWidth + 10, yPos, buttonWidth, buttonHeight), "EXPORT MESH"))
        {
            Export();
        }
    }

    private void Export()
    {
        //gm.cm.objExporter.t
        //string s = ObjExporter.TerrainToString(gm.cm.localTerrain.globalTerrainC);
        //string s = ObjExporterMy.MeshToString(gm.cm.terrainGenerator.myMesh[0]);

        //string fileName = Application.persistentDataPath + "/" + FILE_NAME; fileWriter = File.CreateText(fileName); fileWriter.WriteLine("Hello world"); fileWriter.Close();


        //ObjExporter.MeshToFile(gm.cm.terrainGenerator.myTerrain[0].GetComponent<MeshFilter>(), "C:\\Users\\Vukmir\\Desktop\\obj\\myTerrain_0.obj");
        //ObjExporter.MeshToFile(gm.cm.terrainGenerator.myTerrain[1].GetComponent<MeshFilter>(), "C:\\Users\\Vukmir\\Desktop\\obj\\myTerrain_1.obj");
        List<Layer> layers = new List<Layer>();
        layers.Add(Layer.terrain);
        layers.Add(Layer.river);
        ObjExporter.TerrainToFile(gm.cm.layerManager, layers, "C:\\Users\\Vukmir\\Desktop\\obj\\gt_01.obj");
    }
}
