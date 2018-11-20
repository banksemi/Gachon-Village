using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
public class NewStudyWindow : Window {
    public UIInput NameBox;
	// Use this for initialization
    public override void Open()
    {
        base.Open();
        NameBox.value = "";
    }
	public void ButtonClick()
    {
        JObject json = new JObject();
        json["type"] = NetworkProtocol.NewStudy;
        json["name"] = NameBox.value;
        NetworkMain.SendMessage(json);
    }
}
