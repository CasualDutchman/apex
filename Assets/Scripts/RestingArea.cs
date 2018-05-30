using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestingArea : MonoBehaviour {

    List<Transform> lookList = new List<Transform>();

    public float addedHealth;
    public float addHealthTime;
    float timer;

	void Update () {
        timer += Time.deltaTime;
        if (timer >= addHealthTime) {

            foreach(Transform t in lookList) {
                IAttackable att = t.GetComponent<IAttackable>();
                att.AddHealth(addedHealth);
            }

            timer = 0;
        }
	}

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wolf") || other.gameObject.layer == LayerMask.NameToLayer("Prey") || other.gameObject.layer == LayerMask.NameToLayer("Predator"))
            if (!lookList.Contains(other.transform)) {
                lookList.Add(other.transform);
            }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wolf") || other.gameObject.layer == LayerMask.NameToLayer("Prey") || other.gameObject.layer == LayerMask.NameToLayer("Predator"))
            if (lookList.Contains(other.transform)) {
                lookList.Remove(other.transform);
            }
    }
}
