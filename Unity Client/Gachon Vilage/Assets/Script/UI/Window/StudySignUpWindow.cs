using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
public class StudySignUpWindow : Window {
    private string _key = null;
    public string key
    {
        get
        {
            return _key;
        }
        set
        {
            _key = value;
            Title.text = "\"" + _key + "\"";
        }
    }
   
    public UILabel Title;
    public void Open(string key)
    {
        this.key = key;
        base.Open();
    }
    public void Click()
    {
        JObject json = new JObject();
        json["type"] = NetworkProtocol.Study_SignUp;
        json["name"] = key;
        NetworkMain.SendMessage(json);
    }
}
