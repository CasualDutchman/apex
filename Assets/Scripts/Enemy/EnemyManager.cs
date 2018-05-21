using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public List<GameObject> preys = new List<GameObject>();
    public List<GameObject> predators = new List<GameObject>();

    Dictionary<Vector3, GameObject> enemyDictionary = new Dictionary<Vector3, GameObject>();

	public void UpdateEnemyList(int posX, int posY) {
        for (int y = 0; y < 10; y++) {
            for (int x = 0; x < 10; x++) {

            }
        }
    }
}
