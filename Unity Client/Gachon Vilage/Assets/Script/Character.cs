using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    private UILabel label;
    private Collider col;
    public int No;
    private string _name;
    private List<Vector3> movelist = new List<Vector3>();
    public float movetime = 1;
    public Vector3 startmove;
    public Vector3 nextmove;
    public bool LastMove = false;
    public string Name
    {
        get { return _name; }
        set { _name = value; if (label != null) { label.text = _name; } }
    }
    public string ID;
    void Start()
    {
        col = transform.GetComponent<Collider>();
        label = Instantiate(Preset.objects.NameUI).GetComponent<UILabel>();
        label.transform.parent = GameObject.FindWithTag("CCG").transform;
        label.text = Name;
        label.transform.localScale = new Vector3(1, 1, 1);
    }
    void Update()
    {
        Vector3 aa = col.bounds.center;
        aa.y += col.bounds.size.y / 2 + 0.5f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(aa);
        if (screenPos.z < 0 || screenPos.z > 100)
        {
            label.gameObject.SetActive(false);
        }
        else
        {
            if (screenPos.z > 80)
            {
                Color a = label.color;
                a.a = (100 - screenPos.z) * 5 / 100f;
                label.color = a;
            }

            screenPos.y += 15;
            label.transform.localPosition = screenPos;
            label.gameObject.SetActive(true);
        }
        // 캐릭터 움직임
        if (No != NetworkMain.myNo)
        {
            if (movetime >= NetworkMain.MoveDeley)
            {
                if (movelist.Count > 0)
                {
                    startmove = transform.position;
                    nextmove = movelist[0];
                    movelist.RemoveAt(0);
                    if (LastMove == true)
                        movetime -= NetworkMain.MoveDeley;
                    else
                        movetime = 0;
                }
            }
            LastMove = false;
            if (movetime <= NetworkMain.MoveDeley)
            {
                transform.position += (nextmove - startmove) * (Time.deltaTime) / NetworkMain.MoveDeley;
                LastMove = true;
                // transform.position = Vector3.Lerp(startmove, nextmove, movetime / NetworkMain.MoveDeley);
            }
            else if (movelist.Count == 0 && movetime > NetworkMain.MoveDeley * 2)
            {
                //Debug.Log("ㅇㄴㄻㅉㄸㄻ");
               // transform.position = nextmove;
            }
            movetime += Time.deltaTime;
            Debug.Log(movelist.Count);
        }

    }
    void OnDestroy()
    {
        Destroy(label.gameObject);
    }

    public void Move(Vector3 vector3)
    {
        if (movelist.Count == 10)
        {
            movelist.RemoveAt(0);
        }
        movelist.Add(vector3);
    }
}
