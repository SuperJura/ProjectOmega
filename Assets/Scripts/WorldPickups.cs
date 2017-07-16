using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPickups : MonoBehaviour {
	void Start () {
        for (int i = 0; i < transform.childCount; i++)
        {
            Ammo ammo = transform.GetChild(i).GetComponent<Ammo>();
            ammo.Init(ammo.type, ammo.amount);
        }
	}
}
