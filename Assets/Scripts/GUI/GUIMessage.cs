using UnityEngine;
using System.Collections;

public class GUIMessage {

    GUIManager gm;

    public int messageEndFrame;
    int leftMargin = 5;

    public string message = "666";
    GUIStyle textStyle;

    public GUIMessage(GUIManager gm)
    {
        this.gm = gm;
        messageEndFrame = 666;

        textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.MiddleCenter;
    }

    public void OnGui(int yPosition)
    {
        int yPos = yPosition;

        GUI.Box(new Rect(Screen.width/4 , yPos, Screen.width / 2, 100), "message");
        yPos += 20;

        GUI.TextArea(new Rect(Screen.width / 4 + leftMargin, yPos, Screen.width / 2 - 2 * leftMargin, 75), message, textStyle);
    }

    public void ShowMessage(string message, int duration)
    {
        gm.messageFlag = true;
        messageEndFrame = Time.frameCount + duration;

        this.message = message;
    }

}
