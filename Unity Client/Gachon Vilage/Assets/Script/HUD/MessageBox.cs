using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBox : MonoBehaviour {
    public UILabel name_p;
    public UILabel content;
    public SetMinimumSize set;
    public void Set(string name, string message)
    {
        name_p.text = name;
        content.text = message;
        set.Update();
    }

}
