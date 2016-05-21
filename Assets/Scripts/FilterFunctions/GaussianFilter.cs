using UnityEngine;
using System.Collections;

public class GaussianFilter {

    private LocalTerrain lt;
    private FunctionMathCalculator fmc;
    public GlobalCoordinates globalFilterGaussianC;
    public FilterGenerator fg;

    public int lastKernelSize;
    public float lastBlurFactor;

    public GaussianFilter(FilterGenerator fg)
    {
        this.fg = fg;
    }

    public void AssignFunctions(LocalTerrain lt, FunctionMathCalculator fmc, GlobalCoordinates globalFilterGaussianC)
    {
        this.lt = lt;
        this.fmc = fmc;
        this.globalFilterGaussianC = globalFilterGaussianC;
    }


    public void ApplyGaussianBlurOnRegion(float blurring_factor, int kernel_size, Area region)
    {
        Debug.Log("gauss on " + region + " with " + blurring_factor + "," + kernel_size);

        int x_min = region.botLeft.x;
        int z_min = region.botLeft.z;

        int x_max = region.topRight.x;
        int z_max = region.topRight.z;

        //Build the kernel
        float[,] kernel =  initGaussKernel(blurring_factor, kernel_size);
        int half_step = (int)(kernel_size / 2);

        
        //Iterate through the mesh
        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                
                if (lt.globalTerrainC.IsDefined(x, z))
                {
                    float sum = 0.0f;
                    float centerVal = lt.lm.GetCurrentHeight(x, z);
                    bool definedNeighb = true;
                    //Iterate through kernel
                    for (int m = -1 * half_step; m <= half_step; m++)
                    {
                        for (int n = -1 * half_step; n <= half_step; n++)
                        {
                            if (definedNeighb)
                            {
                                float value;
                                if (!lt.globalTerrainC.IsDefined(x + m, z + n))
                                    definedNeighb = false;
                                value = lt.lm.GetCurrentHeight(x + m, z + n);
                                sum += value * kernel[m + half_step, n + half_step];
                            }
                        }
                    }
                    if(definedNeighb)//don't generate filter on edge
                        fg.SetGlobalValue(x, z, centerVal - sum, false, globalFilterGaussianC);
                }
            }
        }
    }

    /// <summary>
    /// creates Gaussian kernel
    /// </summary>
    private float[,] initGaussKernel(float blurring_factor, int kernel_size)
    {

        //Gaussian kernel build algorithm

        //Initialise kernel map
        float [,] gaussianKernel = new float[kernel_size, kernel_size];
        int half_step = (int)(kernel_size / 2);
        
        float sum = 0.0f;

        //Iterate through kernel map
        for (int x = -1 * half_step; x <= half_step; x++)
        {
            for (int y = -1 * half_step; y <= half_step; y++)
            {

                //Assign raw values to the kernel
                gaussianKernel[x + half_step, y + half_step] = (1.0f / 2 * Mathf.PI * (blurring_factor * blurring_factor)) * Mathf.Exp(-1.0f * ((x * x + y * y) / (2 * (blurring_factor * blurring_factor))));
                sum += gaussianKernel[x + half_step, y + half_step];
            }
        }

        //Iterate through map
        for (int x = -1 * half_step; x <= half_step; x++)
        {
            for (int y = -1 * half_step; y <= half_step; y++)
            {
                //Normalise the values
                gaussianKernel[x + half_step, y + half_step] = gaussianKernel[x + half_step, y + half_step] / sum;
            }
        }

        return gaussianKernel;
    }

    public void ResetFilter()
    {
        globalFilterGaussianC.ResetQuadrants();
        Debug.Log("reset gauss");
    }
}
