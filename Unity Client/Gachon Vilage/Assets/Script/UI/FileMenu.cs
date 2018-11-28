using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
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
}
