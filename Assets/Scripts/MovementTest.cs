using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
        transform.Translate(new Vector3(6, 0, 5) * Time.deltaTime);
	}
}
