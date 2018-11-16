using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
public class ChatInputBox : MonoBehaviour
{
    UIInput mInput;
    // Use this for initialization
    void Start()
    {
        mInput = GetComponent<UIInput>();
        mInput.label.maxLineCount = 1;
    }
    void Send()
    {
        mInput.isSelected = false;
        string text = NGUIText.StripSymbols(mInput.value);
        mInput.value = "";
        if (!string.IsNullOrEmpty(text))
        {
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Chat;
            json["message"] = text;
            NetworkMain.SendMessage(json);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (UIInput.selection == null)
            {
                mInput.isSelected = true;
                if (!string.IsNullOrEmpty(mInput.value)) Send();
            }
            else if (UIInput.selection == mInput)
            {
                Send();
            }
        }
    }
}