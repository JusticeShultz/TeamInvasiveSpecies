using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poision : MonoBehaviour {
public float dps = 1;
public float radius = 10;
	void Update()
    {
        if(Vector3.Distance(PlayerController.Player_Controller.transform.position, transform.position)<radius){
            PlayerController.Player_Controller.TakeDamage(dps*Time.deltaTime, transform.position, true);
        }
    }
}
