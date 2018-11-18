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
    public GameObject PostWindow;
    public GameObject PostItem;
    public GameObject PostItems;
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
        for (int i = 0; i < PostItems.transform.childCount; i++)
        {
            Destroy(PostItems.transform.GetChild(i).gameObject);
        }
    }
}
