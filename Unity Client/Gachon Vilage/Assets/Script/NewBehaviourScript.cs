using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    public float NewTime = 1f;
    public float DisposeTime = 1f;
    public float OnTime = 2f;
    public float Time2 = 0f;
    public int cas = 0;
    public UISprite UI;
    // Use this for initialization
    void Start ()
    {
        UI = GetComponent<UISprite>();
        transform.localPosition = new Vector3(0, 293, 0);
    }
	void Set(float a)
    {
        UI.color = new Color(1, 1, 1, a);
       
    }
    void UpCase()
    {
        cas++;
        Time2 = 0;
    }
	// Update is called once per frame
	void Update () {
        if (cas == 0)
        {
            Set(Time2 / NewTime);
            if (Time2 >= NewTime) UpCase();
        }
        else if (cas == 1)
        {
            if (Time2 >= OnTime) UpCase();
        }
        else if (cas == 2)
        {
            Set(1 - Time2 / DisposeTime);
            if (Time2 >= DisposeTime) UpCase();
        }
        Time2 += Time.deltaTime;
        if (cas == 3) Destroy(this);
	}
}
