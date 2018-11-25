using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
public class KeywordItem : MonoBehaviour {
    public UILabel label;
    // Use this for initialization
    void OnClick()
    {
        if (UICamera.currentTouchID == -2)
        {
            JObject sjson = new JObject();
            sjson["type"] = NetworkProtocol.Keyword_Remove;
            sjson["keyword"] = label.text;
            NetworkMain.SendMessage(sjson);
        }
    }
}
