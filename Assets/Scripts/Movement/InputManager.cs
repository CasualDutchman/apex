using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    Vector3 input = Vector3.zero;

	void Start () {
		
	}
	
	void Update () {
        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	}

    public Vector3 GetInputVector() {
        return input;
    }
}
