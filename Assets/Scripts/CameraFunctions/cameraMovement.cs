﻿using UnityEngine;
using System.Collections;

//CAMERA MOVEMENT SCRIPT
//W,A,S,D, UP,LEFT,DOWN,RIGHT
//R,F -> ZOOM IN AND OUT

public class cameraMovement : MonoBehaviour {
	
	public float playerSpeed = 3.0f;
	public Vector3 camera_pos;

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
            //gameObject.GetComponent<MouseLook>().enabled = !gameObject.GetComponent<MouseLook>().enabled;
            lastActionFrame = Time.frameCount;
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
}
