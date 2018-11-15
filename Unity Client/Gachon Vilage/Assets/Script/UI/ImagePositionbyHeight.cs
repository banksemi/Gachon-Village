using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePositionbyHeight : MonoBehaviour {
    public float Per = 0;
   
	// Use this for initialization
	void Start () {
        Vector3 position = transform.localPosition;
        position.y = -Screen.height * Per;
        transform.localPosition = position;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
