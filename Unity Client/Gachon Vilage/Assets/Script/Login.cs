using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
public class Login : MonoBehaviour
{
    public UIInput ID;
    public UIInput Password;

    // Use this for initialization
    void Start () {
		
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
