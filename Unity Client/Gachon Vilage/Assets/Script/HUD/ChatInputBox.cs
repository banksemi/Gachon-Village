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

    public void OnSubmit()
    {
        string text = NGUIText.StripSymbols(mInput.value);

        if (!string.IsNullOrEmpty(text))
        {
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Chat;
            json["message"] = text;
            mInput.value = "";
            mInput.isSelected = false;
            NetworkMain.SendMessage(json);
        }
    }
}