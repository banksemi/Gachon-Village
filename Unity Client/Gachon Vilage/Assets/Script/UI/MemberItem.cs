using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class MemberItem : MonoBehaviour
{
    public UIButton Level0_Button;
    public UIButton Level0_Button2;
    public UIButton Level1_Button;
    public UILabel Name;
    public UILabel Department;
    public UILabel Number;
    public UILabel Email;
    public string id;
    public void Remove_Button()
    {
        JObject json = new JObject();
        json["type"] = NetworkProtocol.Study_Member_Request;
        json["name"] = Preset.objects.StudyWindow.key;
        json["id"] = id;
        json["positive"] = false;
        NetworkMain.SendMessage(json);
    }
    public void Accept_Button()
    {
        JObject json = new JObject();
        json["type"] = NetworkProtocol.Study_Member_Request;
        json["name"] = Preset.objects.StudyWindow.key;
        json["id"] = id;
        json["positive"] = true;
        NetworkMain.SendMessage(json);
    }
    public void Set(JObject json)
    {
        id = (string)json["student_id"];
        if ((int)json["level"] == 0)
        {
            Level0_Button.gameObject.SetActive(true);
            Level0_Button2.gameObject.SetActive(true);
        }
        else
        {
            Level1_Button.gameObject.SetActive(true);
        }
        Name.text = json["name"] + " (" + json["student_id"] + ")";
        Department.text = (string)json["department"];
        Email.text = (string)json["email"];
        Number.text = (string)json["no"];
    }
}
