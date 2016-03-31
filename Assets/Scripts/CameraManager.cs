using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour, ICameraManager
{
    public GUIManager guiManager;

    public GlobalTerrain globalTerrain;
    public LocalTerrain localTerrain;

    public LayerManager layerManager;

    public TerrainGenerator terrainGenerator;
    public FilterGenerator filterGenerator;
    public RiverGenerator riverGenerator;
    
    private FunctionMathCalculator functionMathCalculator;
    private FunctionDebugger functionDebugger;
    private FunctionRiverDigger functionRiverDigger;
    private FunctionRiverPlanner functionRiverPlanner;
    private FunctionTerrainManager functionTerrainManager;

    private GridManager gridManager;

    public int terrainWidth;
    public int terrainHeight;
    public int patchSize; //size of generated terrain patch
    public int scaleTerrainY = 15;

    void Start () {

        guiManager = GameObject.Find("GUI").GetComponent<GUIManager>();

        //TODO: terrainWidth has to be same as terrainHeight (only due to mesh construction error)
        terrainWidth = 150; 
        terrainHeight = 150;
        patchSize = 128;
        scaleTerrainY = 12;

        int quadrantSize = Math.Max(terrainWidth, terrainHeight);

        globalTerrain = new GlobalTerrain(quadrantSize);
        localTerrain = new LocalTerrain(terrainWidth, terrainHeight, 30, globalTerrain);
        filterGenerator = new FilterGenerator(quadrantSize, localTerrain);

        functionMathCalculator = new FunctionMathCalculator();
        functionDebugger = new FunctionDebugger();
        functionRiverDigger = new FunctionRiverDigger();
        functionRiverPlanner = new FunctionRiverPlanner();
        functionTerrainManager = new FunctionTerrainManager();
        
        terrainGenerator = new TerrainGenerator(patchSize);
        riverGenerator = new RiverGenerator(localTerrain);

        gridManager = new GridManager(new Vector3(0,0,0), patchSize, patchSize);
        layerManager = new LayerManager();

        AssignFunctions();
        terrainGenerator.initialize(scaleTerrainY);
        localTerrain.UpdateVisibleTerrain(new Vector3(0, 0, 0));


        //filterGenerator.PerserveMountains(3, 50, 10);
        //terrainGenerator.build();

        //Debug.Log(gridManager.GetCenterOnGrid(new Vector3(1, 0, 1)));
    }
    
    public void AssignFunctions()
    {
        localTerrain.AssignFunctions(globalTerrain.globalTerrainC, 
            terrainGenerator, filterGenerator, riverGenerator);

        terrainGenerator.AssignFunctions(globalTerrain, localTerrain, 
            filterGenerator, functionMathCalculator, riverGenerator, gridManager, guiManager);

        filterGenerator.AssignFunctions(functionMathCalculator, localTerrain, functionTerrainManager);

        riverGenerator.AssignFunctions(functionTerrainManager, functionRiverPlanner, functionDebugger,
            functionMathCalculator, functionRiverDigger);

        functionDebugger.AssignFunctions(riverGenerator);
        functionRiverDigger.AssignFunctions(riverGenerator);
        functionRiverPlanner.AssignFunctions(riverGenerator);
        functionMathCalculator.AssignFunctions(localTerrain);
        functionTerrainManager.AssignFunctions(localTerrain, functionMathCalculator, riverGenerator);

        layerManager.AssignLayers(globalTerrain.globalTerrainC, riverGenerator);
    }

    int lastActionFrame = 0;

    void FixCameraPosition()
    {
        gameObject.transform.position =
            new Vector3(
                (int)gameObject.transform.position.x,
                gameObject.transform.position.y,
                (int)gameObject.transform.position.z);
    }


    bool textureFlag = true;
    void Update () {

        //generate terrain when camera gets close to border
        if(guiManager.onFlyGeneration && Get2dDistance(gameObject.transform.position, localTerrain.localTerrainC.center) > 70)
        {
            //FixCameraPosition(); //no need now
            //Debug.Log("moving to center: " + gameObject.transform.position);
            localTerrain.UpdateVisibleTerrain(gameObject.transform.position);
        }

        if (Input.GetKey("8") && lastActionFrame < Time.frameCount - 30)
        {
            FixCameraPosition();
            Debug.Log("[8]: generating on: " + gameObject.transform.position);
            localTerrain.UpdateVisibleTerrain(gameObject.transform.position);
            lastActionFrame = Time.frameCount;
        }

        if (Input.GetKey("7") && lastActionFrame < Time.frameCount - 30)
        {
            FixCameraPosition();
            //Debug.Log("moving to: " + gameObject.transform.position);
            localTerrain.MoveVisibleTerrain(gameObject.transform.position, true);
            lastActionFrame = Time.frameCount;
        }
        if (Input.GetKey("6") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("perserve mountain");
            filterGenerator.mf.PerserveMountainsInRegion(localTerrain.localTerrainC.botLeft, localTerrain.localTerrainC.topRight, 3, 50, 10);
            lastActionFrame = Time.frameCount;
        }
        if (Input.GetKey("5") && lastActionFrame < Time.frameCount - 30)
        {
            filterGenerator.ResetFilters();
            riverGenerator.ResetRivers();
            lastActionFrame = Time.frameCount;
        }
        if (Input.GetKey("4") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("color peaks");

            List<Vertex> closestPeaks = 
                terrainGenerator.ds.mountainPeaksManager.GetClosestPeaks(localTerrain.localTerrainC.center);
            foreach(Vertex v in closestPeaks)
            {
                riverGenerator.fd.ColorPixel(v.x, v.z, 3, riverGenerator.fd.greenColor);
            }

            lastActionFrame = Time.frameCount;
        }

        if (Input.GetKey("3") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("draw rivers");
            /*
            riverGenerator.GenerateNewRiver();
            lastActionFrame = Time.frameCount;
            terrainGenerator.build();*/


            foreach (RiverInfo river in riverGenerator.rivers)
            {
                river.DrawRiver();
            }

            lastActionFrame = Time.frameCount;
            //Debug.Log(terrainGenerator.patchSize);
            //Debug.Log(terrainGenerator.terrainWidth);
            //Debug.Log(localTerrain.terrainWidth);
            //Debug.Log(localTerrain.localTerrainC.terrainWidth);
            //Debug.Log(gridManager.stepX);

            //localTerrain.MoveVisibleTerrain(gameObject.transform.position);
        }
        if (Input.GetKey("2") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("default river"); 
            riverGenerator.GenerateDefaultRiver();
            lastActionFrame = Time.frameCount;
        }

        if (Input.GetKey("1") && lastActionFrame < Time.frameCount - 30)
        {
            Vertex start = new Vertex(-140, -140);
            start.height = localTerrain.globalTerrainC.GetValue(start.x, start.z);
            Vertex end = new Vertex(140, 140);
            end.height = localTerrain.globalTerrainC.GetValue(end.x, end.z);

            if(riverGenerator.rivers.Count > 0)
            {
                start = riverGenerator.rivers[riverGenerator.rivers.Count - 1].riverPath[0];
                start.height = localTerrain.globalTerrainC.GetValue(start.x, start.z);
                end = riverGenerator.rivers[riverGenerator.rivers.Count - 1].GetLastVertex();
                end.height = localTerrain.globalTerrainC.GetValue(end.x, end.z);

            }


            RiverInfo river = riverGenerator.frp.GetRiverFromTo(start, end);
            Debug.Log(river);
            river.DrawRiver();
            lastActionFrame = Time.frameCount;
        }

        //L
        if (Input.GetKey("l") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("procedural texture");
            textureFlag = !textureFlag;
            terrainGenerator.setTexture(textureFlag);
            terrainGenerator.build();
            lastActionFrame = Time.frameCount;
        }

        if (Input.GetKey("p") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("diamond square");

            lastActionFrame = Time.frameCount;
        }
        if (Input.GetKey("o") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("applying diamond square");

            lastActionFrame = Time.frameCount;
        }
    }

    public void UpdatePatchSize(int patchSize)
    {
        this.patchSize = patchSize;
        terrainGenerator.patchSize = patchSize;
        gridManager.UpdateSteps(patchSize, patchSize);
    }

    public void UpdateVisibleArea(int visibleArea)
    {
        terrainWidth = visibleArea;
        terrainHeight= visibleArea;

        localTerrain.UpdateVisibleArea(visibleArea);

        terrainGenerator.UpdateVisibleArea(visibleArea);
    }

    /// <summary>
    /// returns distance of 2 points in X and Z space
    /// </summary>
    float Get2dDistance(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(new Vector3(v1.x, 0, v1.z), new Vector3(v2.x, 0, v2.z));
    }
}
