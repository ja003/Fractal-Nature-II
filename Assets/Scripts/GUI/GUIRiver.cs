using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIRiver
{

    int menuWidth;
    int rightMenuOffset;
    int topOffset;
    int buttonHeight;
    public float yPos;
    int sideOffset;

    public float scaleY;
    public float visibleArea;

    GUIManager gm;

    public List<bool> riverFlags;

    RiverGenerator rg;

    public GUIRiver(GUIManager gm)
    {
        this.gm = gm;

        menuWidth = gm.menuWidth;
        rightMenuOffset = gm.rightOffset;
        topOffset = gm.topOffset;
        buttonHeight = gm.smallButtonHeight;
        sideOffset = 10;
        scaleY = gm.scaleY;
        visibleArea = gm.visibleArea;

        rg = gm.cm.riverGenerator;
        rg.riverGui = this;

        riverFlags = new List<bool>();
    }

    public void OnGui(int yPosition)
    {
        float buttonWidth = menuWidth - sideOffset - sideOffset / 2;

        yPos = yPosition;
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "Generate new river"))
        {
            rg.GenerateNewRiver();
            gm.cm.terrainGenerator.build();
        }

        yPos += buttonHeight + 5;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 11 * buttonHeight), "rivers");
        yPos += buttonHeight;

        for(int i = 0; i < riverFlags.Count;i++)
        {

            bool riverFlag = riverFlags[i];
            riverFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + sideOffset, yPos, sideOffset, buttonHeight), riverFlags[i], "");
            if (riverFlag != riverFlags[i])
            {
                riverFlags[i] = riverFlag;
                gm.cm.terrainGenerator.build();
            }

            GUI.Label(new Rect(Screen.width - menuWidth +  3 * sideOffset, yPos, buttonWidth/3, buttonHeight), "river " + i);
            
            if (GUI.Button(new Rect(Screen.width - menuWidth + 3 * sideOffset + buttonWidth/3, yPos, 3*sideOffset, buttonHeight), "X"))
            {
                rg.DeleteRiverAt(i);
                gm.cm.terrainGenerator.build();
            }

            

            yPos += buttonHeight + 5;
        }

    }

}
