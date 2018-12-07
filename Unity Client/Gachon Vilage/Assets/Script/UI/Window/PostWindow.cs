using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
public class PostWindow : Window
{
    public int page;
    public UILabel pagelabel;

    public UIInput Write_Title;
    public UIInput Write_Receiver;
    public UIInput Write_Content;
    public UILabel Write_File;
    private string File_First_Info = "인벤토리에서 아이템을 더블클릭해주세요";
    public int File_number;
    public void LeftPage()
    {
        Page(page - 1);
    }
    public void RightPage()
    {
        Page(page + 1);
    }
    public void Page(int newpage)
    {
        JObject json = new JObject();
        json["type"] = NetworkProtocol.Post_Open;
        json["page"] = newpage;
        NetworkMain.SendMessage(json);
    }
    public void InputItem(int no, string title)
    {
        File_number = no;
        Write_File.text = title;
    }
    public void ResetFile()
    {
        Write_File.text = File_First_Info;
        File_number = -1;
    }
    public void WriteTab()
    {
        Write_Title.value = "";
        Write_Receiver.value = "";
        Write_Content.value = "";
        ResetFile();
        TabChange("Write");
    }

    public void SendPost()
    {
        JObject json = new JObject();
        json["type"] = NetworkProtocol.SendPost;
        json["title"] = Write_Title.value;
        json["content"] = Write_Content.value;
        json["receiver"] = Write_Receiver.value;
        if (File_number != -1)
        {
            json["file"] = File_number;
        }
        NetworkMain.SendMessage(json);
    }
}
