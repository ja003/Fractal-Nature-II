using UnityEngine;
using System.Collections;

public class GUIMesh {

    int menuWidth;
    int rightMenuOffset;
    int topOffset;
    int buttonHeight;
    public float yPos;
    int sideOffset;

    public float scaleY;
    public float noise;
    public float visibleArea;

    GUIManager gm;

    public GUIMesh(GUIManager gm)
    {
        this.gm = gm;

        menuWidth = gm.menuWidth;
        rightMenuOffset = gm.rightOffset;
        topOffset = gm.topOffset;
        buttonHeight = gm.smallButtonHeight;
        sideOffset = 10;
        noise = gm.cm.terrainGenerator.noise;
        scaleY = gm.scaleY;
        visibleArea = gm.visibleArea;
    }

    public void OnGui(int yPosition)
    {
        float buttonWidth = menuWidth / 2 - sideOffset - sideOffset / 2;

        yPos = yPosition;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 13 * buttonHeight), "Mesh Control");

        yPos += buttonHeight;

        // 'On-the-fly' mesh generation toggle
        gm.onFlyGeneration = GUI.Toggle(new Rect(Screen.width - menuWidth + sideOffset, yPos, menuWidth - rightMenuOffset, buttonHeight), gm.onFlyGeneration, "  Allow on-the-fly generation");


        yPos += buttonHeight;
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2*buttonWidth, buttonHeight), "Visible area = " + gm.visibleArea);

        yPos += buttonHeight;
        visibleArea = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + sideOffset, yPos + 5,
                menuWidth - sideOffset - 5,
                buttonHeight), gm.visibleArea, 64, 500);
        //if scale has been changed, perform scale action
        if (GetAreaValue(visibleArea) != gm.visibleArea)
        {
            UpdateVisibleArea(GetAreaValue(visibleArea));
        }

        yPos += buttonHeight;

        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, menuWidth - rightMenuOffset, buttonHeight),
            "Patch Size = " + gm.patchSize + "x" + gm.patchSize);
        

        yPos += buttonHeight + 5;


        // 64x64 patch size
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "64x64"))
        {
            UpdatePatchSize(64);
        }

        // 128x128 patch size
        if (GUI.Button(new Rect(Screen.width - menuWidth + buttonWidth + 2* sideOffset, yPos, buttonWidth, buttonHeight), "128x128"))
        {
            UpdatePatchSize(128);
        }
        yPos += buttonHeight;

        // 256x256 patch size
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "256x256"))
        {
            UpdatePatchSize(256);
        }

        // 512x512 patch size
        if (GUI.Button(new Rect(Screen.width - menuWidth + buttonWidth + 2 * sideOffset, yPos, buttonWidth, buttonHeight), "512x512"))
        {
            UpdatePatchSize(512);
        }

        yPos += buttonHeight + 5;
        
        //ROUGNESS
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth + 4*sideOffset, buttonHeight), "Noise = " + (int)noise
            + "." + (int)((noise - (int)noise)*100));

        noise = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth + 3*sideOffset, yPos + 5,
                menuWidth - 4*sideOffset - buttonWidth - 5,
                buttonHeight), noise, 0, 5);

        if (noise != gm.cm.terrainGenerator.noise)
        {
            gm.cm.terrainGenerator.noise = noise;
        }

        yPos += buttonHeight + 5;

        // GENERATE
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, menuWidth - 2*sideOffset, 1.5f* buttonHeight), "GENERATE"))
        {
            //Debug.Log("generating on: " + gm.cm.gameObject.transform.position);
            gm.cm.localTerrain.UpdateVisibleTerrain(gm.cm.gameObject.transform.position, false);
        }
        yPos += 1.5f * buttonHeight;

        // DELETE meshes
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, menuWidth - 2 * sideOffset, buttonHeight), "DELETE"))
        {
            Debug.Log("DELETE mesh" + gm.cm.gameObject.transform.position);
            gm.cm.terrainGenerator.destroyMeshes();
            gm.cm.terrainGenerator.ResetTerrainValues();
        }

        yPos += buttonHeight;

        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "Scale = " + (int)gm.scaleY);
        

        scaleY = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth, yPos + 5,
                menuWidth - sideOffset - buttonWidth - 5,
                buttonHeight), scaleY, 0, 30);
        //if scale has been changed, perform scale action
        if (scaleY != gm.scaleY)
        {
            UpdateScaleY(scaleY);
        }
        yPos += buttonHeight;

        /*
        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, 65), " ");
        if (GUI.Button(new Rect(Screen.width - menuWidth + 10, offset + 5, menuWidth - rightOffset - 20, 35), "EXPORT MESH"))
        {

        }*/
    }

    /// <summary>
    /// transforms float area to even integer number
    /// </summary>
    private int GetAreaValue(float area)
    {
        int intArea = (int)area;
        if (intArea % 2 != 0)
            intArea += 1;
        return intArea;
    }

    private void UpdatePatchSize(int patchSize)
    {
        gm.UpdatePatchSize(patchSize);
    }

    private void UpdateScaleY(float scaleY)
    {
        gm.scaleY = scaleY;
        gm.cm.terrainGenerator.UpdateScaleY(scaleY);
    }

    private void UpdateVisibleArea(int visibleArea)
    {
        gm.visibleArea = visibleArea;
        gm.cm.UpdateVisibleArea(visibleArea);
    }
}
