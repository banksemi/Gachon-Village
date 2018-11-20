using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour {
    private Window parentwindow;
	// Use this for initialization
	void Start () {
        parentwindow = transform.parent.GetComponent<Window>();
    }
    void OnClick()
    {
        parentwindow.Close();
    }
}
