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
    public string function = null;
    private string _name;
    private string _group;
    private List<Vector4> movelist = new List<Vector4>();
    public float movetime = 1;
    public Vector4 startmove;
    public Vector4 nextmove;
    public bool LastMove = false;
    public string Name
    {
        get { return _name; }
        set { _name = value; if (label != null) { UpdateLabel(); } }
    }
    public string Group
    {
        get { return _group; }
        set { _group = value; if (label != null) { UpdateLabel(); } }
    }
    private void UpdateLabel()
    {
        if (label != null) { label.text = "[88FF73][[c]" + _group + "[/c]][-] " + _name; }
    }
    public string ID;
    void Start()
    {
        label = Instantiate(Preset.objects.NameUI).GetComponent<UILabel>();
        label.transform.parent = GameObject.FindWithTag("CCG").transform;
        UpdateLabel();
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
        if (col == null)
        {
            if (transform.childCount == 0) return;
            else
            {
                col = transform.GetChild(0).GetComponent<Collider>();
            }
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
                Vector4 move = nextmove - startmove;
                transform.position += (Vector3)move * (Time.deltaTime) / NetworkMain.MoveDeley;
                transform.rotation = Quaternion.Euler(0, nextmove.w, 0);
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
        if (col != null)
        {
            MessageTime += Time.deltaTime;
            if (Message != null && MessageTime >= 6f)
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
                if (Message != null) Message.gameObject.SetActive(false);
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
                int z = 100 - (int)(screenPos.z * 2);
                if (Message == null)
                {
                    screenPos.y += 15;
                    label.transform.localPosition = screenPos;
                    label.depth = z;
                    label.gameObject.SetActive(true);

                }
                else
                {
                    screenPos.y += 35;
                    Message.transform.localPosition = screenPos;
                    Message.SetDepth(z);
                    Message.gameObject.SetActive(true);
                    label.gameObject.SetActive(false);
                }
            }
        }
    }
    void OnDestroy()
    {
        if (label != null)
            Destroy(label.gameObject);
        if (Message != null)
            Destroy(Message.gameObject);
    }

    public void Move(Vector4 vector4)
    {
        if (movelist.Count == 10)
        {
            movelist.RemoveAt(0);
        }
        movelist.Add(vector4);
    }
}
