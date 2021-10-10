using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour {

    public GameObject myCamera;
    public GameObject[] cameras;

    // Use this for initialization
    void Start () {

        cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        
		
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider other) {


        if (other.CompareTag("Player"))
        {

            DeactivateAllCameras();
            myCamera.SetActive(true);
            Destroy(this);
        }


		
	}

    public void DeactivateAllCameras()
    {

        for (int i = 0; i < cameras.Length; i++)
        {

            cameras[i].SetActive(false);
        }


    }
}
