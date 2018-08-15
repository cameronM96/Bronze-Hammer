using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBarManager : MonoBehaviour {

	public Image Health_Bar_Image;
	public float Health;
	public float Start_Health;
    public Camera cameraToLookAt;


	public void reduceHealth(float damage) {
        
        Health -= damage;
        Health_Bar_Image.fillAmount = Health / Start_Health;
	}

	public float getStartHealth() {
		return Start_Health;
	}

	public float getHealth() {
		return Health;
	}

	public void setStartHealth(float amount) {
		Start_Health = amount;
	}

	public void setHealth(float amount) {
		Health = amount;
	}

    void Update()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(cameraToLookAt.transform.position - v);
        transform.Rotate(0, 180, 0);
    }
}
