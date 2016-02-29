using UnityEngine;
using System.Collections;
using System;

public class CameraManager : MonoBehaviour, ICameraManager
{
    private GlobalTerrain globalTerrain;
    private LocalTerrain localTerrain;

    private TerrainGenerator terrainGenerator;
    private FilterGenerator filterGenerator;
    private RiverGenerator riverGenerator;
    
    private FunctionMathCalculator functionMathCalculator;
    private FunctionDebugger functionDebugger;
    private FunctionRiverDigger functionRiverDigger;
    private FunctionRiverPlanner functionRiverPlanner;
    private FunctionTerrainManager functionTerrainManager;

    int terrainWidth;
    int terrainHeight;

    void Start () {
        
        terrainWidth = 256; 
        terrainHeight = 256;

        int quadrantSize = Math.Max(terrainWidth, terrainHeight);

        globalTerrain = new GlobalTerrain(quadrantSize);
        localTerrain = new LocalTerrain(terrainWidth, terrainHeight, 30, globalTerrain);
        filterGenerator = new FilterGenerator(quadrantSize, localTerrain);

        functionMathCalculator = new FunctionMathCalculator();
        functionDebugger = new FunctionDebugger();
        functionRiverDigger = new FunctionRiverDigger();
        functionRiverPlanner = new FunctionRiverPlanner();
        functionTerrainManager = new FunctionTerrainManager();
        
        terrainGenerator = new TerrainGenerator();
        riverGenerator = new RiverGenerator(localTerrain);

        AssignFunctions();
        terrainGenerator.initialize();
        localTerrain.UpdateVisibleTerrain(new Vector3(0, 0, 0));
        //filterGenerator.PerserveMountains(3, 50, 10);
        //terrainGenerator.build();
    }
    
    public void AssignFunctions()
    {
        localTerrain.AssignFunctions(globalTerrain.globalTerrainC, terrainGenerator, filterGenerator, riverGenerator);

        terrainGenerator.AssignFunctions(globalTerrain, localTerrain, filterGenerator, functionMathCalculator, riverGenerator);
        filterGenerator.AssignFunctions(functionMathCalculator, localTerrain, functionTerrainManager);
        riverGenerator.AssignFunctions(functionTerrainManager, functionRiverPlanner, functionDebugger,
            functionMathCalculator, functionRiverDigger);

        functionDebugger.AssignFunctions(riverGenerator);
        functionRiverDigger.AssignFunctions(riverGenerator);
        functionRiverPlanner.AssignFunctions(riverGenerator);
        functionMathCalculator.AssignFunctions(localTerrain);
        functionTerrainManager.AssignFunctions(localTerrain, functionMathCalculator);
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

    void Update () {

        //generate terrain when camera gets close to border
        if(Get2dDistance(gameObject.transform.position, localTerrain.localCoordinates.center) > 70)
        {
            //FixCameraPosition();
            //Debug.Log("moving to center: " + gameObject.transform.position);
            //localTerrain.UpdateVisibleTerrain(gameObject.transform.position);
        }

        if (Input.GetKey("8") && lastActionFrame < Time.frameCount - 30)
        {
            FixCameraPosition();
            Debug.Log("generating on: " + gameObject.transform.position);
            localTerrain.UpdateVisibleTerrain(gameObject.transform.position);
            lastActionFrame = Time.frameCount;
        }

        if (Input.GetKey("7") && lastActionFrame < Time.frameCount - 30)
        {
            FixCameraPosition();
            Debug.Log("moving to: " + gameObject.transform.position);
            localTerrain.MoveVisibleTerrain(gameObject.transform.position);
            lastActionFrame = Time.frameCount;
        }
        if (Input.GetKey("6") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("perserve mountain");
            filterGenerator.mf.PerserveMountainsInRegion(localTerrain.localCoordinates.botLeft, localTerrain.localCoordinates.topRight, 3, 50, 10);
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
            Debug.Log("averaging");

            filterGenerator.af.GenerateAverageFilterInRegion(localTerrain.localCoordinates.botLeft, localTerrain.localCoordinates.topRight);
            lastActionFrame = Time.frameCount;
        }

        if (Input.GetKey("3") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("river");

            riverGenerator.GenerateNewRiver();
            lastActionFrame = Time.frameCount;
            //localTerrain.MoveVisibleTerrain(gameObject.transform.position);
        }
        if (Input.GetKey("2") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("median");
            filterGenerator.mdf.GenerateMedianFilterInRegion(localTerrain.localCoordinates.botLeft, localTerrain.localCoordinates.topRight);
            lastActionFrame = Time.frameCount;
        }

        if (Input.GetKey("1") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("color");
            riverGenerator.currentRiver.DrawRiver();
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

    /// <summary>
    /// returns distance of 2 points in X and Z space
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    float Get2dDistance(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(new Vector3(v1.x, 0, v1.z), new Vector3(v2.x, 0, v2.z));
    }
}
