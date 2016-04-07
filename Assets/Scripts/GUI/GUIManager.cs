using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour
{
    public CameraManager cm;

    int frameMark = 0;


    public int menuWidth;
    public int rightOffset;
    public int topOffset;
    public int menuButtonHeight;
    public int smallButtonHeight;


    public bool fractalNatureFlag;
    public bool generalSettingsFlag;
    public bool erosionMenuFlag;
    public bool filterMenuFlag;
    public bool riverMenuFlag;
    public bool terrainProcessing;


    public bool onFlyGeneration;

    public int patchSize;
    public float scaleY;
    public int visibleArea;


    GUIMenu menu;
    GUICamera cameraMenu;
    GUIMesh mesh;
    GUIExport export;
    public GUIProgress progress;
    GUIFilters filter;
    GUIRiver river;


    void Start()
    {
        cm = GameObject.Find("MainCamera").GetComponent<CameraManager>();

        menuWidth = 200;
        rightOffset = 5;
        topOffset = 5;
        menuButtonHeight = 40;
        smallButtonHeight = 20;

        fractalNatureFlag = true;
        generalSettingsFlag = false;
        erosionMenuFlag = false;
        filterMenuFlag = true;
        riverMenuFlag = false;
        terrainProcessing = false;

        scaleY = cm.scaleTerrainY;
        visibleArea = cm.terrainWidth * 2;
        //visibleArea = 100;
        patchSize = cm.patchSize;

        menu = new GUIMenu(this);
        cameraMenu = new GUICamera(this);
        mesh = new GUIMesh(this);
        export = new GUIExport(this);
        progress = new GUIProgress(this);
        filter = new GUIFilters(this);
        river = new GUIRiver(this);
    }
    
    void Update()
    {
        if(Time.frameCount <= 1)
        {
            scaleY = cm.scaleTerrainY;
            mesh.scaleY = scaleY;

            visibleArea = cm.terrainWidth;
            mesh.visibleArea = visibleArea;

            patchSize = cm.patchSize;


            terrainProcessing = false;
        }
    }

    public void UpdatePatchSize(int patchSize)
    {
        this.patchSize = patchSize;
        cm.UpdatePatchSize(patchSize);
    }

    /// <summary>
    /// show/hide menu objects handler
    /// </summary>
    public void CascadeVisibility()
    {
        if (!fractalNatureFlag)
        {
            generalSettingsFlag = false;
            //meshControlFlag = false;
        }

        if (fractalNatureFlag && !erosionMenuFlag && !filterMenuFlag && !riverMenuFlag)
        {
            generalSettingsFlag = true;
        }
        else
        {
            generalSettingsFlag = false;
        }

        if (fractalNatureFlag && !erosionMenuFlag && !generalSettingsFlag && !riverMenuFlag)
        {
            filterMenuFlag = true;
        }
        else
        {
            filterMenuFlag = false;
        }

        if (fractalNatureFlag&& !erosionMenuFlag && !generalSettingsFlag && !filterMenuFlag)
        {
            riverMenuFlag = true;
        }
        else
        {
            riverMenuFlag = false;
        }
    }

    // On-screen Menu Loop
    void OnGUI()
    {
        //This section stores the data regarding the interactive menu.
        CascadeVisibility();

        menu.OnGUI(topOffset);

        
        if (generalSettingsFlag)
        {
            cameraMenu.OnGui(menu.yPos + 5);
            mesh.OnGui(cameraMenu.yPos + 5);
            export.OnGui(mesh.yPos + 5);
        }

        if (terrainProcessing)
        {
            progress.OnGui();
        }

        if (filterMenuFlag)
        {
            filter.OnGui(menu.yPos + 5);
        }

        if (riverMenuFlag)
        {
            river.OnGui(menu.yPos + 5);
        }
        
    }

    public override string ToString()
    {
        string s = "";
        s += "fractalNature = " + fractalNatureFlag + "\n";
        s += "generalSettings = " + generalSettingsFlag + "\n";
        s += "filterMenu = " + filterMenuFlag + "\n";
        s += "erosionMenu = " + erosionMenuFlag + "\n";
        s += "riverMenu = " + riverMenuFlag + "\n";

        return s;
    }

}

