using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIRiverExtra
{

    int menuWidth;
    int rightMenuOffset;
    int topOffset;
    int buttonHeight;
    public float yPos;
    int sideOffset;

    public float scaleY;
    public float visibleArea;

    //public float width;
    //public float depth;
    //public float areaEffect;


    GUIManager gm;

    public List<bool> riverFlags;

    RiverGenerator rg;

    RiverInfo selectedRiver;
    RiverInfo defaultRiver;

    public GUIRiverExtra(GUIManager gm)
    {
        this.gm = gm;
    }

    public void OnGui(int yPosition)
    {
        float buttonWidth = menuWidth - sideOffset - sideOffset / 2;
        float buttonWidth2 = menuWidth / 2 - sideOffset - sideOffset / 2;
        

        yPos += buttonHeight + 5;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 4.5f * buttonHeight), "parameters");

        yPos += buttonHeight + 2;
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth2, buttonHeight), 
            "X: " + (int)selectedRiver.width);
        selectedRiver.width = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth2, yPos + 5,
                menuWidth - sideOffset - buttonWidth2 - 5,
                buttonHeight), selectedRiver.width, 8f, 20f);

        yPos += buttonHeight + 2;
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth2, buttonHeight), 
            "area: " + (int)selectedRiver.areaEffect + "." + (int)((selectedRiver.areaEffect - (int)selectedRiver.areaEffect) * 100));
        selectedRiver.areaEffect = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth2, yPos + 5,
                menuWidth - sideOffset - buttonWidth2 - 5,
                buttonHeight), selectedRiver.areaEffect, 0.5f, 2);
        
            yPos += buttonHeight + 5;
        

    }

}
