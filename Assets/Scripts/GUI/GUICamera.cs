using UnityEngine;
using System.Collections;

public class GUICamera {

    int menuWidth;
    int rightOffset;
    int topOffset;
    int buttonHeight;
    public int yPos;

    GUIManager gm;

    public GUICamera(GUIManager gm)
    {
        this.gm = gm;

        menuWidth = gm.menuWidth;
        rightOffset = gm.rightOffset;
        topOffset = gm.topOffset;
        buttonHeight = gm.smallButtonHeight;
    }

    public void OnGui(int yPosition)
    {
        yPos = yPosition;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, 3*buttonHeight), "Camera");

        yPos += buttonHeight;
        // FREE button settings
        if (GUI.Button(new Rect(Screen.width - menuWidth + 10, yPos, menuWidth / 2 - rightOffset - 20, buttonHeight), "Free"))
        {
            //terrain.ColorPixels();
            /*
            // Deactivate flight script, activate free mouse look and movement
            GameObject.Find("Airplane").GetComponent<PlanePilot>().enabled = false;
            GameObject.Find("Main Camera").GetComponent<MouseLook>().enabled = true;
            GameObject.Find("Main Camera").GetComponent<cameraMovement>().enabled = true;

            // Detach camera from the airplane
            GameObject.Find("Main Camera").transform.parent = null;

            // Move the airplane object below the mesh and the camera above, both aligned to the mesh
            GameObject.Find("Airplane").transform.position = new Vector3(terrain.middleOf.x, -300, terrain.middleOf.z);
            GameObject.Find("Main Camera").transform.position = new Vector3(terrain.middleOf.x, 200, terrain.middleOf.z);
            GameObject.Find("Main Camera").transform.LookAt(new Vector3(terrain.middleOf.x + 20, 200, terrain.middleOf.z));

            // Deactivate FLIGHT mode
            flightModeFlag = false;*/
        }
        // FLIGHT button settings
        if (GUI.Button(new Rect(Screen.width - menuWidth / 2 - rightOffset + 15, yPos, menuWidth / 2 - rightOffset - 20, buttonHeight), "Flight"))
        {
            /*
            // Activate flight script and deactivate the free mouse movement scripts
            GameObject.Find("Airplane").GetComponent<PlanePilot>().enabled = true;
            GameObject.Find("Main Camera").GetComponent<MouseLook>().enabled = false;
            GameObject.Find("Main Camera").GetComponent<cameraMovement>().enabled = false;

            // Move airplane object above the mesh, in the centre, align it to the world and link the camera to the object
            GameObject.Find("Airplane").transform.position = new Vector3(terrain.middleOf.x, 200, terrain.middleOf.z);
            GameObject.Find("Airplane").transform.LookAt(new Vector3(terrain.middleOf.x + 20, 200, terrain.middleOf.z));
            GameObject.Find("Main Camera").transform.parent = GameObject.Find("Airplane").transform;

            // Activate FLIGHT mode
            flightModeFlag = true;*/
        }
        yPos += buttonHeight;

        GUI.Label(new Rect(Screen.width - menuWidth + 10, yPos, menuWidth - rightOffset, buttonHeight), "*Press 'C' to lock view.");

        yPos += buttonHeight;

    }
}
