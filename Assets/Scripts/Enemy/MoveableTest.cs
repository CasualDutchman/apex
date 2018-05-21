using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableTest : MonoBehaviour, IMoveable, IAttackable {

    public void Damage(float f) {
        Debug.Log("Damage");
    }

    public bool IsAlive() {
        return true;
    }

    public bool OnMove() {
        return false;
    }

}
