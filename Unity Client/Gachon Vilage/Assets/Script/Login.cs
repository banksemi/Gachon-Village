using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;
public class Login : MonoBehaviour
{
    public UIInput ID;
    public UIInput Password;
    public UIToggle IDToggle; 
    // Use this for initialization
    void Start () {
        if (File.Exists("idsave.txt"))
        {
            string id = File.ReadAllText("idsave.txt");
            ID.value = id;
            IDToggle.value = true;

            Password.isSelected = true;
        }
        else
        {
            ID.isSelected = true;
        }
	}
	public void IDToggleChange()
    {
        if (IDToggle.value == true)
        {
            File.WriteAllText("idsave.txt", ID.value);
        }
        else
        {
            if (File.Exists("idsave.txt")) File.Delete("idsave.txt");
        }
    }
    public void ChangeValue()
    {
        if (IDToggle.value == true)
        {
            File.WriteAllText("idsave.txt", ID.value);
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
    public void OnClick()
    {
        JObject json = new JObject();
        json["type"] = NetworkProtocol.Login;
        json["id"] = ID.value;
        json["password"] = Password.value;
        NetworkMain.SendMessage(json);
    }
}
