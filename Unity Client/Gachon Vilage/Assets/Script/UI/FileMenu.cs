using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using SFB;
public class FileMenu : MonoBehaviour {
    public int no;
    public UILabel title;
	// Update is called once per frame
	void LateUpdate () {
        if (Input.GetMouseButtonUp(0))
        {
            gameObject.SetActive(false);
        }
	}
    public void Delete()
    {
        JObject json = new JObject();
        json["type"] = NetworkProtocol.Inventory_Remove;
        json["no"] = no;
        NetworkMain.SendMessage(json);
    }
    public void Save(string path = null, bool open = false)
    {
        if (path == null)
        {
            FileItem item = Preset.objects.InventoryWindow.Items[no];
            // 자동으로 파일 저장위치 고려
            string fname = item.owner + "_" +  item.UI_title.text;
            string name = fname;
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Gachon-Files\\";
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            for(int i = 2; File.Exists(folder + name);i++)
            {
                //확장자 분리
                int temp = fname.LastIndexOf('.');
                if (temp == -1)
                    name = fname + " (" + i + ")";
                else
                    name = fname.Substring(0, temp) + " (" + i + ")" + fname.Substring(temp);
            }
            path = folder + name;
        }
        JObject json = new JObject();
        json["type"] = NetworkProtocol.File_Download;
        json["no"] = no;
        json["path"] = path;
        json["open"] = open;
        NetworkMain.SendMessage(json);
    }
    public void SaveAs()
    {
        // IME를 꺼주는 이유 : 한글 입력상태일때 활성 윈도우가 바뀌면 플레이어 이동 키 입력이 안되기때문에
        Input.imeCompositionMode = IMECompositionMode.Auto;
        FileItem item = Preset.objects.InventoryWindow.Items[no];
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Gachon-Files\\";
        int type = item.UI_title.text.LastIndexOf('.');
        string name = "";
        string type_s = "";
        if (type == -1) name = item.UI_title.text;
        else
        {
            name = item.UI_title.text.Substring(0, type);
            type_s = item.UI_title.text.Substring(type+1);
        }
        StandaloneFileBrowser.SaveFilePanelAsync("Save File", folder, name, type_s, delegate(string path)
        {
            Save(path);
            gameObject.SetActive(false);
        });
    }
    public void Open()
    {
        Save(null, true);
    }
}
