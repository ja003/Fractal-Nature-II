﻿using UnityEngine;
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
    public bool debugMenuFlag;
    public bool terrainProcessing;

    public bool messageFlag;
    //public int messageEndFrame;



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
    public GUIRiver river;
    GUIDebug debug;
    GUIErosion erosion;
    public GUIMessage message;


    void Start()
    {
        cm = GameObject.Find("MainCamera").GetComponent<CameraManager>();

        menuWidth = 200;
        rightOffset = 5;
        topOffset = 5;
        menuButtonHeight = 40;
        smallButtonHeight = 20;

        fractalNatureFlag = true;
        generalSettingsFlag = true;
        erosionMenuFlag = false;
        filterMenuFlag = false ;
        riverMenuFlag = false;
        debugMenuFlag = false;

        messageFlag = false;
        //messageEndFrame = 666;

        terrainProcessing = false;

        scaleY = cm.scaleTerrainY;
        visibleArea = cm.terrainWidth;
        //visibleArea = 100;
        patchSize = cm.patchSize;

        
        message = new GUIMessage(this);//has to be declared first!

        menu = new GUIMenu(this);
        cameraMenu = new GUICamera(this);
        mesh = new GUIMesh(this);
        export = new GUIExport(this);
        progress = new GUIProgress(this);
        filter = new GUIFilters(this);
        river = new GUIRiver(this);
        debug = new GUIDebug(this);
        erosion = new GUIErosion(this);

        AssignFunctions();
    }

    public void AssignFunctions()
    {
        cm.terrainGenerator.message = message;
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

        if (fractalNatureFlag && !erosionMenuFlag && !filterMenuFlag 
            && !riverMenuFlag && !debugMenuFlag)
        {
            generalSettingsFlag = true;
        }
        else
        {
            generalSettingsFlag = false;
        }

        if (fractalNatureFlag && !erosionMenuFlag && !generalSettingsFlag 
            && !riverMenuFlag && !debugMenuFlag)
        {
            filterMenuFlag = true;
        }
        else
        {
            filterMenuFlag = false;
        }

        if (fractalNatureFlag&& !erosionMenuFlag && !generalSettingsFlag 
            && !filterMenuFlag && !debugMenuFlag)
        {
            riverMenuFlag = true;
        }
        else
        {
            riverMenuFlag = false;
        }

        if (fractalNatureFlag && !erosionMenuFlag && !generalSettingsFlag 
            && !filterMenuFlag && !riverMenuFlag)
        {
            debugMenuFlag = true;
        }
        else
        {
            debugMenuFlag = false;
        }

        if (fractalNatureFlag && !debugMenuFlag && !generalSettingsFlag
            && !filterMenuFlag && !riverMenuFlag)
        {
            erosionMenuFlag = true;
        }
        else
        {
            erosionMenuFlag = false;
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

        if (debugMenuFlag)
        {
            debug.OnGui(menu.yPos + 5);
        }

        if (erosionMenuFlag)
        {
            erosion.OnGui(menu.yPos + 5);
        }
        
        if (messageFlag && Time.frameCount < message.messageEndFrame)
        {
            message.OnGui(Screen.height - 100);
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
        s += "debug = " + debugMenuFlag + "\n";

        return s;
    }

}

