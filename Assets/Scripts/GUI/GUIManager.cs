using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;

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
    public GUIFilters filter;
    public GUIRiver river;
    GUIDebug debug;
    public GUIErosion erosion;
    public GUIMessage message;

    float deltaTime = 0.0f;
    public float msecMax = 0;

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
        filterMenuFlag = false ;
        riverMenuFlag = false;
        debugMenuFlag = true;

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
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        if (Time.frameCount <= 1)
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
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
        if(msec > msecMax)
        {
            msecMax = msec;
            //Debug.Log("msecMax: " + msecMax);
        }

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

