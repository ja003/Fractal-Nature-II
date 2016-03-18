using System;
using System.Collections.Generic;
using System.Linq;

public class DiamondSquare
{
    int[] indices;

    Random random;
    int roughness = 4;
    //int gridSize = 257;
    int seed = 0;
    
    LocalTerrain lt;

    int patchSize = 0;

    TerrainGenerator tg;
    public MountainPeaksManager mountainPeaksManager;

    public DiamondSquare(TerrainGenerator terrainGenerator, int patchSize)
    {
        tg = terrainGenerator;
        this.patchSize = patchSize;
    }

    public void AssignFunctions(LocalTerrain localTerrain)
    {
        lt = localTerrain;
        UnityEngine.Debug.Log(lt);
        UnityEngine.Debug.Log(tg.gm);
        mountainPeaksManager = new MountainPeaksManager(tg.gm);
    }

    public void Initialize(int patchSize)
    {
        random = new Random();
        seed = random.Next(100);
        
        GenerateTerrain(patchSize);
    }

    // TODO: break these off into a util class
    private int RandRange(Random r, int rMin, int rMax)
    {
        return rMin + r.Next() * (rMax - rMin);
    }

    private double RandRange(Random r, double rMin, double rMax)
    {
        return rMin + r.NextDouble() * (rMax - rMin);
    }

    private float RandRange(Random r, float rMin, float rMax)
    {
        return rMin + (float)r.NextDouble() * (rMax - rMin);
    }

    // Returns true if a is a power of 2, else false
    private bool pow2(int a)
    {
        return (a & (a - 1)) == 0;
    }

    /*
    *	Generates a grid of VectorPositionColor elements as a 2D greyscale representation of terrain by the
    *	Diamond-square algorithm: http://en.wikipedia.org/wiki/Diamond-square_algorithm
    * 
    *	Arguments: 
    *		int size - the width or height of the grid being passed in.  Should be of the form (2 ^ n) + 1
    *		int seed - an optional seed for the random generator
    *		float rMin/rMax - the min and max height values for the terrain (defaults to 0 - 255 for greyscale)
    *		float noise - the roughness of the resulting terrain
    * */
    public float[][] DiamondSquareGridOLD(int size, int seed = 0, float rMin = 0, float rMax = 255, float noise = 0.0f)
    {
        // Fail if grid size is not of the form (2 ^ n) - 1 or if min/max values are invalid
        int s = size - 1;
        if (!pow2(s) || rMin >= rMax)
            return null;

        float modNoise = 0.0f;

        // init the grid
        float[][] grid = new float[size][];
        for (int i = 0; i < size; i++)
            grid[i] = new float[size];

        // Seed the first four corners
        Random rand = (seed == 0 ? new Random() : new Random(seed));
        grid[0][0] = RandRange(rand, rMin, rMax);
        grid[s][0] = RandRange(rand, rMin, rMax);
        grid[0][s] = RandRange(rand, rMin, rMax);
        grid[s][s] = RandRange(rand, rMin, rMax);

        
			//* Use temporary named variables to simplify equations
			//* 
			//* s0 . d0. s1
			//*  . . . . . 
			//* d1 . cn. d2
			//*  . . . . . 
			//* s2 . d3. s3
			//* 
			//* 
        float s0, s1, s2, s3, d0, d1, d2, d3, cn;

        for (int i = s; i > 1; i /= 2)
        {
            // reduce the random range at each step
            modNoise = (rMax - rMin) * noise * ((float)i / s);

            // diamonds
            for (int y = 0; y < s; y += i)
            {
                for (int x = 0; x < s; x += i)
                {
                    s0 = grid[x][y];
                    s1 = grid[x + i][y];
                    s2 = grid[x][y + i];
                    s3 = grid[x + i][y + i];

                    // cn
                    grid[x + (i / 2)][y + (i / 2)] = ((s0 + s1 + s2 + s3) / 4.0f)
                        + RandRange(rand, -modNoise, modNoise);
                }
            }

            // squares
            for (int y = 0; y < s; y += i)
            {
                for (int x = 0; x < s; x += i)
                {
                    s0 = grid[x][y];
                    s1 = grid[x + i][y];
                    s2 = grid[x][y + i];
                    s3 = grid[x + i][y + i];
                    cn = grid[x + (i / 2)][y + (i / 2)];

                    d0 = y <= 0 ? (s0 + s1 + cn) / 3.0f : (s0 + s1 + cn + grid[x + (i / 2)][y - (i / 2)]) / 4.0f;
                    d1 = x <= 0 ? (s0 + cn + s2) / 3.0f : (s0 + cn + s2 + grid[x - (i / 2)][y + (i / 2)]) / 4.0f;
                    d2 = x >= s - i ? (s1 + cn + s3) / 3.0f :
                        (s1 + cn + s3 + grid[x + i + (i / 2)][y + (i / 2)]) / 4.0f;
                    d3 = y >= s - i ? (cn + s2 + s3) / 3.0f :
                        (cn + s2 + s3 + grid[x + (i / 2)][y + i + (i / 2)]) / 4.0f;

                    grid[x + (i / 2)][y] = d0 + RandRange(rand, -modNoise, modNoise);
                    grid[x][y + (i / 2)] = d1 + RandRange(rand, -modNoise, modNoise);
                    grid[x + i][y + (i / 2)] = d2 + RandRange(rand, -modNoise, modNoise);
                    grid[x + (i / 2)][y + i] = d3 + RandRange(rand, -modNoise, modNoise);
                }
            }
        }

        return grid;
    }
    

    int counter = 0;

    /// <summary>
    /// maps local coordinates to global and sets height
    /// </summary>
    public void SetLocalHeight(int x, int z, float value, bool overwrite)
    {/*
        if(x > 20 && x < 50)
        {
            value -= 0.5f;
        }*/
        lt.SetLocalHeight(x, z, value, overwrite);
    }

    /*
    public void SetLocalHeight(int x, int z)
    {
        float factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(0, 0, closestPeaks)) / maxDistance;
        float value = RandRange(rand, rMin * lowFactor, rMax * highFactor);
        bool overwrite = false;
        //value = 1;
        if (debugPeaks)
            SetLocalHeight(0, 0, factor, overwrite); //DEBUG PEAKS
        SetLocalHeight(0, 0, value * factor, overwrite);
    }*/

    float maxDistance = 666;
    

    /// <summary>
    /// calculates smallest distance from given local point to one of give peaks
    /// </summary>
    public float GetSmallestDistanceToPeak(int x, int z, List<Vertex> peaks)
    {
        float distance = 666;
        foreach (Vertex peak in peaks)
        {
            
            int globalX = (int)lt.GetGlobalCoordinate(x, z).x;
            int globalZ = (int)lt.GetGlobalCoordinate(x, z).z;
            float d = tg.fmc.GetDistance(globalX, globalZ, peak.x, peak.z);
            if (d < distance)
            {
                distance = d;
            }
            /*if(counter < 20 && globalX > -64 && globalZ > -64)
            {
                UnityEngine.Debug.Log(globalX + "," + globalZ + ":" + peak + "," + distance);
                counter++;
            }*/
            /*if (x >= 30 && x < 34 && z >= 30 && z < 34)
            {
                UnityEngine.Debug.Log(x+","+z+":"+peak);
                UnityEngine.Debug.Log("global:"+globalX+","+ globalZ);
                UnityEngine.Debug.Log(tg.fmc.GetDistance(globalX, globalZ, peak.x, peak.z));
                UnityEngine.Debug.Log(distance);
            }*/
        }
        if (peaks.Count == 0 || distance == 666)
        {
            UnityEngine.Debug.Log("no peak found");
            distance = maxDistance;
        }
        return distance;
    }
    /*
    private int LocalX(int x)
    {
        return x + (lt.terrainWidth / 2 - patchSize / 2);
    }

    private int LocalZ(int z)
    {
        return z + (lt.terrainHeight / 2 - patchSize / 2);
    }
    */
    /// <summary>
    /// set corners based on neighbouring values
    /// if neighbourhood is not defined, values are random
    /// </summary>
    public void SetupCorners(Random rand, float rMin, float rMax, int s, bool overwrite, List<Vertex> closestPeaks)
    {
        float neighbourhood = 666;
        float factor = 666;
        float value = 666;

        //UnityEngine.Debug.Log("setting: " + lt.GetGlobalCoordinate(0, 0));
        //UnityEngine.Debug.Log("setting: " + lt.GetGlobalCoordinate(s, 0));
        //UnityEngine.Debug.Log("setting: " + lt.GetGlobalCoordinate(0, s));
        //UnityEngine.Debug.Log("setting: " + lt.GetGlobalCoordinate(s, s));

        neighbourhood = lt.GetNeighbourHeight(0, 0);
        if (neighbourhood != 666)
        {
            SetLocalHeight(0, 0, neighbourhood, false);
        }
        else
        {
            factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(0, 0, closestPeaks)) / maxDistance;
            value = RandRange(rand, rMin * lowFactor, rMax * highFactor);
            //value = 1;
            if(debugPeaks)
                SetLocalHeight(0, 0, factor, overwrite); //DEBUG PEAKS
            SetLocalHeight(0, 0, value * factor, overwrite);
        }

        neighbourhood = lt.GetNeighbourHeight(s, 0);
        if (neighbourhood != 666)
        {
            SetLocalHeight(s, 0, neighbourhood, false);
        }
        else
        {
            factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(s, 0, closestPeaks)) / maxDistance;
            value = RandRange(rand, rMin * lowFactor, rMax * highFactor);
            //value = 1;
            if (debugPeaks)
                SetLocalHeight(s, 0, factor, overwrite); //DEBUG PEAKS
            SetLocalHeight(s, 0, value * factor, overwrite);
        }

        neighbourhood = lt.GetNeighbourHeight(0, s);
        if (neighbourhood != 666)
        {
            SetLocalHeight(0, s, neighbourhood, false);
        }
        else
        {
            factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(0, s, closestPeaks)) / maxDistance;
            value = RandRange(rand, rMin * lowFactor, rMax * highFactor);
            //value = 1;
            if (debugPeaks)
                SetLocalHeight(0, s, factor, overwrite); //DEBUG PEAKS
            SetLocalHeight(0, s, value * factor, overwrite);
        }

        neighbourhood = lt.GetNeighbourHeight(s, s);
        if (neighbourhood != 666)
        {
            SetLocalHeight(s, s, neighbourhood, false);
        }
        else
        {
            factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(s, s, closestPeaks)) / maxDistance;
            value = RandRange(rand, rMin * lowFactor, rMax * highFactor);
            //value = 1;
            if (debugPeaks)
                SetLocalHeight(s,s, factor, overwrite); //DEBUG PEAKS
            SetLocalHeight(s, s, value * factor, overwrite);
        }
    }


    float factorConstant = 2;
    float highFactor = 0.5f;
    float lowFactor = 1;
    bool debugPeaks = false;
    float rMin;
    float rMax;
    List<Vertex> closestPeaks;
    Random rand;


    public void DiamondSquareGrid(int size, int seed = 0, 
        float rMin = 0, float rMax = 255, float noise = 0.0f)
    {
        // Fail if grid size is not of the form (2 ^ n) - 1 or if min/max values are invalid
        //int s = size - 1;
        int s = size;
        if (!pow2(s) || rMin >= rMax)
        {
            UnityEngine.Debug.Log("size has to be power of 2. size = " + size);
            return;
        }

        float modNoise = 0.0f;

        this.rMin = rMin;
        this.rMax = rMax;
        this.rand = (seed == 0 ? new Random() : new Random(seed));
        closestPeaks = mountainPeaksManager.GetClosestPeaks(tg.localTerrain.localTerrainC.center);
        
        float defaultHeight = -20; //only to detect deffects in process. If terrain has some bad height (too high/low), there is some error
        float defaultHeight2 = 20;//debug

        bool overwrite = false;
        // Seed the first four corners
        
        float neighbourhood = 666;


        float factor = 666;

        SetupCorners(rand, rMin, rMax, s, overwrite, closestPeaks);

        float s0, s1, s2, s3, d0, d1, d2, d3, cn;
        
        float height = 0;

        for (int i = s; i > 1; i /= 2)
        {
            // reduce the random range at each step
            float modNoiseOrig = (rMax - rMin) * noise * ((float)i / s);

            // diamonds
            for (int z = 0; z < s; z += i)
            {
                for (int x = 0; x < s; x += i)
                {
                    s0 = lt.GetLocalHeight(x, z, defaultHeight); //shouldn't need to define default 'undefined' value
                    s1 = lt.GetLocalHeight(x + i, z, defaultHeight);
                    s2 = lt.GetLocalHeight(x, z + i, defaultHeight);
                    s3 = lt.GetLocalHeight(x + i, z + i, defaultHeight);

                    //factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x + (i / 2), z + (i / 2), closestPeaks)) / maxDistance;
                    modNoise = GetModNoise(x + (i / 2), z + (i / 2), modNoiseOrig);
                    height = GetHeight(x + (i / 2), z + (i / 2),(s0 + s1 + s2 + s3) / 4.0f, modNoise);
                    SetLocalHeight(x + (i / 2), z + (i / 2), height, overwrite);
                }
            }
            counter = 0;
             
            // squares
            for (int z = 0; z < s; z += i)
            {
                for (int x = 0; x < s; x += i)
                {
                    s0 = lt.GetLocalHeight(x, z, defaultHeight);
                    s1 = lt.GetLocalHeight(x + i, z, defaultHeight);
                    s2 = lt.GetLocalHeight(x, z + i, defaultHeight);
                    s3 = lt.GetLocalHeight(x + i, z + i, defaultHeight);
                    cn = lt.GetLocalHeight(x + (i / 2), z + (i / 2), defaultHeight);

                    d0 = z <= 0 ? (s0 + s1 + cn) / 3.0f :
                        (s0 + s1 + cn + lt.GetLocalHeight(x + (i / 2), z - (i / 2), defaultHeight)) / 4.0f;
                    d1 = x <= 0 ? (s0 + cn + s2) / 3.0f :
                        (s0 + cn + s2 + lt.GetLocalHeight(x - (i / 2), z + (i / 2), defaultHeight)) / 4.0f;
                    d2 = x >= s - i ? (s1 + cn + s3) / 3.0f :
                        (s1 + cn + s3 + lt.GetLocalHeight(x + i + (i / 2), z + (i / 2), defaultHeight)) / 4.0f; 
                    d3 = z >= s - i ? (cn + s2 + s3) / 3.0f :
                        (cn + s2 + s3 + lt.GetLocalHeight(x + (i / 2), z + i + (i / 2), defaultHeight)) / 4.0f;
                    

                    //factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x+ (i / 2), z, closestPeaks)) / maxDistance;
                    modNoise = GetModNoise(x + (i / 2), z, modNoiseOrig); 
                    height = GetHeight(x + (i / 2), z, d0, modNoise);
                    SetLocalHeight(x + (i / 2), z, height, overwrite);


                    //factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x, z + (i / 2), closestPeaks)) / maxDistance;
                    modNoise = GetModNoise(x, z + (i / 2), modNoiseOrig);
                    height = GetHeight(x, z + (i / 2), d1, modNoise);
                    SetLocalHeight(x, z + (i / 2), height, overwrite);


                    //factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x + i, z + (i / 2), closestPeaks)) / maxDistance;
                    modNoise = GetModNoise(x + i, z + (i / 2), modNoiseOrig);
                    height = GetHeight(x + i, z + (i / 2), d2, modNoise);
                    SetLocalHeight(x + i, z + (i / 2), height, overwrite);


                    //factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x + (i / 2), z + i, closestPeaks)) / maxDistance;
                    modNoise = GetModNoise(x + (i / 2), z + i, modNoiseOrig);
                    height = GetHeight(x + (i / 2), z + i,d3, modNoise);
                    SetLocalHeight(x + (i / 2), z + i, height, overwrite);
                    
                }
            }
        }
        //UnityEngine.Debug.Log("Diamond square complete");
    }

    public float GetHeight(int x, int z, float initValue, float modNoise)
    {
        if (debugPeaks)
        {
            float factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x, z, closestPeaks)) / maxDistance;
            return factor;
        }
        /*
        if ((x > 20 && x < 50) ||(z > 30 && z < 60))
        {
            lowFactor = 3;
        }
        else
        {
            lowFactor = 1;
        }*/
        float height = initValue + RandRange(rand, -modNoise * lowFactor, modNoise * highFactor);
        
        return height;
    }

    public float GetModNoise(int x, int z, float modNoiseOrig)
    {
        float factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x, z, closestPeaks)) / maxDistance;

        float modNoise = modNoiseOrig * factor;
        /*if ((x > 20 && x < 50) || (z > 30 && z < 60))
        {
            modNoise /= (2*factor*factor);
            //modNoise -= factor;
            if (modNoise < 0)
                modNoise = 0;
        }*/
        /*if(counter < 5)
        {
            counter++;
            UnityEngine.Debug.Log(factor);
            UnityEngine.Debug.Log(modNoise);
        }*/

        return modNoise;
    }

    //float[][] ds;

    public void GenerateTerrain(int patchSize)
    {
    //    UnityEngine.Debug.Log("!!!");
    //    UnityEngine.Debug.Log(tg.localTerrain.localTerrainC.center);
        maxDistance = (float)Math.Sqrt(patchSize * patchSize + patchSize * patchSize);

        DiamondSquareGrid(patchSize, seed, 0, 2, roughness / 5.0f);
    }
    /*
    public void CopyValues()
    {
        for(int x = 0; x < lt.terrainWidth; x++)
        {
            for (int z = 0; z < lt.terrainHeight; z++)
            {
                SetLocalHeight(x, z, ds[x][z], true);
            }
        }
    }*/

    /*
	private void InitShaders()
	{
		float tilt = MathHelper.ToRadians(0);
		worldMatrix = Matrix.CreateRotationX(tilt) * Matrix.CreateRotationY(tilt);
		viewMatrix = Matrix.CreateLookAt(new Vector3(0, 25, 60), Vector3.Zero, Vector3.Up);

		projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
			MathHelper.ToRadians(45),  // 45 degree angle
			(float)GraphicsDevice.Viewport.Width /
			(float)GraphicsDevice.Viewport.Height,
			1.0f, 128.0f);

		basicEffect = new BasicEffect(graphics.GraphicsDevice);
		basicEffect.VertexColorEnabled = false;
		basicEffect.LightingEnabled = true;
		basicEffect.FogEnabled = true;
		//basicEffect.FogColor = new Vector3(0.3f, 0.3f, 0.3f);
		basicEffect.FogStart = 32.0f;
		basicEffect.FogEnd = 128;
		basicEffect.PreferPerPixelLighting = true;

		basicEffect.World = worldMatrix;
		//basicEffect.View = viewMatrix;
		float rads = MathHelper.ToRadians(rotation);
		basicEffect.View = Matrix.CreateRotationY(rads) * Matrix.CreateLookAt(new Vector3(0, 25, 60), Vector3.Zero, Vector3.Up);

		basicEffect.Projection = projectionMatrix;

		// primitive color
		//basicEffect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
		basicEffect.DirectionalLight0.Enabled = true;
		basicEffect.DirectionalLight0.Direction = new Vector3(0.0f, -1.0f, -1.0f);

		basicEffect.DirectionalLight0.DiffuseColor = Color.OliveDrab.ToVector3();
	}
    */
    /*
	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(GraphicsDevice);
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			this.Exit();

			
			//* Keys:
			//*   Up/Down  - Increase/decrase roughness
			//*   Spacebar - Enable/disable rotation
			//*   Enter    - Generate new terrain
			 
		KeyboardState keyState = Keyboard.GetState();
		if (keyState.GetPressedKeys().Length > 1)
		{
			if (!keyDown)
			{
				if (keyState.IsKeyDown(Keys.Up))
					roughness++;
				else if (keyState.IsKeyDown(Keys.Down))
					roughness--;
				else if (keyState.IsKeyDown(Keys.Space))
					doRotate = !doRotate;
				else if (keyState.IsKeyDown(Keys.Enter))
					seed = random.Next(100);

				keyDown = true;
				GenerateTerrain();
			}
		}
		else keyDown = false;

		if (doRotate)
		{
			rotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 40.0f;

			// prevent overflow
			if (rotation >= 360)
				rotation -= 360;

			float rads = MathHelper.ToRadians(rotation);
			basicEffect.View = Matrix.CreateRotationY(rads) *Matrix.CreateLookAt(new Vector3(0, 25, 60),
				Vector3.Zero, Vector3.Up);
		}

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.SteelBlue);

		foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
		{
			pass.Apply();

			GraphicsDevice.RasterizerState = rasterState;
			GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
				PrimitiveType.TriangleList,
				verts,
				0,
				verts.Length,
				indices,
				0,
				indices.Length / 3);
		}

		base.Draw(gameTime);
	}
}*/
}
