using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfMovement : MonoBehaviour {

    InputManager inputManager;
    CharacterController controller;
    Animator anim;

    public float maxSpeed;
    public Vector3 velocity;

	void Start () {
        anim = GetComponent<Animator>();
        inputManager = GameObject.FindObjectOfType<InputManager>();

    }
	
	void Update () {
        Vector3 input = inputManager.GetInputVector();
        if(input.magnitude > 0) {
            velocity = input * maxSpeed;
        }else {
            //Find Location in pack
            //velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        }

        if(!velocity.Equals(Vector3.zero))
            transform.rotation = Quaternion.LookRotation(velocity);
        transform.Translate(transform.forward * velocity.magnitude * Time.deltaTime, Space.World);

        anim.SetFloat("Blend", velocity.magnitude / maxSpeed);

	}

    void OnTriggerEnter(Collider other) {
        
    }

    void OnTriggerExit(Collider other) {
        
    }
}
