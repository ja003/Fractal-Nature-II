using UnityEngine;
using System.Collections;

public class GUIFilters
{

    int menuWidth;
    int rightMenuOffset;
    int topOffset;
    int buttonHeight;
    public float yPos;
    int sideOffset;

    public float scaleY;

    public bool averageFilter;
    public bool medianFilter;
    public bool spikeFilter;
    public bool gaussFilter;

    float spikeThreshold = 0.1f;
    float spikeThresholdFlag;

    float blurFactor = 1;
    float blurFactorFlag;

    float kernelSize = 1;
    float kernelSizeFlag;


    GUIManager gm;

    public GUIFilters(GUIManager gm)
    {
        this.gm = gm;

        menuWidth = gm.menuWidth;
        rightMenuOffset = gm.rightOffset;
        topOffset = gm.topOffset;
        buttonHeight = gm.smallButtonHeight;
        sideOffset = 10;
        scaleY = gm.scaleY;
    }

    public void OnGui(int yPosition)
    {
        float buttonWidth = menuWidth / 2 - sideOffset - sideOffset / 2;

        yPos = yPosition;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 9 * buttonHeight), "Filters");

        yPos += buttonHeight + 5;

        //AVERAGE
        bool averageFilterFlag = averageFilter;
        averageFilterFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + sideOffset, yPos, sideOffset, buttonHeight), averageFilter, "");
        if (GUI.Button(new Rect(Screen.width - menuWidth + 3*sideOffset + 5, yPos, buttonWidth, buttonHeight), "Average"))
        {
            gm.cm.filterGenerator.af.GenerateAverageFilterInRegion(gm.cm.localTerrain.GetVisibleArea());
            averageFilter = true;
            averageFilterFlag = true;
            gm.cm.terrainGenerator.filterAverageLayer = true;
            gm.cm.terrainGenerator.build();
        }

        if (averageFilterFlag != averageFilter)
        {
            averageFilter = averageFilterFlag;
            gm.cm.terrainGenerator.filterAverageLayer = averageFilter;
            gm.cm.terrainGenerator.build();
        }

        yPos += buttonHeight + 5;

        //MEDIAN
        bool medianFilterFlag = medianFilter;
        medianFilterFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + sideOffset, yPos, sideOffset, buttonHeight), medianFilter, "");
        if (GUI.Button(new Rect(Screen.width - menuWidth + 3 * sideOffset + 5, yPos, buttonWidth, buttonHeight), "Median"))
        {
            gm.cm.filterGenerator.mdf.GenerateMedianFilterInRegion(gm.cm.localTerrain.GetVisibleArea());
            medianFilter = true;
            medianFilterFlag = true;
            gm.cm.terrainGenerator.filterMedianLayer = true;
            gm.cm.terrainGenerator.build();
        }

        if (medianFilterFlag != medianFilter)
        {
            medianFilter = medianFilterFlag;
            gm.cm.terrainGenerator.filterMedianLayer = medianFilter;
            gm.cm.terrainGenerator.build();
        }

        yPos += buttonHeight + 5;

        //SPIKE
        bool spikeFilterFlag = spikeFilter;
        spikeFilterFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + sideOffset, yPos, sideOffset, buttonHeight), spikeFilter, "");
        if (GUI.Button(new Rect(Screen.width - menuWidth + 3 * sideOffset + 5, yPos, buttonWidth, buttonHeight), "Spike"))
        {
            Debug.Log("!");
            if (spikeThresholdFlag != spikeThreshold)
            {
                Debug.Log("!");
                gm.cm.filterGenerator.sf.ResetFilter();
                spikeThresholdFlag = spikeThreshold;
            }

            gm.cm.filterGenerator.sf.GenerateSpikeFilterInRegion(gm.cm.localTerrain.GetVisibleArea(), spikeThreshold);
            spikeFilter = true;
            spikeFilterFlag = true;
            gm.cm.terrainGenerator.filterSpikeLayer = true;
            gm.cm.terrainGenerator.build();

            gm.cm.filterGenerator.sf.lastEpsilon = spikeThreshold;
        }
        //Debug.Log(spikeThreshold);
        //Debug.Log(spikeThresholdFlag);
        spikeThreshold = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + 3 * sideOffset + buttonWidth + 10, yPos + 5,
                menuWidth - (3 * sideOffset + buttonWidth + 10 + rightMenuOffset + sideOffset),
                buttonHeight), spikeThreshold, 0, 0.3f);


        if (spikeFilterFlag != spikeFilter)
        {
            Debug.Log("?");


            spikeFilter = spikeFilterFlag;
            gm.cm.terrainGenerator.filterSpikeLayer = spikeFilter;
            gm.cm.terrainGenerator.build();
        }


        yPos += buttonHeight + 5;

        //GAUSS
        bool gaussFilterFlag = gaussFilter;
        gaussFilterFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + sideOffset, yPos, sideOffset, buttonHeight), gaussFilter, "");
        if (GUI.Button(new Rect(Screen.width - menuWidth + 3 * sideOffset + 5, yPos, buttonWidth, buttonHeight), "Gaussian"))
        {
            if (blurFactor != blurFactorFlag || kernelSize != kernelSizeFlag)
            {
                Debug.Log("!");
                gm.cm.filterGenerator.gf.ResetFilter();
                blurFactorFlag = blurFactor;
                kernelSizeFlag = kernelSize;
            }
            int kernel = (int)kernelSize*2 + 1;

            gm.cm.filterGenerator.gf.ApplyGaussianBlurOnRegion(blurFactor, kernel, gm.cm.localTerrain.GetVisibleArea());
            gaussFilter = true;
            gaussFilterFlag = true;
            gm.cm.terrainGenerator.filterGaussianLayer = true;
            gm.cm.terrainGenerator.build();

            gm.cm.filterGenerator.gf.lastBlurFactor = blurFactor;
            gm.cm.filterGenerator.gf.lastKernelSize = kernel;
        }

        yPos += buttonHeight;

        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "blur" );


        blurFactor = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth, yPos + 5,
                menuWidth - sideOffset - buttonWidth - 5,
                buttonHeight), blurFactor, 1.0f, 4.5f);
               

        yPos += buttonHeight;

        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "kernel size");


        kernelSize = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth, yPos + 5,
                menuWidth - sideOffset - buttonWidth - 5,
                buttonHeight), kernelSize, 1, 3);

        if (gaussFilterFlag != gaussFilter)
        {
            gaussFilter = gaussFilterFlag;
            gm.cm.terrainGenerator.filterGaussianLayer = gaussFilter;
            gm.cm.terrainGenerator.build();
        }

        yPos += buttonHeight + 5;

    }
}
