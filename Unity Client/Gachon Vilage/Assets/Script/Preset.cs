using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json.Linq;

class Preset : MonoBehaviour
{
    static Preset preset;
    public static Preset objects
    {
        get
        {
            if (preset == null) preset = GameObject.Find("Preset").GetComponent<Preset>();
            return preset;
        }
    }
    public GameObject NewGameObject;
    public GameObject NameUI;
    public UITextList ChatBox;
    public GameObject MessageBox;


    // 우편함 !!
    public Window PostWindow;
    public GameObject PostItem;
    public GameObject PostItems;
    public UILabel PostCount;
    void Start()
    {
        
    }
    public void PostItem_Add(JObject json)
    {
        GameObject item = Instantiate(PostItem);
        item.GetComponent<PostItem>().Set(json);
        item.transform.parent = PostItems.transform;
        item.transform.localScale = new Vector3(1, 1, 1);
    }
    public void PostItem_Reset()
    {
        while (PostItems.transform.childCount > 0)
        {
            DestroyImmediate(PostItems.transform.GetChild(0).gameObject);
        }
    }
    public void OpenPostWindow()
    {
        PostWindow.Open();
    }
}
