using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
public class StudyWindow : Window {
    public string key;
    public Transform TabList;
    public UIGrid MemberList_Level0;
    public UIGrid MemberList_Level1;
    public MemberItem Prefab_Member;
    public void MoveTabSend(GameObject tab)
    {
        JObject json = new JObject();
        json["type"] = NetworkProtocol.Study_UI;
        json["tab"] = tab.name;
        json["name"] = key;
        NetworkMain.SendMessage(json);
    }
    public void TabChange(JObject json)
    {
        string name = (string)json["tab"];
        // 탭 버튼 활성화
        for (int i = 0; i < TabList.childCount; i++)
        {
            Transform button = TabList.GetChild(i);
            if (button.name == name)
            {
                button.GetComponent<UIButton>().isEnabled = false;
            }
            else
            {
                button.GetComponent<UIButton>().isEnabled = true;
            }
        }

        // Member 정보 갱신
        if (name=="Member")
        {
            Reset_MemberList();
            foreach (JObject item in (JArray)json["items"])
            {
                Add_MemberList(item);
            }
            UpdateList();
        }
        base.TabChange(name);
    }
    public void Reset_MemberList()
    {

        while (MemberList_Level1.transform.childCount > 0)
        {
            DestroyImmediate(MemberList_Level1.transform.GetChild(0).gameObject);
        }
        while (MemberList_Level0.transform.childCount > 0)
        {
            DestroyImmediate(MemberList_Level0.transform.GetChild(0).gameObject);
        }
    }
    public void Add_MemberList(JObject json)
    {
        MemberItem item = null;
        if ((int)json["level"] == 1)
            item = Instantiate(Prefab_Member,MemberList_Level1.transform);
        else
            item = Instantiate(Prefab_Member, MemberList_Level0.transform);
        item.Set(json);
    }
    public void UpdateList()
    {
        MemberList_Level0.repositionNow = true;
        MemberList_Level1.repositionNow = true;
    }
}
