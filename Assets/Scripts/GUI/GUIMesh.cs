using UnityEngine;
using System.Collections;

public class GUIMesh {

    int menuWidth;
    int rightMenuOffset;
    int topOffset;
    int buttonHeight;
    public int yPos;
    int sideOffset;

    float scaleY;

    GUIManager gm;

    public GUIMesh(GUIManager gm)
    {
        this.gm = gm;

        menuWidth = gm.menuWidth;
        rightMenuOffset = gm.rightOffset;
        topOffset = gm.topOffset;
        buttonHeight = gm.smallButtonHeight;
        sideOffset = 10;
        scaleY = 20;
    }

    public void OnGui(int yPosition)
    {
        yPos = yPosition;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 9* buttonHeight), "Mesh Control");

        yPos += buttonHeight;

        // 'On-the-fly' mesh generation toggle
        gm.infiniteTerrain = GUI.Toggle(new Rect(Screen.width - menuWidth + sideOffset, yPos, menuWidth - rightMenuOffset, buttonHeight), gm.infiniteTerrain, "  Allow on-the-fly generation");


        yPos += buttonHeight; 
        
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, menuWidth - rightMenuOffset, buttonHeight),
            "Patch Size = " + gm.patchSize + "x" + gm.patchSize);
        

        yPos += buttonHeight + 5;

        float buttonWidth = menuWidth / 2 - sideOffset - sideOffset/2;

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

        // GENERATE
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, menuWidth - 2*sideOffset, 2* buttonHeight), "GENERATE"))
        {
            Debug.Log("[8]: generating on: " + gm.cm.gameObject.transform.position);
            gm.cm.localTerrain.UpdateVisibleTerrain(gm.cm.gameObject.transform.position);
        }

        yPos += 2*buttonHeight;

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

        /*
        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, 65), " ");
        if (GUI.Button(new Rect(Screen.width - menuWidth + 10, offset + 5, menuWidth - rightOffset - 20, 35), "EXPORT MESH"))
        {

        }*/
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
}
