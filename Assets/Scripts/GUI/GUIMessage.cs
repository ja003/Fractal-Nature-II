﻿using UnityEngine;
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
        gm.cm.guiMessage = this;
    }

    public void OnGui(int yPosition)
    {
        int yPos = yPosition;
        GUI.color = new Color(0, 0, 0, 1);
        GUI.Box(new Rect(Screen.width/4 , yPos, Screen.width / 2, 100), "message");
        yPos += 20;

        GUI.TextArea(new Rect(Screen.width / 4 + leftMargin, yPos, Screen.width / 2 - 2 * leftMargin, 75), message, textStyle);
    }

    /// <summary>
    /// shows message for 3 sec
    /// </summary>
    public void ShowMessage(string message)
    {
        ShowMessage(message, 100);
    }

    /// <summary>
    /// shows message for 'duration' number of frames
    /// </summary>
    public void ShowMessage(string message, int duration)
    {
        gm.messageFlag = true;
        messageEndFrame = Time.frameCount + duration;

        this.message = message;
    }

}
