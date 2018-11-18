using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
public class PostWindowDetail : MonoBehaviour {
    public UIScrollBar scrollBar;
    public UILabel title_detail;
    public UILabel sender_detail;
    public UILabel date_detail;
    public UILabel Content_detail;
    public void Set(JObject json)
    {
        title_detail.text = "제목 : " + json["title"];
        sender_detail.text = "보낸사람 : " + json["sender"] + " (" + json["sender_id"] + ")";
        date_detail.text = (string)json["date"];
        Content_detail.text = (string)json["content"];
        scrollBar.value = 0;
    }
    // Use this for initialization
}
