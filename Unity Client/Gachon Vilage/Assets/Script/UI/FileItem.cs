using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class FileItem : MonoBehaviour {
    public int no;
    public UILabel UI_title;
    public UILabel UI_size;
    public UILabel UI_date;
    void OnClick()
    {
        // 우클릭이면
        if (UICamera.currentTouchID == -2)
        {
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Inventory_Remove;
            json["no"] = no;
            NetworkMain.SendMessage(json);
        }
    }
}
