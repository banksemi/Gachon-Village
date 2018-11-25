using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBox : MonoBehaviour {
    public UILabel name_p;
    public UILabel content;
    public UISprite back;
    public SetMinimumSize set;
    public void Set(string name, string message)
    {
        name_p.text = name;
        content.text = message;
        set.Update();
    }
    public void SetDepth(int value)
    {
        
        name_p.depth = value;
        content.depth = value;
        back.depth = value - 1;
    }
}
