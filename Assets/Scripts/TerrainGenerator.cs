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
        //MoveTerrain(cameraPosition);
        /*
        for(int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0; z < terrainHeight; z++)
            {
                vertices[x, z].y = localTerrain.GetGlobalHeight(x, z); //localTerrain.visibleTerrain[x, z];
            }
        }*/
        AssignHeightsToVertices();

        build();
    }


    /*
    GlobalCoordinates rMinValues = new GlobalCoordinates(100);
    void SetRminValues(Vertex center, int patchSize, float value)
    {
        for (int x = center.x - patchSize/2; x < center.x + patchSize/2; x++)
        {
            for (int z = center.z - patchSize / 2; z < center.z + patchSize / 2; z++)
            {
                rMinValues.SetValue(x, z, value);
            }
        }
    }*/

    public int extraPatchCount = 0;
    //// <summary>
    /// checks all points on grid in visible area are defined
    /// if not, regions with centers in points on the grid is generated
    /// </summary>
    public void PregenerateRegions(Vertex center, Area visibleArea, int patchSize, int extraPatchCount)
    {
        Vertex centerOnGrid = gm.GetPointOnGrid(center);
        Area surroundingArea = fmc.GetSurroundingAreaOf(centerOnGrid.Clone(), visibleArea, patchSize, extraPatchCount);
        //Debug.Log("patchSize: " + patchSize);

        //Debug.Log("-----------------------");
        //Debug.Log("center: " + center);
        //Debug.Log("centerOnGrid: " + centerOnGrid);
        //Debug.Log("visibleArea: " + visibleArea);
        //Debug.Log("surroundingArea: " + surroundingArea);

        extraPatchCount = 1; //set to 1 after first generation
        

        int x_min = (int)surroundingArea.botLeft.x;
        int z_min = (int)surroundingArea.botLeft.z;
        int x_max = (int)surroundingArea.topRight.x;
        int z_max = (int)surroundingArea.topRight.z;


        localTerrain.UpdateSize(patchSize, patchSize);

        int _x;
        int _z;
        Vertex tmpCenter;

        /*List<PatchLevel> patchOrder = new List<PatchLevel>();

        patchOrder.Add(PatchLevel.low);
        patchOrder.Add(PatchLevel.medium);
        patchOrder.Add(PatchLevel.high);
        patchOrder.Add(PatchLevel.random);*/

        if (pm.patchLevel.GetValue(0,0) == -1)
        {
            //...
        }

        //for (int i = 0; i <= 3; i++)//0 = low, 1 = medium, 2 = high, 3 = random,

        //Debug.Log("generating on: " + surroundingArea);
        bool debug = false;

        foreach (PatchLevel i in pm.patchOrder)
        {
            //for (int x = -count; x <= count; x++)
            //{
            //    for (int z = -count; z <= count; z++)
            //    {
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

                        _x = centerOnGrid.x + x * patchSize;
                        _z = centerOnGrid.z + z * patchSize;
                        _x = x;
                        _z = z;

                        //pm.patchLevel.SetValue(_x, _z, 1);

                        int level = (int)pm.patchLevel.GetValue(_x, _z, -1);


                        rMin = pm.rMin.GetValue(_x, _z, -1);
                        rMax = pm.rMax.GetValue(_x, _z, 1);
                        noise = pm.noise.GetValue(_x, _z, 2);

                        //Debug.Log(x + "," + z);
                        //Debug.Log("rMin:"+ rMin);
                        //Debug.Log("rMax:" + rMax);
                        //Debug.Log("noise:" + noise);


                        tmpCenter = new Vertex(_x, _z);
                        if (i == PatchLevel.low && level == 0)
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
                            //pm.SetValues(tmpCenter, patchSize, rMin, rMax, noise);
                        }

                        if (i == PatchLevel.random && level == -1)
                        {
                            if (debug)
                            {
                                Debug.Log(x + "," + z);
                                Debug.Log(i);
                            }
                            //Debug.Log(x + "," + z);

                            Vertex patchC = gm.GetGridCoordinates(new Vertex(x, z));

                            gtp.SetPatchValue(patchC.x, patchC.z, PatchLevel.random);
                            rMin = pm.rMin.GetValue(_x, _z, -1);
                            rMax = pm.rMax.GetValue(_x, _z, 1);
                            noise = pm.noise.GetValue(_x, _z, 2);

                            //Debug.Log(rMin);
                            //Debug.Log(rMax);
                            //Debug.Log(noise);

                            /*rMin = pm.GetNeighbourAverage(_x, _z, PatchInfo.rMin);
                            rMax = pm.GetNeighbourAverage(_x, _z, PatchInfo.rMax);
                            noise = pm.GetNeighbourAverage(_x, _z, PatchInfo.noise);
                            

                            if (rMin == 666)
                                rMin = -1;
                            if (rMax == 666)
                                rMax = 1;
                            if (noise == 666)
                                noise = 2;

                            if (rMax <= rMin)
                                rMax = rMin + 0.5f;*/

                            //pm.SetValues(new Vertex(x, z), patchSize, rMin, rMax, noise);

                            localTerrain.MoveVisibleTerrain(tmpCenter, false);
                            ds.Initialize(patchSize, noise, rMin, rMax);
                            //pm.SetValues(tmpCenter, patchSize, rMin, rMax, noise);
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
    public void GenerateTerrainOn(Vector3 center, bool defaultTerrain)//Vector3 botLeft, Vector3 topRight)
    {
        ///functional RANDOM terrin generator
        //rt.GenerateRandomTerrain(botLeft, topRight);

        //
        //defaultTerrain = true;

        if (defaultTerrain)
            GenerateDefaultTerrain(TerrainType.gradient_radialMinus, terrainWidth); //can't generate river first!
        else
            PregenerateRegions(center, localTerrain.GetVisibleArea(), patchSize, extraPatchCount);

        //ds.Initialize();

        //localTerrain.MoveVisibleTerrain(center);


        //filterGenerator.PerserveMountains(3, 50, 10);

        //AssignHeightsToVertices();

        

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
        //float value;

        Vertex globalC;
        /*layers = new List<Layer>();
        if (terrainLayer)
            layers.Add(Layer.terrain);
        if (riverLayer)
            layers.Add(Layer.river);

        if (filterAverageLayer)
            layers.Add(Layer.filterAverage);
        if (filterMedianLayer)
            layers.Add(Layer.filterMedian);
        if (filterSpikeLayer)
            layers.Add(Layer.filterSpike);
        if (filterGaussianLayer)
            layers.Add(Layer.filterGaussian);
        if (filterMinThresholdLayer)
            layers.Add(Layer.filterMinThreshold);
        if (filterMaxThresholdLayer)
            layers.Add(Layer.filterMaxThreshold);


        //if (erosionHydraulicWaterLayer)
            //layers.Add(Layer.erosionHydraulicWater);
        if (erosionHydraulicLayer)
            layers.Add(Layer.erosionHydraulic);
        if (erosionThermalLayer)
            layers.Add(Layer.erosionThermal);
            */

        localTerrain.lm.UpdateLayers();

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0; z < terrainHeight; z++)
            {
                globalC = localTerrain.GetGlobalCoordinate(x, z);
                //vertices[x, z].y = localTerrain.lm.GetValueFromLayers(globalC.x, globalC.z, layers);
                vertices[x, z].y = localTerrain.lm.GetValueFromLayers(
                    globalC.x, globalC.z, localTerrain.lm.activeLayers);
                W[x, z] = erosionGenerator.he.GetTerrainWatterValue(globalC.x, globalC.z);
            }
        }
    }

    //CLASS AND OBJECTS INITIALISATION

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

        //Class Constructor
        //Initialising all data and setting variables to null value


        //patchSize = patch_size;  //the size of a single patch of terrain
        //patchCount = patch_count;//the number of patches valueXvalue

        //The terrain size is built with a row of quads between the patches, to avoid vertex overlapping
        //while the meshSize is the renderable 2^i X 2^i mesh.
        //terrainSize = patchSize * patchCount + patchCount;
        terrainWidth = localTerrain.terrainWidth;
        terrainHeight = localTerrain.terrainHeight;

        scaleTerrain = new Vector3(terrainWidth, scaleY, terrainHeight);
        //meshSize =  patchSize * patchCount + 1;
        int meshSizeX = 32 * 2 + 1;
        int meshSizeZ = 64 * 2 + 1;
        //individualMeshSize = meshSize / 2 + 1;
        individualMeshWidth = terrainWidth / 2;
        individualMeshHeight = terrainHeight / 2;

        //Textures initialisation
        //heightMap = new Texture2D(meshSize, meshSize, TextureFormat.RGBA32, false);
        //waterMap = new Texture2D[4];
        //proceduralTexture = new Texture2D(terrainSize, terrainSize);

        heightMap = new Texture2D(terrainWidth, terrainHeight, TextureFormat.RGBA32, false);
        waterMap = new Texture2D[4];
        proceduralTexture = new Texture2D(terrainWidth, terrainHeight);

        //Meshes initialisation. 4 Meshes are built for each due to 
        //Unity's inability to hold more than 65 000 vertices/mesh
        myMesh = new Mesh[4];
        myWaterMesh = new Mesh[4];



        //The offset considered when generating on-the-fly geometry
        blurOffset = 2;
        thermalOffset = 5;

        //Init hydraulic erosion maps
        //W = new float[terrainSize, terrainSize]; //water map
        //S = new float[terrainSize, terrainSize]; //sediment map
        //V = new Vector2[terrainSize, terrainSize]; //velocity map
        //F = new Vector4[terrainSize, terrainSize]; //outflow map

        W = new float[terrainWidth, terrainHeight]; //water map
        S = new float[terrainWidth, terrainHeight]; //sediment map
        V = new Vector2[terrainWidth, terrainHeight]; //velocity map
        F = new Vector4[terrainWidth, terrainHeight]; //outflow map

        //Mesh values
        //can't be used...mesh will get freaky
        //int individualMeshSize = Mathf.Max(individualMeshWidth, individualMeshHeight);


        int numVerts = (individualMeshWidth + 1) * (individualMeshHeight + 1);
        int numQuads = (individualMeshWidth - 1) * (individualMeshHeight - 1);
        int numTriangles = numQuads * 2;

        //Mesh data
        //vertices = new Vector3[terrainSize, individualMeshHeight];
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

        //Initialising scaling factors according to mesh sizes
        //uvScale = new Vector2(1.0f / (terrainSize - 1), 1.0f / (terrainSize - 1));
        //waterUvScale = new Vector2(1.0f / (individualMeshSize - 1), 1.0f / (individualMeshSize - 1));
        //vertsScale = new Vector3(scaleTerrain.x / (terrainSize - 1), scaleTerrain.y, scaleTerrain.z / (terrainSize - 1));
        uvScale = new Vector2(1.0f / (terrainWidth - 1), 1.0f / (terrainWidth - 1));
        waterUvScale = new Vector2(1.0f / (individualMeshWidth - 1), 1.0f / (individualMeshWidth - 1));
        vertsScale = new Vector3(scaleTerrain.x / (terrainWidth - 1), scaleTerrain.y, scaleTerrain.z / (terrainWidth - 1));

        //Initialising primary height map to null
        //for (int z = 0; z < terrainSize; z++)
        //    for (int x = 0; x < terrainSize; x++)
        //        vertices[x, z] = new Vector3(x, 0, z);

        //no need
        /*
        for (int z = 0; z < terrainHeight; z++)
            for (int x = 0; x < terrainWidth; x++)
                vertices[x, z] = new Vector3(x, 0, z);
        */

        int meshIndex = 0;

        //Build vertices, normals and uv's for each of the four meshes
        //for (int i = 0; i < 2; i++)
        //{
        //    for (int j = 0; j < 2; j++)
        //    {

        //        for (int z = 0; z < individualMeshSize; z++)
        //        {
        //            for (int x = 0; x < individualMeshSize; x++)
        //            {

        //                verticesOut[meshIndex][(z * individualMeshSize) + x] = vertices[x + individualMeshSize * j - j, z + individualMeshSize * i - i];
        //                verticesOut[meshIndex][(z * individualMeshSize) + x].Scale(vertsScale);
        //                uv[meshIndex][(z * individualMeshSize) + x] = Vector2.Scale(new Vector2(x + individualMeshSize * j - j, z + individualMeshSize * i - i), uvScale);
        //                waterUv[(z * individualMeshSize) + x] = Vector2.Scale(new Vector2(x, z), waterUvScale);
        //                normals[meshIndex][(z * individualMeshSize) + x] = new Vector3(0, 1, 0);
        //            }
        //        }
        //        ++meshIndex;
        //    }
        //}
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
                            //Vector3 p = verticesOut[meshIndex][(z * individualMeshHeight) + x]; //????
                            //Vector3 r = vertices[x + individualMeshWidth * j - j, z + individualMeshHeight * i - i]; //????
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


        //Build triangles, used for both meshes
        int index = 0;

        //for (int z = 0; z < individualMeshSize - 1; z++)
        //{
        //    for (int x = 0; x < individualMeshSize - 1; x++)
        //    {

        //        triangles[index++] = (z * (individualMeshSize)) + x;
        //        triangles[index++] = ((z + 1) * (individualMeshSize)) + x;
        //        triangles[index++] = (z * (individualMeshSize)) + x + 1;

        //        triangles[index++] = ((z + 1) * (individualMeshSize)) + x;
        //        triangles[index++] = ((z + 1) * (individualMeshSize)) + x + 1;
        //        triangles[index++] = (z * (individualMeshSize)) + x + 1;
        //    }
        //}
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

        //Call function to assign data structures to the meshes
        initMeshes();
        //Initialise hydraulic erosion model's maps


        //erosionManager = new ErosionManager(this);
        //erosionManager.initHydraulicMaps();
    }

    /// <summary>
    /// move meshes
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

        //move terrain
        //Debug.Log("move to " + localTerrain.center);
        MoveTerrain(localTerrain.localTerrainC.center);
        

        ApplyLayers();

        //Function called to update the renderables when changes occur

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
        //Debug.Log("----");
        //Debug.Log(minusValue);
        //Debug.Log(valueRange);
        //valueRange += terrainBrightness;

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
                        //float this_color = vertices[x + individualMeshWidth * j, z + 
                        //individualMeshHeight * i].y;
                        float this_color = 666;
                        if (debugHeightmap)
                        {
                            this_color = Mathf.Clamp(((vertices[x + j * individualMeshWidth, z + i * individualMeshWidth].y
                            + minusValue) / valueRange) + terrainBrightness, 0.1f, 0.9f);

                            this_color = localTerrain.lm.GetCurrentHeight(gc.x, gc.z);
                                //localTerrain.GetLocalHeight(x + j * individualMeshWidth, z + i * individualMeshWidth);
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

                        /*else if (debugRmax)
                            this_color = Mathf.Clamp((pm.GetValue(gc.x, gc.z, PatchInfo.rMax))/ (rMax_max - rMax_min) + terrainBrightness, 0,1);
                        else if (debugNoise)
                            this_color = Mathf.Clamp((pm.GetValue(gc.x, gc.z, PatchInfo.noise)) / (noise_max - noise_min) + terrainBrightness,0,1);
                            */
                        /*
                        if(gc.x == 0 && gc.z == 0)
                        {
                            Debug.Log(gc);
                            Debug.Log(this_color);
                        }
                        if (gc.x == 64 && gc.z == 0)
                        {
                            Debug.Log(gc);
                            Debug.Log(this_color);
                        }*/

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
                            {/*
                                if(counter < 10)
                                {
                                    counter++;
                                    Debug.Log("-----");
                                    Debug.Log(this_color);
                                }*/
                                heightMap.SetPixel(local_x, local_z,
                            new Color(this_color, secondary, secondary));
                            }
                        }

                        //Set water data if water is present
                        //if (W[x + individualMeshWidth * j - j, z + individualMeshHeight * i - i] > 0.0001f)
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
        //ColorPixels();

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

    /*
    private void initDiamondSquare(Vector3 start, float scale)
    {
        //Debug.Log(start);
        //Debug.Log(scale);
        //Debug.Log(ss);
        //Debug.Log(sd);
        //Main diamond-square algorithm

        //Size of step at iteration 0
        //int stepSize = patchSize;
        int stepX = patchWidth;
        int stepZ = patchHeight;

        //Random numbers limit (-value, +value)
        float rand_value1 = 0.50f;
        float rand_value2 = 0.40f;

        //int x = 0;
        //int y = 0;

        float offset;

        int start_x = (int)start.x;
        int start_z = (int)start.z;
        

        //Start the main displacement loop
        //while (stepSize > 1)
        while (stepX > 1 && stepZ > 1)
        {
            //Debug.Log("step:" + stepSize);            //Halving the resolution each step


            //int half_step = stepSize / 2;
            int half_stepX = stepX / 2;
            int half_stepZ = stepZ / 2;

            
            //Halving the resolution and the roughness parameter
            for (int x = start_x + half_stepX; x < start_x + patchWidth + half_stepX; x = x + stepX)
            {
                for (int z = start_z + half_stepZ; z < start_z + patchWidth+ half_stepZ; z = z + stepZ)
                {
                    stepSquare(x, z, rand_value1, scale, half_stepX, half_stepZ);
                }
            }

            //Diamond step
            for (int x = start_x + half_stepX; x < start_x + patchWidth + half_stepX; x = x + stepX)
            {
                for (int z = start_z + half_stepZ; z < start_z + patchWidth+ half_stepZ; z = z + stepZ)
                {
                    stepDiamond(x, z, rand_value2, scale, half_stepX, half_stepZ, start);
                }
            }

            //REVERSE
            
            for (int x = start_x + patchWidth + half_stepX; x > start_x + half_stepX; x -= stepX)
            {
                for (int z = start_z + patchWidth + half_stepZ; z > start_z + half_stepZ; z -= stepZ)
                {
                    stepSquare(x, z, rand_value1, scale, half_stepX, half_stepZ);
                }
            }

            //Diamond step
            for (int x = start_x + patchWidth + half_stepX; x > start_x + half_stepX; x -= stepX)
            {
                for (int z = start_z + patchWidth + half_stepZ; z > start_z + half_stepZ; z -= stepZ)
                {
                    stepDiamond(x, z, rand_value2, scale, half_stepX, half_stepZ, start);
                }
            }
            

            //Halving the resolution and the roughness parameter
            stepX /= 2;
            stepZ /= 2;
            scale /= 2;

        }
        

        // TODO 
        //Copy margin values to neighbouring vertices belonging to nearby pathes 
        //to avoid unwanted artifacts/seams between patches
        
        //west
        if (start_x != 0)
            for (int i = start_z; i < start_z + patchHeight + 1; i++)
                SetVertex(start_x - 1, i, GetVertexHeight(start_x, i));
        //vertices[start_x - 1, i].y = vertices[start_x, i].y;
        //south
        if (start_z != 0)
            for (int i = start_x; i < start_x + patchWidth + 1; i++)
                SetVertex(i, start_z - 1, GetVertexHeight(i, start_z));
        //vertices[i, start_z - 1].y = vertices[i, start_z].y;
        //east
        if (start_x + patchWidth != terrainWidth - 1)
            for (int i = start_z; i < start_z + patchHeight + 1; i++)
                SetVertex(start_x + patchWidth + 1, i, GetVertexHeight(start_x + patchWidth, i));
        //vertices[start_x + patchSize + 1, i].y = vertices[start_x + patchSize, i].y;
        //north
        if (start_z + patchHeight != terrainHeight - 1)
            for (int i = start_x; i < start_x + patchWidth + 1; i++)
                SetVertex(i, start_z + patchHeight + 1, GetVertexHeight(i, start_z + patchHeight));
        

        //vertices[i, start_z + patchSize + 1].y = vertices[i, start_z + patchSize].y;
    }
    int ss = 0;
    private void stepSquare(int x, int z, float rand_value, float scale, int half_stepX, int half_stepZ)
    {
        //Debug.Log("stepSquare"); 
        ss++;
        //Get corner valuesorners
        float a = GetVertexHeight(x - half_stepX, z - half_stepZ); //vertices[x - half_step, y - half_step].y;
        float b = GetVertexHeight(x + half_stepX, z - half_stepZ); //vertices[x + half_step, y - half_step].y;
        float c = GetVertexHeight(x - half_stepX, z + half_stepZ); //vertices[x - half_step, y + half_step].y;
        float d = GetVertexHeight(x + half_stepX, z + half_stepZ); //vertices[x + half_step, y + half_step].y;

        //if (x > 50 && x < 100 && y > 50 && y < 100)
        //    scale = 0;

        //Set new averaged and randomised value of the centre
        /*
        if (localTerrain.GetHeight(x, y) == 666)
        { //set only if not defined (y = z...)
            vertices[x, y].y = (a + b + c + d) / 4.0f + UnityEngine.Random.Range(-rand_value / 3, rand_value) * scale;
        }
        SetVertex(x, z, (a + b + c + d) / 4.0f + UnityEngine.Random.Range(-rand_value / 3, rand_value) * scale);
    }
    int sd = 0;
    private void stepDiamond(int x, int z, float rand_value, float scale, int half_stepX, int half_stepZ, Vector3 start)
    {
        sd++;
        //Get side points (diamond-shaped)
        float a = GetVertexHeight(x - half_stepX, z - half_stepZ); //vertices[x - half_step, y - half_step].y;
        float b = GetVertexHeight(x + half_stepX, z - half_stepZ); //vertices[x + half_step, y - half_step].y;
        float c = GetVertexHeight(x - half_stepX, z + half_stepZ); //vertices[x - half_step, y + half_step].y;
        float d = GetVertexHeight(x + half_stepX, z + half_stepZ); //vertices[x + half_step, y + half_step].y;

        float offset;
        offset = (a + c) / 2.0f + UnityEngine.Random.Range(-rand_value, rand_value) * scale;
        SetVertex(x - half_stepX, z, offset, true);
        offset = (c + d) / 2.0f + UnityEngine.Random.Range(-rand_value, rand_value) * scale;
        SetVertex(x, z + half_stepZ, offset);
        offset = (b + d) / 2.0f + UnityEngine.Random.Range(-rand_value, rand_value) * scale;
        SetVertex(x + half_stepX, z, offset);
        offset = (a + b) / 2.0f + UnityEngine.Random.Range(-rand_value, rand_value) * scale;
        SetVertex(x, z - half_stepZ, offset);
        /*float offset;
        if (a_corner || d_corner || x - half_stepX != (int)start.x)
        {
            offset = (a + c) / 2.0f + UnityEngine.Random.Range(-rand_value, rand_value) * scale;
            SetVertex(x - half_stepX, z, offset);
        }

        if (c_corner || d_corner || z + half_stepZ != (int)start.z + patchHeight)
        {
            offset = (c + d) / 2.0f + UnityEngine.Random.Range(-rand_value, rand_value) * scale;
            SetVertex(x, z + half_stepZ, offset);
        }

        if (b_corner || c_corner || x + half_stepX != (int)start.x + patchWidth)
        {
            offset = (b + d) / 2.0f + UnityEngine.Random.Range(-rand_value, rand_value) * scale;
            SetVertex(x + half_stepX, z, offset);
        }

        if (a_corner || b_corner || z - half_stepZ != (int)start.z)
        {
            offset = (a + b) / 2.0f + UnityEngine.Random.Range(-rand_value, rand_value) * scale;
            SetVertex(x, z - half_stepZ, offset);
        }
    }
    */

    //PROCEDURAL TEXTURES MODEL

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




    //MESH CONTROL  /  'ON-THE-FLY' CONTENT GENERATION
    /*
    public void goNorth(bool diSqFlag, float diSqScale, bool blurFlag, float blurring_factor, int kernel_size, bool thermalFlag, int thermalIterations, float thermalSlope, float thermalC)
    {

        //Displace vertices
        for (int z = 0; z < terrainHeight - patchHeight - 1; z++)
        {
            for (int x = 0; x < terrainWidth; x++)
            {
                vertices[x, z] = vertices[x, z + patchSize];
            }
        }

        //Initialise new vertices
        for (int z = terrainSize - patchSize - 1; z < terrainSize; z++)
        {
            for (int x = 0; x < terrainSize; x++)
            {
                vertices[x, z].y = 0;
                vertices[x, z].z += patchSize;
            }
        }

        //Apply Diamond-Square method to new patches
        if (diSqFlag)
        {

            //Copy neighbouring values
            for (int x = 0; x < terrainSize; x++)
                vertices[x, terrainSize - patchSize - 1].y = vertices[x, terrainSize - patchSize - 2].y;

            //Set welding flags
            a_corner = b_corner = c_corner = d_corner = true;

            a_corner = false;
            b_corner = false;

            for (int x = 0; x < patchCount; x++)
            {

                //Further set welding flags
                if (x > 0)
                {
                    a_corner = b_corner = c_corner = d_corner = true;

                    a_corner = false;
                    d_corner = false;
                    b_corner = false;
                }

                //Set starting point and call Diamond-Square algorithm
                Vector3 temp = new Vector3(x * patchSize + x, 0, (patchCount - 1) * patchSize + (patchCount - 1));
                initDiamondSquare(temp, diSqScale);
            }
        }

        //Apply Thermal Erosion method to new patches
        if (thermalFlag)
        {

            //erosionManager.localThermalErosion(new Vector3(0, 0, (patchCount - 1) * patchSize + (patchCount - 1) - thermalOffset), new Vector3(terrainSize - 1, 0, terrainSize - 1), thermalIterations, thermalSlope, thermalC);
           // filterManager.applySpikesFilter(0.005f);
        }

        if (blurFlag)
            //filterManager.applyGaussianBlur(blurring_factor, kernel_size, new Vector3(0, 0, (patchCount - 1) * patchSize + (patchCount - 1) - blurOffset), new Vector3(terrainSize - 1, 0, terrainSize - 1));

        build();
    }

    public void goSouth(bool diSqFlag, float diSqScale, bool blurFlag, float blurring_factor, int kernel_size, bool thermalFlag, int thermalIterations, float thermalSlope, float thermalC)
    {

        //Same comments as in the 'goNorth' function

        for (int z = terrainSize - 1; z > patchSize; z--)
        {
            for (int x = 0; x < terrainSize; x++)
            {
                vertices[x, z] = vertices[x, z - patchSize];
            }
        }

        for (int z = 0; z < patchSize + 1; z++)
        {
            for (int x = 0; x < terrainSize; x++)
            {
                vertices[x, z].y = 0;
                vertices[x, z].z -= patchSize;
            }
        }

        if (diSqFlag)
        {

            for (int x = 0; x < terrainSize; x++)
                vertices[x, patchSize].y = vertices[x, patchSize + 1].y;

            a_corner = b_corner = c_corner = d_corner = true;

            c_corner = false;
            d_corner = false;

            for (int x = 0; x < patchCount; x++)
            {

                if (x > 0)
                {
                    a_corner = b_corner = c_corner = d_corner = true;

                    c_corner = false;
                    d_corner = false;
                    a_corner = false;
                }
                Vector3 temp = new Vector3(x * patchSize + x, 0, 0);

                initDiamondSquare(temp, diSqScale);
            }
        }

        if (thermalFlag)
        {

            //erosionManager.localThermalErosion(new Vector3(0, 0, 0), new Vector3(terrainSize - 1, 0, patchSize + thermalOffset), thermalIterations, thermalSlope, thermalC);
            //filterManager.applySpikesFilter(0.005f);
        }

        if (blurFlag)
            //filterManager.applyGaussianBlur(blurring_factor, kernel_size, new Vector3(0, 0, 0), new Vector3(terrainSize - 1, 0, patchSize + blurOffset));

        build();
    }

    public void goWest(bool diSqFlag, float diSqScale, bool blurFlag, float blurring_factor, int kernel_size, bool thermalFlag, int thermalIterations, float thermalSlope, float thermalC)
    {

        //Same comments as in the 'goNorth' function

        for (int x = terrainSize - 1; x > patchSize; x--)
        {
            for (int z = 0; z < terrainSize; z++)
            {
                vertices[x, z] = vertices[x - patchSize, z];
            }
        }

        for (int x = 0; x < patchSize + 1; x++)
        {
            for (int z = 0; z < terrainSize; z++)
            {
                vertices[x, z].y = 0;
                vertices[x, z].x -= patchSize;
            }
        }

        if (diSqFlag)
        {

            for (int z = 0; z < terrainSize; z++)
                vertices[patchSize, z].y = vertices[patchSize + 1, z].y;

            a_corner = b_corner = c_corner = d_corner = true;

            b_corner = false;
            c_corner = false;

            for (int z = 0; z < patchCount; z++)
            {

                if (z > 0)
                {
                    a_corner = b_corner = c_corner = d_corner = true;

                    b_corner = false;
                    c_corner = false;
                    a_corner = false;
                }
                Vector3 temp = new Vector3(0, 0, z * patchSize + z);

                initDiamondSquare(temp, diSqScale);
            }
        }

        if (thermalFlag)
        {

            //erosionManager.localThermalErosion(new Vector3(0, 0, 0), new Vector3(patchSize + thermalOffset, 0, terrainSize - 1), thermalIterations, thermalSlope, thermalC);
            //filterManager.applySpikesFilter(0.005f);
        }

        if (blurFlag)
           // filterManager.applyGaussianBlur(blurring_factor, kernel_size, new Vector3(0, 0, 0), new Vector3(patchSize + blurOffset, 0, terrainSize - 1));

        build();
    }

    public void goEast(bool diSqFlag, float diSqScale, bool blurFlag, float blurring_factor, int kernel_size, bool thermalFlag, int thermalIterations, float thermalSlope, float thermalC)
    {

        //Same comments as in the 'goNorth' function

        for (int x = 0; x < terrainSize - 1 - patchSize; x++)
        {
            for (int z = 0; z < terrainSize; z++)
            {
                vertices[x, z] = vertices[x + patchSize, z];
            }
        }

        for (int x = terrainSize - 1 - patchSize; x < terrainSize; x++)
        {
            for (int z = 0; z < terrainSize; z++)
            {
                vertices[x, z].y = 0;
                vertices[x, z].x += patchSize;
            }
        }

        if (diSqFlag)
        {

            for (int z = 0; z < terrainSize; z++)
                vertices[terrainSize - 1 - patchSize, z].y = vertices[terrainSize - 2 - patchSize, z].y;

            a_corner = b_corner = c_corner = d_corner = true;

            a_corner = false;
            d_corner = false;

            for (int z = 0; z < patchCount; z++)
            {

                if (z > 0)
                {
                    a_corner = b_corner = c_corner = d_corner = true;

                    a_corner = false;
                    d_corner = false;
                    b_corner = false;
                }
                Vector3 temp = new Vector3((patchCount - 1) * patchSize + (patchCount - 1), 0, z * patchSize + z);

                initDiamondSquare(temp, diSqScale);
            }
        }

        if (thermalFlag)
        {

            //erosionManager.localThermalErosion(new Vector3((patchCount - 1) * patchSize + (patchCount - 1) - thermalOffset, 0, 0), new Vector3(terrainSize - 1, 0, terrainSize - 1), thermalIterations, thermalSlope, thermalC);
            //filterManager.applySpikesFilter(0.005f);
        }

        if (blurFlag)
            //filterManager.applyGaussianBlur(blurring_factor, kernel_size, new Vector3((patchCount - 1) * patchSize + (patchCount - 1) - blurOffset, 0, 0), new Vector3(terrainSize - 1, 0, terrainSize - 1));

        build();
    }

    */

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

    /*
    public void exportObj()
    {

        //Export mesh at destination c:/ on the system
        //onto 4 meshes due to Unity's incapability of
        //holding meshes of over 65000 vertices

        ObjExporter.MeshToFile(myTerrain[0].GetComponent<MeshFilter>(), "/myTerrain_0.obj");
        ObjExporter.MeshToFile(myTerrain[1].GetComponent<MeshFilter>(), "/myTerrain_1.obj");
        ObjExporter.MeshToFile(myTerrain[2].GetComponent<MeshFilter>(), "/myTerrain_2.obj");
        ObjExporter.MeshToFile(myTerrain[3].GetComponent<MeshFilter>(), "/myTerrain_3.obj");
    }*/


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

    //Blurring kernel
    float[,] gaussianKernel;

    //On-the-fly generation offsets
    int blurOffset;
    int thermalOffset;

    //Hydraulic erosion maps
    public float[,] W; //water
    public float[,] S; //sediment
    public Vector2[,] V; //velocity
    public Vector4[,] F; //outflow flux


    //Wind parameters
    Vector2 windStrength = new Vector2(0.0f, 0.0f);
    float windCoverage = 1.0f;
    bool windAltitude = true;

    /* moved  to SceneManager due to Destroy function
    public void destroyMeshes()
    {

        //Meshes destructor

        for (int i = 0; i < 4; i++)
        {

            //Destroy(myMesh[i]);
            //Destroy(myWaterMesh[i]);
        }
    }
    */
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
