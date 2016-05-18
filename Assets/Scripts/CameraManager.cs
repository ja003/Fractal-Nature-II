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
    public ErosionGenerator erosionGenerator;

    private FunctionMathCalculator functionMathCalculator;
    private FunctionDebugger functionDebugger;
    private FunctionRiverDigger functionRiverDigger;
    private FunctionRiverPlanner functionRiverPlanner;
    private FunctionTerrainManager functionTerrainManager;

    private GridManager gridManager;

    public GUIMessage guiMessage;

    public int terrainWidth;
    public int terrainHeight;
    public int patchSize; //size of generated terrain patch
    public int scaleTerrainY = 15;

    void Start () {

        guiManager = GameObject.Find("GUI").GetComponent<GUIManager>();

        //TODO: terrainWidth has to be same as terrainHeight (only due to mesh construction error)
        terrainWidth = 100; 
        terrainHeight = 100;
        patchSize = 64;
        scaleTerrainY = 12;

        int quadrantSize = Math.Max(terrainWidth, terrainHeight);

        layerManager = new LayerManager();

        globalTerrain = new GlobalTerrain(quadrantSize);
        localTerrain = new LocalTerrain(terrainWidth, terrainHeight, 30, globalTerrain, layerManager);
        filterGenerator = new FilterGenerator(quadrantSize, localTerrain);

        functionMathCalculator = new FunctionMathCalculator();
        functionDebugger = new FunctionDebugger();
        functionRiverDigger = new FunctionRiverDigger();
        functionRiverPlanner = new FunctionRiverPlanner();
        functionTerrainManager = new FunctionTerrainManager();
        
        terrainGenerator = new TerrainGenerator(patchSize);
        riverGenerator = new RiverGenerator(localTerrain);
        erosionGenerator = new ErosionGenerator(localTerrain);

        gridManager = new GridManager(new Vector3(0,0,0), patchSize, patchSize);
        

        AssignFunctions();
        terrainGenerator.initialize(scaleTerrainY);
        //localTerrain.UpdateVisibleTerrain(new Vector3(0, 0, 0), true);


        //filterGenerator.PerserveMountains(3, 50, 10);
        //terrainGenerator.build();

        //Debug.Log(gridManager.GetCenterOnGrid(new Vector3(1, 0, 1)));
        

        localTerrain.UpdateVisibleTerrain(new Vector3(0, 0, 0), false);
    }

    void FixedUpdate()
    {
        if(Time.frameCount == 1)
        {
            //localTerrain.UpdateVisibleTerrain(new Vector3(0, 0, 0), false);

            //localTerrain.lm = layerManager;
            //Debug.Log("updated");
        }
    }
    
    public void AssignFunctions()
    {
        localTerrain.AssignFunctions(globalTerrain.globalTerrainC, 
            terrainGenerator, filterGenerator, riverGenerator, erosionGenerator);

        terrainGenerator.AssignFunctions(globalTerrain, localTerrain, 
            filterGenerator, functionMathCalculator, riverGenerator, gridManager, guiManager,
            erosionGenerator);

        filterGenerator.AssignFunctions(functionMathCalculator, localTerrain, functionTerrainManager);

        riverGenerator.AssignFunctions(functionTerrainManager, functionRiverPlanner, functionDebugger,
            functionMathCalculator, functionRiverDigger, guiManager.river);

        functionDebugger.AssignFunctions(riverGenerator);
        functionRiverDigger.AssignFunctions(riverGenerator);
        functionRiverPlanner.AssignFunctions(riverGenerator);
        functionMathCalculator.AssignFunctions(localTerrain);
        functionTerrainManager.AssignFunctions(localTerrain, 
            functionMathCalculator, riverGenerator, layerManager);

        layerManager.AssignFunctions(terrainGenerator, filterGenerator, riverGenerator, erosionGenerator);

        erosionGenerator.AssignFunctions(functionTerrainManager);

        layerManager = localTerrain.lm;
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

    int hydraulicErosionStep = 1;

    void Update () {

        //generate terrain when camera gets close to border
        if(guiManager.onFlyGeneration && Get2dDistance(gameObject.transform.position, localTerrain.localTerrainC.center) > 70)
        {
            //FixCameraPosition(); //no need now
            //Debug.Log("moving to center: " + gameObject.transform.position);
            localTerrain.UpdateVisibleTerrain(gameObject.transform.position, false);
        }

        if (Input.GetKey("8") && lastActionFrame < Time.frameCount - 30)
        {
            FixCameraPosition();
            Debug.Log("threshold ");
            terrainGenerator.filterMinThresholdLayer = !terrainGenerator.filterMinThresholdLayer;
            terrainGenerator.filterMaxThresholdLayer = !terrainGenerator.filterMaxThresholdLayer;
            filterGenerator.tf.GenerateMinThresholdInRegion(localTerrain.GetVisibleArea(), 0, 3);
            filterGenerator.tf.GenerateMaxThresholdInRegion(localTerrain.GetVisibleArea(), 1, 3);
            terrainGenerator.build();
            lastActionFrame = Time.frameCount;
        }


        if (Input.GetKey("m") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("mark axis");
            terrainGenerator.markAxis = !terrainGenerator.markAxis;
            terrainGenerator.build();
            lastActionFrame = Time.frameCount;
        }

        if (Input.GetKey("u") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("delete water");
            erosionGenerator.he.waterMap.ResetQuadrants();
            terrainGenerator.build();

            Debug.Log(erosionGenerator.he.WaterValuesString(localTerrain.GetVisibleArea()));
            lastActionFrame = Time.frameCount;
        }

        if (Input.GetKey("7") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("distribute water");
            hydraulicErosionStep = 1;
            erosionGenerator.he.DistributeWater(localTerrain.GetVisibleArea(), hydraulicErosionStep, 0.1f);
            terrainGenerator.build();

            //Debug.Log(erosionGenerator.he.WaterValuesString(localTerrain.GetVisibleArea()));
            lastActionFrame = Time.frameCount;
        }
        if (Input.GetKey("6") && lastActionFrame < Time.frameCount - 10)
        {
            //Debug.Log("hydraulic erosion step " + hydraulicErosionStep);
            erosionGenerator.he.HydraulicErosionStep();
            terrainGenerator.build();

            //Debug.Log(erosionGenerator.he.ErosionValuesString(localTerrain.GetVisibleArea()));

            hydraulicErosionStep++;
            lastActionFrame = Time.frameCount;
        }
        if (Input.GetKey("5") && lastActionFrame < Time.frameCount - 30)
        {
            hydraulicErosionStep += 10;
            //Debug.Log("hydraulic erosion " + hydraulicErosionStep + " steps ");
            for(int i = hydraulicErosionStep - 10; i < hydraulicErosionStep; i++)
            {
                erosionGenerator.he.HydraulicErosionStep();
            }
            //Debug.Log(erosionGenerator.he.WaterValuesString(localTerrain.GetVisibleArea()));
            //Debug.Log(erosionGenerator.he.TerrainWaterValuesString(localTerrain.GetVisibleArea()));
            Debug.Log(erosionGenerator.he.ErosionValuesString(localTerrain.GetVisibleArea()));

            terrainGenerator.build();

            lastActionFrame = Time.frameCount;
        }
        if (Input.GetKey("4") && lastActionFrame < Time.frameCount - 30)
        {
            /*Debug.Log("thermal erosion");

            erosionGenerator.te.ThermalErosionStep(localTerrain.GetVisibleArea(), 500, 0.00002f, 0.2f);

            terrainGenerator.build();*/
            Debug.Log("color peaks");

            List<Vertex> closestPeaks =
                terrainGenerator.ds.mountainPeaksManager.GetClosestPeaks(localTerrain.localTerrainC.center);
            foreach (Vertex v in closestPeaks)
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
            Debug.Log("print terrain");
            terrainGenerator.PrintTerrain();
           
            lastActionFrame = Time.frameCount;
        }

        //L
        if (Input.GetKey("l") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("procedural texture");
            textureFlag = !textureFlag;
            terrainGenerator.RefreshProceduralTexture();
            terrainGenerator.setTexture(textureFlag);
            //terrainGenerator.build();
            lastActionFrame = Time.frameCount;
        }

        if (Input.GetKey("n") && lastActionFrame < Time.frameCount - 30)
        {
            
            lastActionFrame = Time.frameCount;
        }
        if (Input.GetKey("b") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("filter erosion");
            erosionGenerator.he.FilterErosionIn(localTerrain.GetVisibleArea(), guiManager.filter.spikeThreshold);
            terrainGenerator.build();

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
