﻿using UnityEngine;
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
    public bool minThresholdFilter;
    public bool maxThresholdFilter;

    public float spikeThreshold = 0.1f;
    float spikeThresholdFlag;

    float blurFactor = 1;
    float blurFactorFlag;

    float kernelSize = 1;
    float kernelSizeFlag;

    float minThreshold = -1;
    float minThresholdFlag;
    float maxThreshold = 2;
    float maxThresholdFlag;

    float minStrength = 3;
    float minStrengthFlag = 3;
    float maxStrength = 3;
    float maxStrengthFlag = 3;



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

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 15 * buttonHeight), "Filters");

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
            if (spikeThresholdFlag != spikeThreshold)
            {
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
                buttonHeight), spikeThreshold, 0.3f, 0);


        if (spikeFilterFlag != spikeFilter)
        {
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

        //MIN THRESHOLD
        bool minThresholdFilterFlag = minThresholdFilter;
        minThresholdFilterFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + sideOffset, yPos, sideOffset, buttonHeight), minThresholdFilter, "");
        if (GUI.Button(new Rect(Screen.width - menuWidth + 3 * sideOffset + 5, yPos, 1.5f*buttonWidth, buttonHeight),
            "Min Threshold"))
        {
            if (minStrength != minStrengthFlag || minThresholdFilter != minThresholdFilterFlag || minThreshold != minThresholdFlag)
            {
                gm.cm.filterGenerator.tf.ResetMinFilter();
                minThresholdFlag = minThreshold;
                minStrengthFlag = minStrength;
            }
            gm.cm.filterGenerator.tf.GenerateMinThresholdInRegion(gm.cm.localTerrain.GetVisibleArea(), minThreshold, minStrength);
            minThresholdFilter = true;
            minThresholdFilterFlag = true;
            gm.cm.terrainGenerator.filterMinThresholdLayer = true;
            gm.cm.terrainGenerator.build();
        }
        if (minThresholdFilterFlag != minThresholdFilter)
        {
            minThresholdFilter = minThresholdFilterFlag;
            gm.cm.terrainGenerator.filterMinThresholdLayer = minThresholdFilter;
            gm.cm.terrainGenerator.build();
        }
        if ((minStrength != minStrengthFlag || minThreshold != minThresholdFlag) && minThresholdFilter)
        {
            minThresholdFlag = minThreshold;
            minStrengthFlag = minStrength;
            gm.cm.filterGenerator.tf.ResetMinFilter();
            gm.cm.filterGenerator.tf.GenerateMinThresholdInRegion(gm.cm.localTerrain.GetVisibleArea(), minThreshold, minStrength);
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight;

        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), 
            "value: " + (int)minThreshold + "." +
            Mathf.Abs((int)((minThreshold - (int)minThreshold) * 100)));


        minThreshold = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth, yPos + 5,
                menuWidth - sideOffset - buttonWidth - 5,
                buttonHeight), minThreshold, -3f, 3f);

        yPos += buttonHeight;
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight),
            "strength: ");
        minStrength = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth, yPos + 5,
                menuWidth - sideOffset - buttonWidth - 5,
                buttonHeight), minStrength, 2f, 10f);

        yPos += buttonHeight;



        //MAX THRESHOLD
        bool maxThresholdFilterFlag = maxThresholdFilter;
        maxThresholdFilterFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + sideOffset, yPos, sideOffset, buttonHeight), maxThresholdFilter, "");
        if (GUI.Button(new Rect(Screen.width - menuWidth + 3 * sideOffset + 5, yPos, 1.5f * buttonWidth, buttonHeight),
            "Max Threshold"))
        {
            if (maxThresholdFilter != maxThresholdFilterFlag || 
                maxThreshold != maxThresholdFlag || 
                maxStrength != maxStrengthFlag)
            {
                gm.cm.filterGenerator.tf.ResetMaxFilter();
                maxThresholdFlag = maxThreshold;
                maxStrengthFlag = maxStrength;
            }
            gm.cm.filterGenerator.tf.GenerateMaxThresholdInRegion(gm.cm.localTerrain.GetVisibleArea(), maxThreshold, maxStrength);
            maxThresholdFilter = true;
            maxThresholdFilterFlag = true;
            gm.cm.terrainGenerator.filterMaxThresholdLayer = true;
            gm.cm.terrainGenerator.build();
        }
        if (maxThresholdFilterFlag != maxThresholdFilter)
        {
            maxThresholdFilter = maxThresholdFilterFlag;
            gm.cm.terrainGenerator.filterMaxThresholdLayer = maxThresholdFilter;
            gm.cm.terrainGenerator.build();
        }
        if ((maxStrength != maxStrengthFlag || maxThreshold != maxThresholdFlag) && maxThresholdFilter)
        {
            maxThresholdFlag = maxThreshold;
            maxStrengthFlag = maxStrength;

            gm.cm.filterGenerator.tf.ResetMaxFilter();
            gm.cm.filterGenerator.tf.GenerateMaxThresholdInRegion(gm.cm.localTerrain.GetVisibleArea(), maxThreshold, maxStrength);
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight;

        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight),
            "value: " + (int)maxThreshold + "." +
            Mathf.Abs((int)((maxThreshold - (int)maxThreshold) * 100)));
        maxThreshold = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth, yPos + 5,
                menuWidth - sideOffset - buttonWidth - 5,
                buttonHeight), maxThreshold, -3f, 3f);
        yPos += buttonHeight;
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight),
            "strength: ");
        maxStrength = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth, yPos + 5,
                menuWidth - sideOffset - buttonWidth - 5,
                buttonHeight), maxStrength, 2f, 10f);

        yPos += buttonHeight;




    }
}
