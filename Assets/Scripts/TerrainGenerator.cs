using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TerrainGenerator
{
    public GUIManager guiManager;
    public GUIMessage message;

    public GlobalTerrain globalTerrain;
    public LocalTerrain localTerrain;

    public FilterGenerator filterGenerator;
    public RiverGenerator riverGenerator;
    public ErosionGenerator erosionGenerator;

    public DiamondSquare ds;
    public RandomTerrain rt;

    public FunctionMathCalculator fmc;

    public GridManager gm;

    public int terrainWidth;
    public int terrainHeight;
    public int patchSize;

    public bool waterMesh = true;
    public bool markAxis = false;

    //------LAYERS-----


    public bool terrainLayer = true;
    public bool riverLayer = true;

    public bool filterAverageLayer = false;
    public bool filterMedianLayer = false;
    public bool filterSpikeLayer = false;
    public bool filterGaussianLayer = false;
    public bool filterMinThresholdLayer = false;
    public bool filterMaxThresholdLayer = false;

    public bool erosionHydraulicLayer = true;
    public bool erosionThermalLayer = true;


    //------/LAYERS-----

    //------PATCH PARAMETERS-----
    public float noise = 3;
    public float noise_min = 1;
    public float noise_max = 4;
    public int rStep = 10;

    public float rMin = -0.5f;
    float rMin_min = -1;
    float rMin_max = 0.5f;

    public float rMax = 0.5f;
    float rMax_min = 0;
    float rMax_max = 1;

    public float maxRdif = 1f;
    public float minRdif = 0.5f;
    
    public PatchManager pm;
    public bool colorMode = false;
    public bool debugHeightmap = true;
    public bool debugRmin = false;
    public bool debugRmax = false;
    public bool debugNoise = false;

    public float terrainBrightness = 0f;


    //------/PATCH PARAMETERS-----
    GUIterrainPatch gtp;

    int individualMeshWidth;
    int individualMeshHeight;
    public Vector3 scaleTerrain;

    public TerrainGenerator(int patchSize)
    {
        //initialize(64,3);
        ds = new DiamondSquare(this, patchSize);
        rt = new RandomTerrain(this);
        this.patchSize = patchSize;
        try
        {
            GUIterrainPlannerMenu tpMenu = GameObject.Find("TerrainPlanner").GetComponent<GUIterrainPlannerMenu>();
            pm = tpMenu.patch.pm;
            gtp = tpMenu.patch;
            extraPatchCount = tpMenu.patch.extraPatchCount;
        }
        catch (Exception e)
        {
            Debug.Log("TerrainPlanner not found"); 
            //pm = new PatchManager(patchSize);
            gtp = new GUIterrainPatch(patchSize);
            gtp.SetDefaultPatch(DefaultTerrain.valleys);
            pm = gtp.pm;
            
            extraPatchCount = 0;
        }

    }

    public void AssignFunctions(GlobalTerrain globalTerrain, LocalTerrain localTerrain,
        FilterGenerator filterGenerator, FunctionMathCalculator functionMathCalculator,
        RiverGenerator riverGenerator, GridManager gridManager, GUIManager guiManager,
        ErosionGenerator erosionGenerator)
    {
        this.globalTerrain = globalTerrain;
        this.localTerrain = localTerrain;
        this.filterGenerator = filterGenerator;
        this.riverGenerator = riverGenerator;
        this.erosionGenerator = erosionGenerator;

        fmc = functionMathCalculator;

        gm = gridManager;
        

        rt.AssignFunctions(fmc);
        ds.AssignFunctions(localTerrain);

        this.guiManager = guiManager;
        message = guiManager.message;
    }

    public void MoveVisibleTerrain(Vector3 cameraPosition)
    {
        AssignHeightsToVertices();

        build();
    }

    

    public int extraPatchCount = 0;
    //// <summary>
    /// checks all points on grid in visible area are defined
    /// if not, regions with centers in points on the grid is generated
    /// </summary>
    public void PregenerateRegions(Vertex center, Area visibleArea, int patchSize, int extraPatchCount)
    {
        Vertex centerOnGrid = gm.GetPointOnGrid(center);
        Area surroundingArea = fmc.GetSurroundingAreaOf(centerOnGrid.Clone(), visibleArea, patchSize, extraPatchCount);
        
        extraPatchCount = 1; //set to 1 after first generation took place
        

        int x_min = surroundingArea.botLeft.x;
        int z_min = surroundingArea.botLeft.z;
        int x_max = surroundingArea.topRight.x;
        int z_max = surroundingArea.topRight.z;


        localTerrain.UpdateSize(patchSize, patchSize);
        
        Vertex tmpCenter;
        
        
        bool debug = false;

        foreach (PatchLevel i in pm.patchOrder)
        {
            for (int x = x_min; x <= x_max; x += patchSize)
            {
                for (int z = z_min; z <= z_max; z += patchSize)
                {
                    if (localTerrain.globalTerrainC.IsDefined(x, z))
                    {
                        //Debug.Log("defined: " + x + "," + z);
                    }
                    else
                    {
                        int level = (int)pm.patchLevel.GetValue(x, z, -1);


                        rMin = pm.rMin.GetValue(x, z, -1);
                        rMax = pm.rMax.GetValue(x, z, 1);
                        noise = pm.noise.GetValue(x, z, 2);


                        tmpCenter = new Vertex(x, z);
                        if (i == PatchLevel.low && level == 0)
                        {
                            if (debug)
                            {
                                Debug.Log(x + "," + z);
                                Debug.Log(i);
                            }
                            localTerrain.MoveVisibleTerrain(tmpCenter, false);
                            ds.Initialize(patchSize, noise, rMin, rMax);
                        }

                        if (i == PatchLevel.high && level == 2)
                        {
                            if (debug)
                            {
                                Debug.Log(x + "," + z);
                                Debug.Log(i);
                            }
                            localTerrain.MoveVisibleTerrain(tmpCenter, false);
                            ds.Initialize(patchSize, noise, rMin, rMax);
                            //pm.SetValues(tmpCenter, patchSize, rMin, rMax, noise);
                        }

                        if (i == PatchLevel.medium && level == 1)
                        {
                            if (debug)
                            {
                                Debug.Log(x + "," + z);
                                Debug.Log(i);
                            }
                            localTerrain.MoveVisibleTerrain(tmpCenter, false);
                            ds.Initialize(patchSize, noise, rMin, rMax);
                        }

                        if (i == PatchLevel.random && level == -1)
                        {
                            if (debug)
                            {
                                Debug.Log(x + "," + z);
                                Debug.Log(i);
                            }

                            Vertex patchC = gm.GetGridCoordinates(new Vertex(x, z));

                            gtp.SetPatchValue(patchC.x, patchC.z, PatchLevel.random);
                            rMin = pm.rMin.GetValue(x, z, -1);
                            rMax = pm.rMax.GetValue(x, z, 1);
                            noise = pm.noise.GetValue(x, z, 2);
                            
                            localTerrain.MoveVisibleTerrain(tmpCenter, false);
                            ds.Initialize(patchSize, noise, rMin, rMax);
                        }
                    }

                }
            }
        }
        //move back
        localTerrain.MoveVisibleTerrain(center, false);
        localTerrain.UpdateSize(terrainWidth, terrainHeight);

    }

    /// <summary>
    /// calculates roughness, rMin, rMax for defined patch
    /// </summary>
    public void CalculatePatchValues(Vertex center, int x, int z)
    {
        float roughnessAvg = pm.GetNeighbourAverage(x, z, PatchInfo.noise);
        if (roughnessAvg != 666)
            noise += UnityEngine.Random.Range(-0.5f, 0.4f);
        if (noise < noise_min)
            noise = noise_min;
        if (noise > noise_max)
            noise = noise_max;

        //rMin
        float rMinAvg = pm.GetNeighbourAverage(x, z, PatchInfo.rMin);
        if (rMinAvg != 666)
            rMin += UnityEngine.Random.Range(-noise / rStep, noise / rStep);
        if (rMin < rMin_min)
            rMin = rMin_min;
        if (rMin > rMin_max)
            rMin = rMin_max;


        //rMax
        float rMaxAvg = pm.GetNeighbourAverage(x, z, PatchInfo.rMax);
        if (rMaxAvg != 666)
            rMax += UnityEngine.Random.Range(-noise / rStep, noise / rStep);
        if (rMax < rMax_min)
            rMax = rMax_min;
        if (rMax > rMax_max)
            rMax = rMax_max;
        //rMax = rMin + roughness / 3;

        //dif
        if (rMax - rMin < minRdif)
            rMax += noise / 3;
        if (rMax - rMin > maxRdif)
            rMax = rMin + maxRdif;
        //Debug.Log(roughness);
        //Debug.Log(rMin);
        //Debug.Log(rMax);

        pm.SetValues(center, patchSize, rMin, rMax, noise);
    }
    
      
    /// <summary>
    /// generates terrain with predefined shape
    /// moves camera to center
    /// </summary>
    public void GenerateDefaultTerrain(TerrainType type, int size) 
    {
        float factor = 1f;
        for (int x = -size; x < size; x++)
        {
            for (int z = -size; z < size; z++)
            {
                switch (type)
                {
                    case TerrainType.gradientX_lr:
                        localTerrain.globalTerrainC.SetValue(x, z, factor*x / size);
                        break;
                    case TerrainType.gradientX_rl:
                        localTerrain.globalTerrainC.SetValue(x, z, factor * (- x) / size);
                        break;

                    case TerrainType.gradientZ_lr:
                        localTerrain.globalTerrainC.SetValue(x, z, factor * (z+Mathf.Sin(z)/2) / size);
                        break;
                    case TerrainType.gradientZ_rl:
                        localTerrain.globalTerrainC.SetValue(x, z, factor * (- z) / size);
                        break;

                    case TerrainType.constant:
                        localTerrain.globalTerrainC.SetValue(x, z, -0.1f);
                        break;
                    case TerrainType.gradient_radialPlus:
                        float distance = Vector2.Distance(new Vector2(0, 0), new Vector2(x, z));
                        localTerrain.globalTerrainC.SetValue(x, z, (size - distance)/30);
                        int c = 5;
                        //if (z > -c && z < c && x > -c && x < c)
                        //    localTerrain.globalTerrainC.SetValue(x, z, 0.3f);
                        break;
                    case TerrainType.gradient_radialMinus:
                        distance = Vector2.Distance(new Vector2(0, 0), new Vector2(x, z));
                        localTerrain.globalTerrainC.SetValue(x, z, distance / 30);
                        c = 5;
                        //if (z > -c && z < c && x > -c && x < c)
                        //    localTerrain.globalTerrainC.SetValue(x, z, 0.3f);
                        break;
                    case TerrainType.river:
                        //c = 10;
                        //if(x > -c && x < c)
                        //{
                        //    distance = Vector2.Distance(new Vector2(0, z), new Vector2(x, z));
                        //    //float d0 = Vector2.Distance(new Vector2(0, z), new Vector2(c, z));
                        //    localTerrain.globalTerrainC.SetValue(x, z, - distance/30);
                        //}
                        //else
                        //    localTerrain.globalTerrainC.SetValue(x, z, 0);
                        localTerrain.globalTerrainC.SetValue(x, z, 0.5f);
                        break;
                }
            }
        }
        if(type == TerrainType.river)
        {
            riverGenerator.GenerateDefaultRiver();
        }

        GameObject.Find("MainCamera").GetComponent<cameraMovement>().
            ChangePosition(new Vector3(0, 100, 0));
        GameObject.Find("MainCamera").GetComponent<cameraMovement>().
            ChangeRotation(new Vector3(90, 0, 0));
    }

    

    /// <summary>
    /// generates terrain around given center
    /// and pregenerates neighbouring regions
    /// </summary>
    public void GenerateTerrainOn(Vector3 center, bool defaultTerrain)
    {
        if (defaultTerrain)
            GenerateDefaultTerrain(TerrainType.gradient_radialMinus, terrainWidth); //can't generate river first!
        else
            PregenerateRegions(center, localTerrain.GetVisibleArea(), patchSize, extraPatchCount);
    }

    /// <summary>
    /// refresh procedural textur settings with default values
    /// </summary>
    public void RefreshProceduralTexture()
    {
        applyProceduralTex(true, sandColor, sandLimit, sandStrength, sandCoverage, true, grassColor, grassStrength, true, snowColor, snowLimit, snowStrength, snowCoverage, true, rockColor, slopeLimit, slopeStrength, noiseTexValue);
    }

    public void FixUnsetValues()
    {
        for (int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0; z < terrainHeight; z++)
            {
                if (localTerrain.GetLocalHeight(x, z) == 666)
                {
                    Debug.Log("fixing: " + x + "," + z);
                    SetVertex(x, z, localTerrain.GetNeighbourHeight(x, z));
                    //Debug.Log(vertices[x, z].y);
                }
            }
        }
    }



    /// <summary>
    /// copies global heights from visible area to vertices
    /// </summary>
    public void AssignHeightsToVertices()
    {
        for (int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0; z < terrainHeight; z++)
            {
                vertices[x, z].y = localTerrain.GetLocalHeight(x, z);
            }
        }
    }


    int counter = 0;
    List<Layer> layers;

    /// <summary>
    /// applies values from all layers to vertices
    /// </summary>
    public void ApplyLayers()
    {
        Vertex globalC;
        localTerrain.lm.UpdateLayers();

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0; z < terrainHeight; z++)
            {
                globalC = localTerrain.GetGlobalCoordinate(x, z);
                vertices[x, z].y = localTerrain.lm.GetValueFromLayers(
                    globalC.x, globalC.z, localTerrain.lm.activeLayers);
                W[x, z] = erosionGenerator.he.GetTerrainWatterValue(globalC.x, globalC.z);
            }
        }
    }
    

    public void UpdateScaleY(float scaleY)
    {
        scaleTerrain.y = scaleY;
        build();
    }

    public void UpdateVisibleArea(int visibleArea)
    {
        //terrainWidth = visibleArea;
        //terrainHeight = visibleArea;

        //vertices = new Vector3[terrainWidth, terrainHeight];
        destroyMeshes();

        build();
    }

    public void ResetTerrainValues()
    {
        localTerrain.globalTerrainC.ResetQuadrants();

        riverGenerator.ResetRivers();

        //filterGenerator.ResetFilters();
    }

    public void destroyMeshes()
    {

        for (int i = 0; i < 4; i++)
        {
            GameObject.Destroy(myMesh[i]);
            GameObject.Destroy(myWaterMesh[i]);
            GameObject.Destroy(myTerrain[i]);
            GameObject.Destroy(myWater[i]);
        }


        //TODO: reset filters

        initialize(scaleTerrain.y);
    }

    public void initialize(float scaleY)
    {
        terrainWidth = localTerrain.terrainWidth;
        terrainHeight = localTerrain.terrainHeight;

        scaleTerrain = new Vector3(terrainWidth, scaleY, terrainHeight);

        individualMeshWidth = terrainWidth / 2;
        individualMeshHeight = terrainHeight / 2;
        
        heightMap = new Texture2D(terrainWidth, terrainHeight, TextureFormat.RGBA32, false);
        waterMap = new Texture2D[4];
        proceduralTexture = new Texture2D(terrainWidth, terrainHeight);

        //Meshes initialisation. 4 Meshes are built for each due to 
        //Unity's inability to hold more than 65 000 vertices/mesh
        myMesh = new Mesh[4];
        myWaterMesh = new Mesh[4];

        W = new float[terrainWidth, terrainHeight]; //water map
        S = new float[terrainWidth, terrainHeight]; //sediment map
        V = new Vector2[terrainWidth, terrainHeight]; //velocity map
        F = new Vector4[terrainWidth, terrainHeight]; //outflow map
        
        int numVerts = (individualMeshWidth + 1) * (individualMeshHeight + 1);
        int numQuads = (individualMeshWidth - 1) * (individualMeshHeight - 1);
        int numTriangles = numQuads * 2;

        //Mesh data
        vertices = new Vector3[terrainWidth, terrainHeight];

        verticesOut = new Vector3[8][];
        normals = new Vector3[4][];
        uv = new Vector2[4][];
        triangles = new int[numTriangles * 3];

        //Water mesh data
        waterUv = new Vector2[numVerts];
        verticesWater = new Vector3[4][];

        //Initialising meshes data
        for (int i = 0; i < 4; i++)
        {
            verticesOut[i] = new Vector3[numVerts];
            uv[i] = new Vector2[numVerts];
            verticesWater[i] = new Vector3[numVerts];
            normals[i] = new Vector3[numVerts];
            //waterMap[i] = new Texture2D(individualMeshSize, individualMeshSize, TextureFormat.RGBA32, false);
            waterMap[i] = new Texture2D(individualMeshWidth, individualMeshHeight, TextureFormat.RGBA32, false);
        }
        
        uvScale = new Vector2(1.0f / (terrainWidth - 1), 1.0f / (terrainWidth - 1));
        waterUvScale = new Vector2(1.0f / (individualMeshWidth - 1), 1.0f / (individualMeshWidth - 1));
        vertsScale = new Vector3(scaleTerrain.x / (terrainWidth - 1), scaleTerrain.y, scaleTerrain.z / (terrainWidth - 1));

        int meshIndex = 0;
        
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {

                for (int z = 0; z < individualMeshHeight; z++)
                {
                    for (int x = 0; x < individualMeshWidth; x++)
                    {
                        try
                        {
                            verticesOut[meshIndex][(z * individualMeshHeight) + x] = vertices[x + individualMeshWidth * j - j, z + individualMeshHeight * i - i];
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            Debug.Log(x + "," + z + " OUT");
                            Debug.Log(meshIndex);
                        }
                        try
                        {
                            verticesOut[meshIndex][(z * individualMeshHeight) + x].Scale(vertsScale);
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            Debug.Log("!");
                        }
                        uv[meshIndex][(z * individualMeshHeight) + x] = Vector2.Scale(new Vector2(x + individualMeshWidth * j - j, z + individualMeshHeight * i - i), uvScale);
                        waterUv[(z * individualMeshHeight) + x] = Vector2.Scale(new Vector2(x, z), waterUvScale);
                        normals[meshIndex][(z * individualMeshHeight) + x] = new Vector3(0, 1, 0);
                    }
                }
                ++meshIndex;
            }
        }
        int index = 0;
        for (int z = 0; z < individualMeshHeight - 1; z++)
        {
            for (int x = 0; x < individualMeshWidth - 1; x++)
            {

                triangles[index++] = (z * (individualMeshHeight)) + x;
                triangles[index++] = ((z + 1) * (individualMeshHeight)) + x;
                triangles[index++] = (z * (individualMeshHeight)) + x + 1;

                triangles[index++] = ((z + 1) * (individualMeshHeight)) + x;
                triangles[index++] = ((z + 1) * (individualMeshHeight)) + x + 1;
                triangles[index++] = (z * (individualMeshHeight)) + x + 1;
            }
        }
        initMeshes();
    }

    /// <summary>
    /// move terrain meshes
    /// </summary>
    public void MoveTerrain(Vector3 center)
    {
        for (int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0; z < terrainWidth; z++)
            {
                vertices[x, z].x = x + center.x - terrainWidth / 2;
                vertices[x, z].z = z + center.z - terrainHeight / 2;
            }
        }
    }


    public void build()
    {
        MoveTerrain(localTerrain.localTerrainC.center);
        
        ApplyLayers();

        //Initialise scaling values according to terrain size and user-controlled sizing
        vertsScale = new Vector3(scaleTerrain.x / (terrainWidth - 1), scaleTerrain.y, scaleTerrain.z / (terrainHeight - 1));

        int meshIndex = 0;

        //for color mapping
        float valueRange = globalTerrain.globalTerrainC.globalMax - globalTerrain.globalTerrainC.globalMin;
        if (valueRange == 0)
            valueRange = 1;
        float minusValue = 0;
        if (globalTerrain.globalTerrainC.globalMin < 0)
            minusValue = Mathf.Abs(globalTerrain.globalTerrainC.globalMin);
        if (riverGenerator.rivers.Count > 0)
            minusValue += 0.1f;

        //Rebuild mesh data
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {

                for (int z = 0; z < individualMeshHeight; z++)
                {
                    for (int x = 0; x < individualMeshWidth; x++)
                    {

                        Vertex gc = localTerrain.GetGlobalCoordinate(x + j * individualMeshWidth, z + i * individualMeshWidth);//global coord
                        Vertex glob_grid_c = gm.GetRealCoordinates(gm.GetGridCoordinates(gc));

                        //Set output vertices
                        verticesOut[meshIndex][(z * individualMeshHeight) + x] = vertices[x + individualMeshWidth * j - j, z + individualMeshHeight * i - i];
                        verticesOut[meshIndex][(z * individualMeshHeight) + x].Scale(vertsScale);

                        //Set normals
                        normals[meshIndex][(z * individualMeshHeight) + x] =
                            getNormalAt(vertices[
                                x + individualMeshWidth * j - j,
                                z + individualMeshHeight * i - i],
                                x + individualMeshWidth * j - j,
                                z + individualMeshHeight * i - i);

                        //Set heightmap texture pixel
                        float this_color = 666;
                        if (debugHeightmap)
                        {
                            this_color = Mathf.Clamp(((vertices[x + j * individualMeshWidth, z + i * individualMeshWidth].y
                            + minusValue) / valueRange) + terrainBrightness, 0.1f, 0.9f);

                            this_color = localTerrain.lm.GetCurrentHeight(gc.x, gc.z);
                            this_color += minusValue;
                            this_color += terrainBrightness;
                            this_color /= valueRange;
                            this_color = Mathf.Clamp(this_color, 0.1f, 0.9f);
                        }
                        else if (debugRmin || debugRmax || debugNoise)
                        {
                            this_color = Mathf.Clamp((pm.GetValue(gc.x, gc.z, PatchInfo.rMin) + Mathf.Abs(rMin_min)) / (rMin_max - rMin_min) + terrainBrightness, 0, 1);

                            this_color = pm.patchLevel.GetValue(glob_grid_c.x, glob_grid_c.z, 0);
                            this_color += 1;
                            this_color /= 3;
                        }
                        
                        int local_x = x + individualMeshWidth * j;
                        int local_z = z + individualMeshHeight * i;

                        if (!colorMode)
                        {
                            heightMap.SetPixel(local_x, local_z,
                              new Color(this_color, this_color, this_color));
                        }
                        

                        if (colorMode)
                        {
                            float secondary = this_color / 2;
                            if (secondary < 0.1)
                                secondary = 0.1f;
                            if(this_color < 0.1)//random
                            {
                                this_color = 0.1f;
                                heightMap.SetPixel(local_x, local_z,
                            new Color(this_color, this_color, this_color));
                            }
                            else if (this_color <= 0.34f)//low
                            {
                                heightMap.SetPixel(local_x, local_z,
                            new Color(secondary, secondary, this_color));
                            }
                            else if (this_color <= 0.67f)//medium
                            {
                                heightMap.SetPixel(local_x, local_z,
                            new Color(secondary, this_color, secondary));
                            }
                            else //high
                            {
                                heightMap.SetPixel(local_x, local_z,
                            new Color(this_color, secondary, secondary));
                            }
                        }

                        //Set water data if water is present
                        if(waterMesh)
                        {
                            int tex = 15;

                            //Set transparency
                            float alpha = W[x + individualMeshWidth * j - j, z + individualMeshHeight * i - i] * 120;
                            alpha = 0.5f;
                            if (alpha > 0.9f) alpha = 1.0f;

                            //Set water texture pixel
                            waterMap[meshIndex].SetPixel(x, z, new Color(0,0,this_color, alpha));

                            //Set water output vertex
                            verticesWater[meshIndex][(z * individualMeshHeight) + x] = vertices[x + individualMeshWidth * j - j, z + individualMeshHeight * i - i];
                            verticesWater[meshIndex][(z * individualMeshHeight) + x].y = W[x + individualMeshWidth * j - j, z + individualMeshHeight * i - i];
                            verticesWater[meshIndex][(z * individualMeshHeight) + x].Scale(vertsScale);
                        }
                        else
                        {
                            //Set water vertex just under the mesh
                            verticesWater[meshIndex][(z * individualMeshHeight) + x] = vertices[x + individualMeshWidth * j - j, z + individualMeshHeight * i - i];
                            verticesWater[meshIndex][(z * individualMeshHeight) + x].y -= 0.02f;
                            verticesWater[meshIndex][(z * individualMeshHeight) + x].Scale(vertsScale);
                        }
                    }
                }
                ++meshIndex;
            }
        }

        counter = 0;

        ///mark axis
        if (markAxis)
        {
            Color markColor = new Color(1, 0, 0);
            for (int x = 0; x <= 30; x++)
            {
                for (int z = 0; z <= 10; z++)
                {
                    heightMap.SetPixel(x, z, new Color(1, 1, 0));
                    heightMap.SetPixel(z, x, new Color(1, 0, 1));
                }
            }
            heightMap.SetPixel(patchWidth, patchHeight, new Color(1, 0, 0));
        }
        

        //Apply changes to heighmap teture
        heightMap.Apply();

        //Assign data structures to the meshes
        for (int i = 0; i < 4; i++)
        {
            myMesh[i].vertices = verticesOut[i];
            myMesh[i].normals = normals[i];
            myMesh[i].RecalculateBounds();

            waterMap[i].Apply();

            myWaterMesh[i].vertices = verticesWater[i];
            myWaterMesh[i].RecalculateNormals();
            myWaterMesh[i].RecalculateBounds();
        }
        //Set bounds
        endOf = verticesOut[3][individualMeshWidth * individualMeshHeight - 1];
        startOf = verticesOut[0][0];
        middleOf = (startOf + endOf) / 2;



    }

    public void ColorPixels()
    {
        Color markColor = new Color(1, 0, 0);
        for (int x = 0; x <= 10; x++)
        {
            for (int z = 0; z <= 10; z++)
            {
                heightMap.SetPixel(x, z, markColor);
            }
        }

        //Apply changes to heighmap teture
        heightMap.Apply();
    }


    private void initMeshes()
    {

        //Function to assign data structures to meshes and link to Renderer


        //Create game objects
        myTerrain = new GameObject[4];
        myWater = new GameObject[4];

        //Initialise terrain and water meshes and link to the data structures
        for (int i = 0; i < 4; i++)
        {

            //TERRAIN
            myMesh[i] = new Mesh();
            myTerrain[i] = new GameObject();
            myTerrain[i].name = "Terrain [" + (int)(i + 1) + "/4]";

            myTerrain[i].AddComponent<MeshFilter>().mesh = myMesh[i];
            myTerrain[i].AddComponent<MeshRenderer>();

            myMesh[i].vertices = verticesOut[i];
            myMesh[i].triangles = triangles;
            myMesh[i].uv = uv[i];
            myMesh[i].normals = normals[i];

            myTerrain[i].GetComponent<Renderer>().material.mainTexture = heightMap;
            myTerrain[i].GetComponent<Renderer>().material.mainTexture.wrapMode = TextureWrapMode.Clamp;

           
            myTerrain[i].GetComponent<Renderer>().material.SetFloat("_Glossiness", 0);
            //Debug.Log(myTerrain[i].GetComponent<Renderer>().material.GetFloat("_Glossiness"));

            //WATER
            myWaterMesh[i] = new Mesh();
            myWater[i] = new GameObject();
            myWater[i].name = "Water [" + (int)(i + 1) + "/4]";

            myWater[i].AddComponent<MeshFilter>().mesh = myWaterMesh[i];
            myWater[i].AddComponent<MeshRenderer>();

            myWaterMesh[i].vertices = verticesWater[i];
            myWaterMesh[i].triangles = triangles;
            myWaterMesh[i].uv = waterUv;
            myWaterMesh[i].RecalculateNormals();

            //28 APRIL 2014  ||  18:30 pm  ||   OCCURED CHANGE DUE TO INCONSISTENCY OF STANDALONE SHADER COMPILER
            //SHADER HAS BEEN ADDED TO MATERIAL IN A NEW RESOURCES FOLDER INSIDE THE ASSETS
            //MATERIAL IS ATTACHED AS FOLLOWS:

            //Apply texture and transparent shader 
            //myWater[i].renderer.material.shader = Shader.Find( "Transparent/BumpedDiffuse" );
            myWater[i].GetComponent<Renderer>().material = Resources.Load("Watermat", typeof(Material)) as Material;

            myWater[i].GetComponent<Renderer>().material.mainTexture = waterMap[i];
            myWater[i].GetComponent<Renderer>().material.mainTexture = waterMap[i];
            myWater[i].GetComponent<Renderer>().material.mainTexture.wrapMode = TextureWrapMode.Clamp;
        }
    }

    public void PrintTerrain()
    {
        Area area = localTerrain.GetVisibleArea();
        string s = area + ": ";
        string undef = "undefined: ";
        string _s = "";
        for(int x = area.botLeft.x; x < area.topRight.x; x++)
        {
            for (int z = area.botLeft.z; z < area.topRight.z; z++)
            {
                float val = localTerrain.globalTerrainC.GetValue(x, z);
                _s = "" + val;
                s += _s.Substring(0, Mathf.Min(_s.Length, 4)) + ",";
                if(val == 666)
                {
                    undef += "[" + x + "," + z + "],";
                }
            }
            s += "| \n";
        }
        Debug.Log(s);
        Debug.Log(undef);
    }


    int patchWidth;
    int patchHeight;

   

    //modulus function
    public int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    /// <summary>
    /// sets height to vertex only if it is not already set
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="height"></param>
    public void SetVertex(int x, int z, float height, bool overwrite)
    {
        if (!overwrite && localTerrain.GetLocalHeight(x, z) != 666)
            return;

        if (CheckBounds(x, z))
        {
            if (vertices[x, z].y == 666)
            {
                vertices[x, z].y = height;
                //localTerrain.SetHeight(x, z, height, false);
            }
            else
            {

            }
        }
        //update values also to global terrain        
        localTerrain.SetLocalHeight(x, z, height, overwrite);
    }

    public void SetVertex(int x, int z, float height)
    {
        SetVertex(x, z, height, false);
    }

    /// <summary>
    /// returns height of vertex
    /// if coordinates are out of bounds, it returns global height
    /// returns 0 if vertex is not defined
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public float GetVertexHeight(int x, int z)
    {

        if (localTerrain.GetLocalHeight(x, z) != 666)
            return localTerrain.GetLocalHeight(x, z);
        else
            return 0;


        if (!CheckBounds(x, z))
        {
            if (globalTerrain.GetHeight(x, z) != 666)
                return globalTerrain.GetHeight(x, z);
            else
                return 0;
        }

        if (vertices[x, z].y == 666)
        {
            return 0;
        }
        else
        {
            return vertices[x, z].y;
        }

    }
    
    public void setTexture(bool flag)
    {

        //Set procedural texture if true, otherwise set heighmap texture

        Debug.Log(flag);
        Debug.Log(heightMap);
        Debug.Log(proceduralTexture);

        if (flag)
            for (int i = 0; i < 4; i++)
                myTerrain[i].GetComponent<Renderer>().material.mainTexture = proceduralTexture;
        else
            for (int i = 0; i < 4; i++)
                myTerrain[i].GetComponent<Renderer>().material.mainTexture = heightMap;
    }

    public void applyProceduralTex(bool sandFlag, Vector3 sandColor, float sandLimit, float sandStrength, float sandCoverage, bool grassFlag, Vector3 grassColor, float grassStrength, bool snowFlag, Vector3 snowColor, float snowLimit, float snowStrength, float snowCoverage, bool slopeFlag, Vector3 slopeColor, float slopeLimit, float slopeStrength, float noiseLimit)
    {

        //Procedural texture main algorithm


        //Create temporary texture map as vectors for increased performance
        //Texture2D tempTexture = new Texture2D(terrainSize, terrainSize);
        Texture2D tempTexture = new Texture2D(terrainWidth, terrainHeight);

        //Color layers index
        int texLayers = 4;

        //Initialise color layers and weights
        Vector3[] layer = new Vector3[texLayers];
        float[] weights = new float[texLayers];

        layer[0] = sandColor; //sand
        layer[1] = grassColor; //grass
        layer[2] = slopeColor; //rock
        layer[3] = snowColor; //snow


        //Iterate through texture matrix
        for (int xVal = 0; xVal < terrainWidth; xVal++)
        {
            for (int zVal = 0; zVal < terrainHeight; zVal++)
            {

                //Initialise weights variables
                float random = UnityEngine.Random.Range(-noiseLimit, noiseLimit); //random noise
                float height = GetVertexHeight(xVal, zVal); //vertices[xVal, yVal].y;                //height at current point
                float steepness = 1.0f - findSlope(xVal, zVal);         //get inverted slope value

                //Clamp slope to 0
                if (steepness < 0) steepness = 0;


                //Compute weights

                //Sand weight
                if (height < sandLimit && steepness < sandCoverage && sandFlag)
                    weights[0] = ((sandLimit - height) * sandStrength) + UnityEngine.Random.Range(-(noiseLimit - 0.1f), (noiseLimit - 0.1f));
                else
                    weights[0] = 0;

                //Grass weight
                if (grassFlag)
                    weights[1] = random + (1.0f - height) * grassStrength;
                else
                    weights[1] = 0;

                //Slopes/Rocks weight
                if (slopeFlag && (steepness > slopeLimit))
                    weights[2] = (steepness) * slopeStrength + random;
                else
                    weights[2] = 0;

                //Snow weight
                if (height > snowLimit && steepness < snowCoverage && snowFlag)
                    weights[3] = ((height - snowLimit) * snowStrength) + UnityEngine.Random.Range(-(noiseLimit - 0.15f), (noiseLimit - 0.15f));
                else
                    weights[3] = 0;

                //Average the values
                float sum = (weights[0] + weights[1] + weights[2] + weights[3]);
                Vector3 finalColor = new Vector3(0, 0, 0);

                //Multiply by normalised weights
                for (int i = 0; i < texLayers; i++)
                    finalColor += layer[i] * (weights[i] / sum);

                //Set final pixel color
                tempTexture.SetPixel(xVal, zVal, new Color(finalColor.x, finalColor.y, finalColor.z, 0.0f));
            }
        }
        tempTexture.Apply();

        //Cross-neighbours averaging for smoother transitions between colors
        for (int xVal = 0; xVal < terrainWidth; xVal++)
        {
            for (int zVal = 0; zVal < terrainHeight; zVal++)
            {

                Color allCol = new Color(0, 0, 0, 0);
                int index = 0;

                if (xVal > 0 && zVal > 0)
                {
                    allCol += tempTexture.GetPixel(xVal - 1, zVal - 1);
                    index++;
                }

                if (xVal > 0 && zVal < terrainWidth - 1)
                {
                    allCol = tempTexture.GetPixel(xVal - 1, zVal + 1);
                    index++;
                }

                if (xVal < terrainWidth - 1 && zVal < terrainHeight - 1)
                {
                    allCol = tempTexture.GetPixel(xVal + 1, zVal + 1);
                    index++;
                }

                if (xVal < terrainWidth - 1 && zVal > 0)
                {
                    allCol = tempTexture.GetPixel(xVal + 1, zVal - 1);
                    index++;
                }

                //Averaging
                Color finalColor = allCol / index;
                //Setting final pixel color
                proceduralTexture.SetPixel(xVal, zVal, finalColor);
            }
        }
        proceduralTexture.Apply();
    }

    
    //HELPER FUNCTIONS
    public void ColorPixel(int x, int z, int offset, Color color)
    {
        for (int _x = x - offset; _x <= x + offset; _x++)
        {
            for (int _z = z - offset; _z <= z + offset; _z++)
            {
                if (CheckBounds(x, z))
                    heightMap.SetPixel(_x, _z, color);
            }
        }

        heightMap.Apply();
    }
    public bool CheckBounds(int x, int z)
    {
        return x >= 0 && x < terrainWidth && z >= 0 && z < terrainHeight;
    }

    private Vector3 getNormalAt(Vector3 vertex, int x, int z)
    {

        //Function to return normal at vertex

        //Initialise neighboring values
        Vector3 n1 = new Vector3(0, 0, 0);
        Vector3 n2 = new Vector3(0, 0, 0);
        Vector3 n3 = new Vector3(0, 0, 0);
        Vector3 n4 = new Vector3(0, 0, 0);

        float coef = 0;
        //int segments = meshSize;
        int segmentsX = terrainWidth;
        int segmentsZ = terrainHeight;

        //Scale to renderable output
        Vector3 A = vertices[x, z];
        A.Scale(vertsScale);

        //Clamp to edge 
        if (x > 0 && z < segmentsZ)
        {
            //Get neigbors
            Vector3 B = vertices[x - 1, z];
            Vector3 C = vertices[x, z + 1];

            //Set neighbors to renderable scale
            B.Scale(vertsScale);
            C.Scale(vertsScale);

            //Get the normalised cross-product and increment index
            n1 = Vector3.Cross(B - A, C - A).normalized;

            coef += 1;
        }
        //Clamp to edge 
        if (x < segmentsX && z < segmentsZ)
        {
            //Get neigbors
            Vector3 C = vertices[x, z + 1];
            Vector3 D = vertices[x + 1, z];

            //Set neighbors to renderable scale
            C.Scale(vertsScale);
            D.Scale(vertsScale);

            //Get the normalised cross-product and increment index
            n2 = Vector3.Cross(C - A, D - A).normalized;
            coef += 1;
        }
        //Clamp to edge 
        if (x < segmentsX && z > 0)
        {
            //Get neigbors
            Vector3 D = vertices[x + 1, z];
            Vector3 E = vertices[x, z - 1];

            //Set neighbors to renderable scale
            D.Scale(vertsScale);
            E.Scale(vertsScale);

            //Get the normalised cross-product and increment index
            n3 = Vector3.Cross(D - A, E - A).normalized;
            coef += 1;
        }
        //Clamp to edge 
        if (x > 0 && z > 0)
        {
            //Get neigbors
            Vector3 E = vertices[x, z - 1];
            Vector3 B = vertices[x - 1, z];

            //Set neighbors to renderable scale
            E.Scale(vertsScale);
            B.Scale(vertsScale);

            //Get the normalised cross-product and increment index
            n4 = Vector3.Cross(E - A, B - A).normalized;
            coef += 1;
        }

        //Return normal
        return new Vector3(
            (n1.x + n2.x + n3.x + n4.x) / coef,
            (n1.y + n2.y + n3.y + n4.y) / coef,
            (n1.z + n2.z + n3.z + n4.z) / coef);
    }

    public Vector3 collisionCheck(Vector3 objPosition)
    {

        //Check for collision and return new height
        //Simulating the sliding over terrain

        //Copy object position
        Vector3 newPos = objPosition;

        //Scale position to fit normalised vertices[,] map
        objPosition.x -= startOf.x;
        objPosition.z -= startOf.z;

        objPosition.x /= vertsScale.x;
        objPosition.y /= vertsScale.y;
        objPosition.z /= vertsScale.z;

        if (objPosition.x > terrainWidth - 1) objPosition.x = terrainWidth - 1;
        else
            if (objPosition.x < 0) objPosition.x = 0;

        if (objPosition.y > terrainHeight - 1) objPosition.y = terrainHeight - 1;
        else
            if (objPosition.y < 0) objPosition.y = 0;

        //Get height at current point
        float terrHeight = GetVertexHeight((int)objPosition.x, (int)objPosition.z);// vertices[(int)objPosition.x, (int)objPosition.z].y;

        //Check heights and set new height if collision occurs
        if (objPosition.y < terrHeight) newPos.y = terrHeight * vertsScale.y;

        //Return new position
        return newPos;
    }


    private float findSlope(int xVal, int zVal)
    {

        //Find the slope at position xVal, yVal

        float[] neighbH = new float[4];

        //Find neighbours
        if (xVal > 0)
            neighbH[0] = GetVertexHeight(xVal - 1, zVal);// vertices[xVal - 1, yVal].y;
        else
            neighbH[0] = GetVertexHeight(xVal, zVal);// vertices[xVal, yVal].y;

        if (xVal < terrainWidth - 1)
            neighbH[1] = GetVertexHeight(xVal + 1, zVal);// vertices[xVal + 1, yVal].y;
        else
            neighbH[1] = GetVertexHeight(xVal, zVal);// vertices[xVal, yVal].y;

        if (zVal > 0)
            neighbH[2] = GetVertexHeight(xVal, zVal - 1);// vertices[xVal, yVal - 1].y;
        else
            neighbH[2] = GetVertexHeight(xVal, zVal);//vertices[xVal, yVal].y;

        if (zVal < terrainHeight - 1)
            neighbH[3] = GetVertexHeight(xVal, zVal + 1);// vertices[xVal, yVal + 1].y;
        else
            neighbH[3] = GetVertexHeight(xVal, zVal);// vertices[xVal, yVal].y;

        //Find normal
        Vector3 va = new Vector3(1.0f, 0.0f, neighbH[1] - neighbH[0]);
        Vector3 vb = new Vector3(0.0f, 1.0f, neighbH[3] - neighbH[2]);
        Vector3 n = Vector3.Cross(va.normalized, vb.normalized);

        //Return dot product of normal with the Y axis
        return Mathf.Max(0.05f, 1.0f - Mathf.Abs(Vector3.Dot(n, new Vector3(0, 1, 0))));

    }
    
    /// <summary>
    /// PARAMETERS I DONT NEED...
    /// </summary>

    Vector3 sandColor = new Vector3(0.90f, 0.90f, 0.00f);  // Sand r,g,b
    Vector3 grassColor = new Vector3(0.00f, 0.70f, 0.00f); // Grass r,g,b
    Vector3 rockColor = new Vector3(0.20f, 0.05f, 0.00f); // Rock r,g,b
    Vector3 snowColor = new Vector3(1.00f, 1.00f, 1.00f); // Snow r,g,b

    float sandLimit = 0.3f;     // Sand maximum altitude
    float sandStrength = 7.0f;  // Sand color multiplier
    float sandCoverage = 0.02f; // Sand maximum slope size
    float grassStrength = 1.0f; // Grass color multiplier
    float snowLimit = 0.55f;    // Snow minimum altitude
    float snowStrength = 6.0f;  // Snow color multiplier
    float snowCoverage = 0.02f; // Snow minumum slope size
    float slopeLimit = 0.0f;    // Rock altitude
    float slopeStrength = 6.0f; // Rock multiplier
    float noiseTexValue = 0.2f; // Random noise range

    //Terrain object and mesh
    public GameObject[] myTerrain;
    public Mesh[] myMesh;

    //Water object and mesh
    public GameObject[] myWater;
    public Mesh[] myWaterMesh;

    //Textures
    public Texture2D heightMap;
    public Texture2D[] waterMap;
    public Texture2D proceduralTexture;

    //Mesh limits 
    public Vector3 endOf;
    public Vector3 startOf;
    public Vector3 middleOf;

    //Terrain sizes
    //public int terrainSize;
    //public int patchSize;
    //int patchCount;
    //int meshSize;
    //int individualMeshSize;

    Vector2 uvScale;
    Vector3 vertsScale;
    Vector2 waterUvScale;

    //Meshes data
    public Vector3[][] verticesWater;
    public Vector3[,] vertices;
    public Vector3[][] verticesOut;

    Vector2[] waterUv;
    Vector2[][] uv;
    int[] triangles;
    public Vector3[][] normals;
    

    //Hydraulic erosion maps
    public float[,] W; //water
    public float[,] S; //sediment
    public Vector2[,] V; //velocity
    public Vector4[,] F; //outflow flux


    //Wind parameters
    Vector2 windStrength = new Vector2(0.0f, 0.0f);
    float windCoverage = 1.0f;
    bool windAltitude = true;
    
}

public enum TerrainType
{
    real,
    constant,
    gradientX_lr,
    gradientX_rl,
    gradientZ_lr,
    gradientZ_rl,
    gradient_radialPlus,
    gradient_radialMinus,
    river
}
