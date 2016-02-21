using UnityEngine;
using System.Collections;
using System;

public class CameraManager : MonoBehaviour, ICameraManager
{
    private GlobalTerrain globalTerrain;
    private LocalTerrain localTerrain;
    private TerrainGenerator terrainGenerator;
    private FilterGenerator filterGenerator;
    private FunctionMathCalculator functionMathCalculator;

    int terrainWidth;
    int terrainHeight;

    void Start () {
        
        terrainWidth = 200; //TODO: terrain has to be exactly 200x200 and terrainWidth = terrainWidth / 6....why??
        terrainHeight = 200;

        int quadrantSize = Math.Max(terrainWidth, terrainHeight);

        globalTerrain = new GlobalTerrain(quadrantSize);
        terrainGenerator = new TerrainGenerator();
        localTerrain = new LocalTerrain(terrainWidth, terrainHeight, 30, globalTerrain);
        filterGenerator = new FilterGenerator(quadrantSize, localTerrain);

        functionMathCalculator = new FunctionMathCalculator();
            
        AssignFunctions();
        terrainGenerator.initialize();
        localTerrain.UpdateVisibleTerrain(new Vector3(0, 0, 0));
        //filterGenerator.PerserveMountains(3, 50, 10);
        //terrainGenerator.build();
    }

    public void AssignFunctions()
    {
        //localTerrain.globalTerrain = globalTerrain;
        localTerrain.tg = terrainGenerator;
        localTerrain.AssignFunctions(globalTerrain.globalTerrainC, filterGenerator);

        terrainGenerator.AssignFunctions(globalTerrain, localTerrain, filterGenerator, functionMathCalculator);
        

        filterGenerator.AssignFunctions(functionMathCalculator, localTerrain);
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
        if(Get2dDistance(gameObject.transform.position, localTerrain.localTerrainC.center) > 70)
        {
            FixCameraPosition();
            //Debug.Log("moving to center: " + gameObject.transform.position);
            localTerrain.UpdateVisibleTerrain(gameObject.transform.position);
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
            filterGenerator.mf.PerserveMountainsInRegion(localTerrain.localTerrainC.botLeft, localTerrain.localTerrainC.topRight, 3, 50, 10);
            lastActionFrame = Time.frameCount;
        }
        if (Input.GetKey("5") && lastActionFrame < Time.frameCount - 30)
        {
            filterGenerator.ResetFilters();
            lastActionFrame = Time.frameCount;
        }
        if (Input.GetKey("4") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("averaging");

            filterGenerator.af.GenerateAverageFilterInRegion(localTerrain.localTerrainC.botLeft, localTerrain.localTerrainC.topRight);
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
