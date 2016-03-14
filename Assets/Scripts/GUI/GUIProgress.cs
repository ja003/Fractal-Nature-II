using UnityEngine;
using System.Collections;

public class GUIProgress {

    int stepsNumber;

    float barDisplay;//  = 0;
    Vector2 pos = new Vector2(20,40);
    Vector2 size = new Vector2(60,20);
    Texture2D progressBarEmpty;
    Texture2D progressBarFull;


    GUIManager gm;

    /// <summary>
    /// probably not usable
    /// </summary>    
    public GUIProgress(GUIManager gm)
    {
        this.gm = gm;
        barDisplay = 0;
        SetProgress(100);
    }

    public void OnGui()
    {
        Debug.Log(barDisplay);
        // draw the background:
        GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), progressBarEmpty);

        // draw the filled-in part:
        GUI.BeginGroup(new Rect(0, 0, size.x * barDisplay, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), progressBarFull);
        GUI.EndGroup();

        GUI.EndGroup();

        //barDisplay = Time.time * 0.05f;
        //AddToProgress(10);
    }

    public void SetProgress(int stepsNumber)
    {
        this.stepsNumber = stepsNumber;
    }

    public void AddToProgress(int step)
    {
        barDisplay += (float)step / stepsNumber;
        Debug.Log(barDisplay);
        Debug.Log((float)step / stepsNumber);

    }
}
