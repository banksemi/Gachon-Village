using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
public class PostWindow : Window
{
    public int page;
    public UILabel pagelabel;
    public void LeftPage()
    {
        Page(page - 1);
    }
    public void RightPage()
    {
        Page(page + 1);
    }
    public void Page(int newpage)
    {
        JObject json = new JObject();
        json["type"] = NetworkProtocol.Post_Open;
        json["page"] = newpage;
        NetworkMain.SendMessage(json);
    }

}
