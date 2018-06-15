using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public WolfManager manager;
    Transform cameraLook;

	void Start () {
        cameraLook = transform.GetChild(0).GetChild(0);
    }
	
	void Update () {
        transform.position = Vector3.Lerp(transform.position, manager.GetCenter(), Time.deltaTime);
        cameraLook.LookAt(manager.GetCenter());
	}
}
