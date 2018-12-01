using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class StudyFile : MonoBehaviour
{
    int no;
    public UILabel InfoLabel;
    public UILabel FileName;
    public UILabel FileDetail;
    public UILabel Size;

    public void Set(JObject json)
    {
        no = (int)json["no"];
        InfoLabel.text = ((DateTime)json["upload_date"]).ToString("yyyy-MM-dd") + " " + json["upload_user"] + " 님이 등록";
        FileName.text = (string)json["name"];
        FileDetail.text = ((DateTime)json["file_date"]).ToString("yyyy-MM-dd") + " From " + json["owner"];
        Size.text = json["size"].ToString();
    }
    public void OnDoubleClick()
    {
        // 인벤토리로 다운로드 요청
        JObject json = new JObject();
        json["type"] = NetworkProtocol.Study_FileDownload;
        json["group_name"] = Preset.objects.StudyWindow.key;
        json["no"] = no;
        NetworkMain.SendMessage(json);
    }
}
