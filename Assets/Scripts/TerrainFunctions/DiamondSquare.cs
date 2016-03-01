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

    TerrainGenerator tg;

    public DiamondSquare(TerrainGenerator terrainGenerator)
    {
        tg = terrainGenerator;
    }

    public void AssignFunctions(LocalTerrain localTerrain)
    {
        lt = localTerrain;
    }

    public void Initialize()
    {
        random = new Random();
        seed = random.Next(100);
        
        GenerateTerrain();
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
    /*
    public float[][] DiamondSquareGrid(int size, int seed = 0, float rMin = 0, float rMax = 255, float noise = 0.0f)
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
    */

    int counter = 0;

    public void DiamondSquareGrid(int size, int seed = 0, float rMin = 0, float rMax = 255, float noise = 0.0f)
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


        //TODO: there is some bug on the Z-edge, if i put defaultHeight high, border will be high
        float defaultHeight = 0; //only to detect deffects in process. If terrain has some bad height (too high/low), there is some error
        float defaultHeight2 = 20;//debug

        bool overwrite = false;
        // Seed the first four corners
        Random rand = (seed == 0 ? new Random() : new Random(seed));
        float neighbourhood = 666;
        //TODO: make method
        neighbourhood = lt.GetNeighbourHeight(0, 0);
        if (neighbourhood != 666)
        {
            lt.SetLocalHeight(0, 0, neighbourhood, false);
        }
        else
        {
            lt.SetLocalHeight(0, 0, RandRange(rand, rMin, rMax), overwrite);
        }

        neighbourhood = lt.GetNeighbourHeight(s, 0);
        if (neighbourhood != 666)
        {
            lt.SetLocalHeight(s, 0, neighbourhood, false);
        }
        else
        {
            lt.SetLocalHeight(s, 0, RandRange(rand, rMin, rMax), overwrite);
        }

        neighbourhood = lt.GetNeighbourHeight(0, s);
        if (neighbourhood != 666)
        {
            lt.SetLocalHeight(0, s, neighbourhood, false);
        }
        else
        {
            lt.SetLocalHeight(0, s, RandRange(rand, rMin, rMax), overwrite);
        }

        neighbourhood = lt.GetNeighbourHeight(s,s);
        if (neighbourhood != 666)
        {
            lt.SetLocalHeight(s, s, neighbourhood, false);
        }
        else
        {
            lt.SetLocalHeight(s, s, RandRange(rand, rMin, rMax), overwrite);
        }

        
        
        /*
			* Use temporary named variables to simplify equations
			* 
			* s0 . d0. s1
			*  . . . . . 
			* d1 . cn. d2
			*  . . . . . 
			* s2 . d3. s3
			* 
			* */
        float s0, s1, s2, s3, d0, d1, d2, d3, cn;


        for (int i = s; i > 1; i /= 2)
        {
            // reduce the random range at each step
            modNoise = (rMax - rMin) * noise * ((float)i / s);

            // diamonds
            for (int z = 0; z < s; z += i)
            {
                for (int x = 0; x < s; x += i)
                {
                    
                    s0 = lt.GetLocalHeight(x, z, defaultHeight); //should need to define default 'undefined' value
                    s1 = lt.GetLocalHeight(x + i, z, defaultHeight);
                    s2 = lt.GetLocalHeight(x, z + i, defaultHeight);
                    s3 = lt.GetLocalHeight(x + i, z + i, defaultHeight);

                    if (s0 > defaultHeight2 || s1 > defaultHeight2 || s2 > defaultHeight2 || s3 > defaultHeight2)
                    {
                        counter++;
                        UnityEngine.Debug.Log("default");
                    }

                    if (lt.localCoordinates.IsDefined(x + (i / 2), z + (i / 2), lt.globalTerrainC) && counter < 10)
                    {
                        UnityEngine.Debug.Log(x + "," + z + ": set");
                        counter++;
                    }
                    // cn
                    lt.SetLocalHeight(x + (i / 2), z + (i / 2), ((s0 + s1 + s2 + s3) / 4.0f)
                        + RandRange(rand, -modNoise, modNoise), overwrite);
                }
            }

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
                    d3 = x >= s - i ? (s1 + cn + s3) / 3.0f : 
                        (s1 + cn + s3 + lt.GetLocalHeight(x + (i / 2), z + i + (i / 2), defaultHeight)) / 4.0f;

                    if (s0 > defaultHeight2 || s1 > defaultHeight2 || s2 > defaultHeight2 || s3 > defaultHeight2||
                        d0 > defaultHeight2 || d1 > defaultHeight2 || d2 > defaultHeight2 || d3 > defaultHeight2||
                        cn > defaultHeight2)
                    {
                        counter++; 
                        UnityEngine.Debug.Log("default");
                    }

                    lt.SetLocalHeight(x + (i / 2), z, d0 + RandRange(rand, -modNoise, modNoise), overwrite);
                    lt.SetLocalHeight(x, z + (i / 2), d1 + RandRange(rand, -modNoise, modNoise), overwrite);
                    lt.SetLocalHeight(x + i, z + (i / 2), d2 + RandRange(rand, -modNoise, modNoise), overwrite);
                    lt.SetLocalHeight(x + (i / 2), z + i, d3 + RandRange(rand, -modNoise, modNoise), overwrite);

                }
            }
        }
        UnityEngine.Debug.Log("Diamond square complete");
    }


    //float[][] ds;

    public void GenerateTerrain()
    {
        //!!!TODO: terrainWidth?
        DiamondSquareGrid(lt.terrainWidth, seed, -1, 2, roughness / 5.0f);
    }
    /*
    public void CopyValues()
    {
        for(int x = 0; x < lt.terrainWidth; x++)
        {
            for (int z = 0; z < lt.terrainHeight; z++)
            {
                lt.SetLocalHeight(x, z, ds[x][z], true);
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
