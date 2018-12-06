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
    public Character Player;
    public GameObject NewGameObject;
    public GameObject NameUI;
    public UITextList ChatBox;
    public GameObject MessageBox;


    // 우편함 !!
    public Window PostWindow;
    public GameObject PostItem;
    public GameObject PostItems;
    public UILabel PostCount;

    public Window NewStudyWindow;

    public KeywordWindow KeywordWindow;

    public InventoryWindow InventoryWindow;

    public FileMenu fileMenu;
    public UIGrid SocketFileGrid;
    public SocketFile SocketFile;

    public StudySignUpWindow StudySignUpWindow;
    public StudyWindow StudyWindow;

    public UIGrid HomeworkGrid;
    public HomeworkItem HomeworkItem;
    void Start()
    {

        // 들어온 다음에 로딩 완료 메세지를 보낸다.
        JObject json = new JObject();
        json["type"] = NetworkProtocol.EnterWorld;
        NetworkMain.SendMessage(json);
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
