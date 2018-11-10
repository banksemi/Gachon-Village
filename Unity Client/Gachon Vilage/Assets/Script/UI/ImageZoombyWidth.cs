using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageZoombyWidth : MonoBehaviour {

    private UIWidget UI;
    // Use this for initialization
    void Start ()
    {
        UI = GetComponent<UIWidget>();
        UI.width = Screen.width;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
