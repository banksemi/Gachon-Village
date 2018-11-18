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
	// Use this for initialization
    public void Set(JObject json)
    {
        title.text = json["title"].ToString();
        date.text = json["date"].ToString();
        sender.text = json["sender"].ToString();
        content.text = json["content"].ToString();
    }
}
