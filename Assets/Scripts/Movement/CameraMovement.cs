using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public Transform[] wolfList;
    Vector3 center;

    Transform cameraLook;

	void Start () {
        cameraLook = transform.GetChild(0).GetChild(0);
	}
	
	void Update () {
        center = Vector3.zero;
        foreach (Transform trans in wolfList) {
            center += trans.position;
        }
        center /= wolfList.Length;

        transform.position = Vector3.Lerp(transform.position, center, 0.02f);
        cameraLook.LookAt(center);
	}
}
