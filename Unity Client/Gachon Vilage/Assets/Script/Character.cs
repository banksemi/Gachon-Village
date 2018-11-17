using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    private MessageBox Message;
    private float MessageTime = 0;
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
    public void ChatMessage(string message)
    {
        if (Message != null) Destroy(Message.gameObject);
        Message = Instantiate(Preset.objects.MessageBox).GetComponent<MessageBox>();
        Message.Set(Name, message);
        Message.transform.parent = GameObject.FindWithTag("CCG").transform;
        Message.transform.localScale = new Vector3(1, 1, 1);
        MessageTime = 0;
    }
    void Update()
    {
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
                // transform.position = nextmove;
            }
            movetime += Time.deltaTime;
        }
    }
    void LateUpdate()
    {
        MessageTime += Time.deltaTime;
        if (Message != null && MessageTime >= 2f)
        {
            Destroy(Message.gameObject);
            Message = null;
        }
        Vector3 aa = col.bounds.center;
        aa.y += col.bounds.size.y / 2 + 0.5f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(aa);
        if (screenPos.z < 0 || screenPos.z > 100)
        {
            label.gameObject.SetActive(false);
        }
        else
        {
            screenPos.x = (int)screenPos.x;
            screenPos.y = (int)screenPos.y;
            if (screenPos.z > 80)
            {
                Color a = label.color;
                a.a = (100 - screenPos.z) * 5 / 100f;
                label.color = a;
            }
            if (Message == null)
            {
                screenPos.y += 15;
                label.transform.localPosition = screenPos;
                label.gameObject.SetActive(true);
            }
            else
            {
                screenPos.y += 35;
                Message.transform.localPosition = screenPos;
                Message.gameObject.SetActive(true);
                label.gameObject.SetActive(false);
            }
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
