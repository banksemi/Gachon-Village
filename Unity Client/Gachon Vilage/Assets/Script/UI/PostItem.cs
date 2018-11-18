using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class PostItem : MonoBehaviour {
    public UILabel title;
    public UILabel date;
    public UILabel sender;
    public UILabel content;
    private JObject json;
	// Use this for initialization
    public void Set(JObject json)
    {
        this.json = json;
        title.text = json["title"].ToString();
        date.text = json["date"].ToString();
        sender.text = json["sender"].ToString();
        content.text = json["content"].ToString();

        if ((int)json["read"] == 1)
        {
            title.color = Color.black;
        }
    }
    void OnClick()
    {
        JObject sjson = new JObject();
        sjson["type"] = NetworkProtocol.Post_Detail;
        sjson["no"] = json["no"];
        NetworkMain.SendMessage(sjson);
    }
}
