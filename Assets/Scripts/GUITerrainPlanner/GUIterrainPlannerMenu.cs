using UnityEngine;
using System.Collections;

public class GUIterrainPlannerMenu : MonoBehaviour
{

    public GUIterrainPatch patch;
    public GUIterrainPlanner planner;
    public GUIterrainHint hint;


    bool plannerActive = true;
    bool plannerMenu = true;

    public int menuWidth = 200;
    public int yPos = 10;
    public int rightOffset = 5;
    public int buttonHeight = 40; 
    public int sideOffset = 10;
    public float buttonWidth; 


    // Use this for initialization
    void Start()
    {
        patch = new GUIterrainPatch(64);//TODO: set patch size
        DontDestroyOnLoad(transform.gameObject);
        buttonWidth = menuWidth / 2 - sideOffset - sideOffset / 2;

        planner = new GUIterrainPlanner(this);
        hint = new GUIterrainHint(this);

    }

    void OnGUI()
    {


        if (plannerActive)
        {
            if (GUI.Button(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, buttonHeight), "Procedural Terrain"))
            {
                plannerMenu = !plannerMenu;
            }
            if (plannerMenu)
            {
                planner.OnGui(yPos + buttonHeight + 5);
                hint.OnGui(planner.yPosMax);
            }
            
            patch.OnGui();
            if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height * 0.9f, 150, 40), "GENERATE"))
            {
                Debug.Log("GENERATE");
                plannerActive = false;
                plannerMenu = false;
                Application.LoadLevel("testScene");
            }
        }
    }
}
