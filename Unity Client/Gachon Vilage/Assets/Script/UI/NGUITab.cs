using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGUITab : MonoBehaviour
{
    public List<UIInput> UIInputs;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (UIInput.selection)
            {
                for (int i = 0; i < UIInputs.Count; i++)
                {
                    if (UIInputs[i] == UIInput.selection)
                    {
                        UIInputs[(i + 1) % UIInputs.Count].isSelected = true;
                        return;
                    }
                }
            }
            else
            {
                UIInputs[0].isSelected = true;
            }
        }
	}
}
