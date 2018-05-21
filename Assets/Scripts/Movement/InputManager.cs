using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour{

    public float inputMultiplier;

    Vector3 input = Vector3.zero;
    Vector3 begin, dir;

    void Start () {

    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            begin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0)) {
            Vector3 drag = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            dir = drag - begin;
            dir *= inputMultiplier;

            dir = dir.magnitude > 1 ? dir.normalized : dir;
            input = new Vector3(dir.x, 0, dir.y);
        }
        if (Input.GetMouseButtonUp(0)) {
            input = Vector3.zero;
        }
    }

    public Vector3 GetInputVector() {
        return input;
    }  
}
