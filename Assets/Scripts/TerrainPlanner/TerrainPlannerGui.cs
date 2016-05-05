using UnityEngine;
using System.Collections;

public class TerrainPlannerGui : MonoBehaviour
{

    public GUIterrainPatch tp;
    bool plannerActive = true;


    // Use this for initialization
    void Start()
    {
        tp = new GUIterrainPatch(64);//TODO: set patch size
        DontDestroyOnLoad(transform.gameObject);
    }

    void OnGUI()
    {

        if (plannerActive)
        {
            tp.OnGui();
            if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height*0.9f, 150, 40), "GENERATE"))
            {
                Debug.Log("GENERATE");
                plannerActive = false;
                Application.LoadLevel("testScene");
            }

        }
    }
}
