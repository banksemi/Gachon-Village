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
    // Use this for initialization
    void Start () {
		
	}
    public void Set(JObject json)
    {
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
	
	// Update is called once per frame
	void Update () {
		
	}
}
