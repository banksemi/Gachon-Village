using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
public class PostWindowDetail : MonoBehaviour {
    int no = -1;
    public UIScrollBar scrollBar;
    public UILabel title_detail;
    public UILabel sender_detail;
    public UILabel date_detail;
    public UILabel Content_detail;
    public void Set(JObject json)
    {
        no = (int)json["no"];
        title_detail.text = "제목 : " + json["title"];
        sender_detail.text = "보낸사람 : " + json["sender"] + " (" + json["sender_id"] + ")";
        date_detail.text = (string)json["date"];
        Content_detail.text = (string)json["content"];
        scrollBar.value = 0;
    }
    public void ClickContent()
    {
        string url = Content_detail.GetUrlAtPosition(UICamera.lastHit.point);
        if (!string.IsNullOrEmpty(url))
        {
            JObject json = new JObject();
            json["type"] = NetworkProtocol.GetFileInPost;
            json["post_no"] = no;
            json["file_no"] = url.Replace("file-", "");
            NetworkMain.SendMessage(json);
        }
    }
    // Use this for initialization
}
