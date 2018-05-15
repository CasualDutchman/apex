using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour {

    public Vector3 vec = new Vector3(4f, 0, 3f);

    public Transform[] testWolves;

	void Start () {
        RotateWolves();
    }
	
    void RotateWolves() {
        foreach (Transform trans in testWolves) {
            trans.localRotation = Quaternion.LookRotation(vec);
        }
    }

	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            vec = new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f));
            RotateWolves();
        }
        transform.Translate(vec * Time.deltaTime);
	}
}
