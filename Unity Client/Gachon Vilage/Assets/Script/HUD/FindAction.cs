using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class FindAction : MonoBehaviour {

    private float dist;
    private Character select_action = null;
    public UILabel label;
    // Use this for initialization
    void Start () {
        InvokeRepeating("getClosestNPC", 0, 0.2f);

    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.F))
        {
            if (select_action != null)
            {
                JObject json = new JObject();
                json["type"] = NetworkProtocol.Action;
                json["no"] = select_action.No;
                json["function"] = select_action.function;
                NetworkMain.SendMessage(json);
            }
        }
	}
    void getClosestNPC()
    {
        select_action = null;
        foreach (Character tNPC in NetworkMain.gameObjects.Values)
        {
            if (tNPC.function == null) continue;
            Vector3 objectPos = tNPC.transform.position;
            dist = (objectPos - transform.position).sqrMagnitude;
            if (dist < 140.0f)
            {
                select_action = tNPC;
            }
        }
        if (select_action != null)
            label.text = "'" + select_action.function + "' 선택 (F키)";
        else
            label.text = "";
    }
}
