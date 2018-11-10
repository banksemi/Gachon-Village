using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetworkLibrary;
using Newtonsoft.Json.Linq;
using System.Net;
public class NetworkMain : MonoBehaviour {
    private LinkedList<JObject> queue = new LinkedList<JObject>();
    private static Client server;
    public GameObject TipMessageObject;
    public static void SendMessage(JObject json)
    {
        server.Send(json);
    }
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        server = new Client("easyrobot.co.kr", 1119);
        server.Connect += Server_Connect;
        server.Receive += Server_Receive;
        server.Start();
    }
    private void TipMessage(string Message)
    {
        GameObject gameObject = Instantiate(TipMessageObject);
        gameObject.transform.parent = GameObject.Find("TIPList").transform;
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        UILabel label = gameObject.transform.GetChild(0).GetComponent<UILabel>();
        gameObject.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
        label.text = Message;
       
    }
    private void Server_Receive(ESocket socket, JObject Message)
    {
        queue.AddLast(Message);
    }

    private void Server_Connect(ESocket socket)
    {
    }
    public void Receive(JObject json)
    {
        switch ((int)json["type"])
        {
            case 1:
                TipMessage((string)json["message"]);
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        while (queue.Count > 0)
        {
            JObject a = queue.First.Value;
            queue.RemoveFirst();
            Receive(a);
        }
    }
    void OnDestroy()
    {
        server.Dispose();

    }
}
