using UnityEngine;
using System.Collections;

public class GUICamera {

    int menuWidth;
    int rightOffset;
    int topOffset;
    int buttonHeight;
    public int yPos;

    GUIManager gm;

    cameraMovement camera;

    public GUICamera(GUIManager gm)
    {
        this.gm = gm;
        camera = GameObject.Find("MainCamera").GetComponent<cameraMovement>();
        camera.gui_camera = this;

        menuWidth = gm.menuWidth;
        rightOffset = gm.rightOffset;
        topOffset = gm.topOffset;
        buttonHeight = gm.smallButtonHeight;
    }
    public string projectionString = "perspective";

    public void OnGui(int yPosition)
    {
        yPos = yPosition;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, 3*buttonHeight + 2), "Camera");

        yPos += buttonHeight + 2;
        // FREE button settings
        if (GUI.Button(new Rect(Screen.width - menuWidth + 10, yPos, menuWidth - rightOffset - 20, buttonHeight), projectionString))
        {
            camera.ChangeProjection();
        }
        yPos += buttonHeight;

        GUI.Label(new Rect(Screen.width - menuWidth + 10, yPos, menuWidth - rightOffset, buttonHeight), "*Press 'C' to lock view.");

        yPos += buttonHeight;

    }
}
