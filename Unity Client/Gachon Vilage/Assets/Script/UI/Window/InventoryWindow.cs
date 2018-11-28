using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class InventoryWindow : Window
{
    public Dictionary<int, FileItem> Items = new Dictionary<int, FileItem>();
    public UIGrid Items_Form;
    public FileItem Prefab_Item;
    public void Add(JObject json)
    {
        FileItem item = Instantiate(Prefab_Item, Items_Form.transform);
        item.UI_title.text = (string)json["name"];
        item.UI_size.text = "" + (long)json["size"];
        item.UI_date.text = ((DateTime)json["date"]).ToShortDateString();
        item.no = (int)json["no"];
        Items.Add((int)json["no"], item);
        if (isOpen()) Items_Form.repositionNow = true;
    }

    public void Remove(int no)
    {
        FileItem item = Items[no];
        DestroyImmediate(item.gameObject);
        if (isOpen()) Items_Form.repositionNow = true;

    }
}
