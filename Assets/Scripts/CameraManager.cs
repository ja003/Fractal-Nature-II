using UnityEngine;
using System.Collections;
using System;

public class CameraManager : MonoBehaviour, ICameraManager
{
    private GlobalTerrain globalTerrain;
    private LocalTerrain localTerrain;
    private TerrainGenerator terrainGenerator;
    private FilterGenerator filterGenerator;
    private FunctionMathCalculator fmc;

    int terrainWidth;
    int terrainHeight;

    void Start () {
        
        terrainWidth = 200; //TODO: terrain has to be exactly 200x200 and terrainWidth = terrainWidth / 6....why??
        terrainHeight = 200;

        int quadrantSize = Math.Max(terrainWidth, terrainHeight);

        globalTerrain = new GlobalTerrain(quadrantSize);
        terrainGenerator = new TerrainGenerator();
        localTerrain = new LocalTerrain(terrainWidth, terrainHeight, 30);
        filterGenerator = new FilterGenerator(quadrantSize, localTerrain);

        fmc = new FunctionMathCalculator();
            
        AssignFunctions();
        terrainGenerator.initialize();
        localTerrain.UpdateVisibleTerrain(new Vector3(0, 0, 0));
        //filterGenerator.PerserveMountains(3, 50, 10);
        terrainGenerator.build();
    }

    public void AssignFunctions()
    {
        //localTerrain.globalTerrain = globalTerrain;
        localTerrain.terrainGenerator = terrainGenerator;
        localTerrain.AssignFunctions(globalTerrain.globalTerrainC);

        terrainGenerator.AssignFunctions(globalTerrain, localTerrain, filterGenerator);

        filterGenerator.AssignFunctions(fmc, localTerrain);
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

        if (Input.GetKey("8") && lastActionFrame < Time.frameCount - 30)
        {
            FixCameraPosition();
            //Debug.Log("generating on: " + gameObject.transform.position);
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
            //filterGenerator.PerserveMountains(3, 50, 10);
            lastActionFrame = Time.frameCount;
        }
    }
}
