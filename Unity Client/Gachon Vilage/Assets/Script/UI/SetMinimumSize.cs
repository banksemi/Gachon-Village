using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMinimumSize : MonoBehaviour {
    public int width = 0;
    UIWidget UIWidget;
    UIAnchor UIAnchor;
    // Use this for initialization
    void Start ()
    {
        UIWidget = GetComponent<UIWidget>();
    }
	
	// Update is called once per frame
	public void Update () {
        if (UIWidget == null) UIWidget = GetComponent<UIWidget>();
        if (UIWidget.width < width) UIWidget.width = width;
	}
}
