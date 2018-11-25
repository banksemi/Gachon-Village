using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class KeywordWindow : Window {

    public UIGrid Items;
    public UILabel label;
    public UIScrollBar UIScrollBar;

    public UIInput InputBox;
    public void NewList(JArray jArray)
    {
        // 모든 정보 초기화
        while (Items.transform.childCount > 0)
        {
            DestroyImmediate(Items.GetChild(0).gameObject);
        }
        foreach (string data in jArray)
        {
            UILabel newitem = Instantiate(label, Items.transform);
            newitem.text = data;
        }
        Items.GetComponent<UIWidget>().height = (int)(Items.cellHeight * jArray.Count);
        Items.repositionNow = true;
        UIScrollBar.value = 0;
    }
    public void KeywordAddButton()
    {
        JObject json = new JObject();
        json["type"] = NetworkProtocol.Keyword_Add;
        json["keyword"] = InputBox.value;
        NetworkMain.SendMessage(json);
        InputBox.value = "";
    }
}
