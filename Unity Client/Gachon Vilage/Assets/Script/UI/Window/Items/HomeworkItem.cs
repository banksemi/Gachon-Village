using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class HomeworkItem : MonoBehaviour {
    public UILabel Title;
    public UILabel Date;
    public DateTime time;
    int delay = 0;
	// Update is called once per frame
	void Update () {
        if (delay++ == 300)
        {
            if (time.DayOfYear < DateTime.Now.DayOfYear)
            {
                Destroy(gameObject);
            }
            delay = 0;
        }
	}
}
