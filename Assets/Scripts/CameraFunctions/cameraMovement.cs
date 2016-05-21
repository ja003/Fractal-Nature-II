using UnityEngine;
using System.Collections;

//CAMERA MOVEMENT SCRIPT
//W,A,S,D, UP,LEFT,DOWN,RIGHT
//R,F -> ZOOM IN AND OUT

public class cameraMovement : MonoBehaviour {
	
	public float playerSpeed = 3.0f;
	public Vector3 camera_pos;
    public GUICamera gui_camera;

    int lastActionFrame = 0;

    // Update is called once per frame
    void Update () {
		
		transform.Translate (Vector3.right * Input.GetAxis("Horizontal") * playerSpeed);
		transform.Translate (Vector3.forward * Input.GetAxis("Vertical") * playerSpeed);
        
        if (Input.GetKey("c") && lastActionFrame < Time.frameCount - 30)
        {
            if (gameObject.GetComponent<MouseLook>().enabled)
                gameObject.GetComponent<MouseLook>().enabled = false;
            else
                gameObject.GetComponent<MouseLook>().enabled = transform;
            lastActionFrame = Time.frameCount;
        }

        if (Input.GetKey("p") && lastActionFrame < Time.frameCount - 30)
        {
            Debug.Log("PERSPECTIVE");
            ChangeProjection();
            lastActionFrame = Time.frameCount;
        }
        if (Input.GetKey("s"))
        {
            transform.GetComponent<Camera>().orthographicSize = transform.GetComponent<Camera>().orthographicSize+1;
        }
        if (Input.GetKey("w"))
        {
            transform.GetComponent<Camera>().orthographicSize = transform.GetComponent<Camera>().orthographicSize - 1;
        }


        if (Input.GetKey("r")) {
			transform.Translate (Vector3.up * playerSpeed);
		}
		if (Input.GetKey("f")) {
			transform.Translate (Vector3.down * playerSpeed);
		}
        

        camera_pos.x = transform.position.x;
		camera_pos.y = transform.position.y;
		camera_pos.z = transform.position.z;
	}

    bool perspective = true;

    Vector3 lastPosition;
    //Vector3 lastRotation;
    Vector3 lastLocalRotation;

    /// <summary>
    /// switches between orthogonal and perspective projection
    /// </summary>
    public void ChangeProjection()
    {
        if (perspective)
        {
            transform.GetComponent<Camera>().orthographic = true;

            lastPosition = transform.position;
            lastLocalRotation = transform.localRotation.eulerAngles;

            transform.localEulerAngles = new Vector3(90, 0, 0);
            transform.position = new Vector3(0, 100, 0);
            transform.GetComponent<Camera>().orthographicSize = 50;

            gui_camera.projectionString = "orthographic";
        }
        else
        {
            transform.GetComponent<Camera>().orthographic = false;
            transform.position = lastPosition;
            transform.localEulerAngles = lastLocalRotation;
            gui_camera.projectionString = "perspective";
        }

        perspective = !perspective;
    }

    /// <summary>
    /// changes MainCamera's position
    /// </summary>
    public void ChangePosition(Vector3 position)
    {
        transform.position = position;
    }

    /// <summary>
    /// changes MainCamera's position
    /// </summary>
    public void ChangeRotation(Vector3 rotation)
    {
        transform.localEulerAngles = rotation;
    }
}
