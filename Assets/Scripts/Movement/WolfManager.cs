using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfManager : MonoBehaviour {

    public GameObject wolfObj;
    public int amountOfWolves;

    List<WolfMovement> wolfList = new List<WolfMovement>();
    Vector3 wolfCenter;

    public List<Transform> animalList = new List<Transform>();
    Vector3 animalCenter;

    void Start () {
        for (int i = 0; i < amountOfWolves; i++) {
            WolfMovement m = Instantiate(wolfObj).GetComponent<WolfMovement>();
            Vector2 ran = Random.insideUnitCircle * amountOfWolves;
            m.transform.position = new Vector3(ran.x, 0, ran.y);
            m.transform.eulerAngles = new Vector3(0, Random.Range(0f, 360f), 0);
            m.manager = this;
            wolfList.Add(m);
            animalList.Add(m.transform);
        }
	}
	
    public void KillEnemy(Transform c) {
        foreach (WolfMovement wolf in wolfList) {
            wolf.KilledEnemy(c);
        }
    }

	void Update () {
        wolfCenter = Vector3.zero;
        foreach (WolfMovement wolf in wolfList) {
            wolfCenter += wolf.transform.position;
        }
        wolfCenter /= wolfList.Count;

        animalCenter = Vector3.zero;
        foreach (Transform animal in animalList) {
            animalCenter += animal.position;
        }
        animalCenter /= animalList.Count;
    }

    public Vector3 GetRandomCenter() {
        Vector2 ran = Random.insideUnitCircle * amountOfWolves;
        if(ran.magnitude < 2f)
            ran *= 2;
        return wolfCenter + new Vector3(ran.x, 0, ran.y);
    }

    public Vector3 GetCenter() {
        return wolfCenter;
    }
}
