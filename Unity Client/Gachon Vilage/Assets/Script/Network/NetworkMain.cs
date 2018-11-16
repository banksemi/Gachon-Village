using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetworkLibrary;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Threading;
using UnityEngine.SceneManagement;
public class NetworkMain : MonoBehaviour {
    private LinkedList<JObject> queue = new LinkedList<JObject>();
    private static Client server;
    private Thread thread;
    private Dictionary<int, Character> gameObjects = new Dictionary<int, Character>();
    public GameObject TipMessageObject;
    public GameObject NewGameObject;
    public static int myNo;
    public static float MoveDeley = 0.1f;
    public static void SendMessage(JObject json)
    {
        server.Send(json);
    }
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        thread = new Thread(
            delegate ()
            {
                server = new Client("easyrobot.co.kr", 1119);
                server.Connect += Server_Connect;
                server.Receive += Server_Receive;
                server.Start();
            });
        thread.Start();
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
    private static bool isPlayer(JObject json)
    {
        return myNo == (int)json["no"];
    }
    private Character GetGameObject(JObject json)
    {
        if (isPlayer(json) && !gameObjects.ContainsKey((int)json["no"]))
        {
            gameObjects.Add(myNo, GameObject.Find("Player").GetComponent<Character>());
        }
        return gameObjects[(int)json["no"]];
    }
    public void Receive(JObject json)
    {
        Debug.Log(json);
        Character character;
        switch ((int)json["type"])
        {
            case NetworkProtocol.TipMessage:
                TipMessage((string)json["message"]);
                break;
            case NetworkProtocol.EnterWorld:
                myNo = (int)json["no"];
                SceneManager.LoadScene("Game");
                // 들어온 다음에 로딩 완료 메세지를 보낸다.
                SendMessage(json);
                break;
            case NetworkProtocol.NewObject:
                if (!isPlayer(json))
                {
                    // 새로운 객체 생성
                    GameObject gameObject = Instantiate(Preset.objects.NewGameObject);
                    gameObjects.Add((int)json["no"], gameObject.GetComponent<Character>());
                }
                character = GetGameObject(json);
                character.No = (int)json["no"];
                character.Name = (string)json["name"];
                character.transform.position = new Vector3((int)json["x"], (int)json["y"], (int)json["z"]);
                break;
            case NetworkProtocol.RemoveObject:
                character = GetGameObject(json);
                gameObjects.Remove((int)json["no"]);
                DestroyImmediate(character.gameObject);
                break;
            case NetworkProtocol.Move:
                character = GetGameObject(json);
                character.Move(new Vector3((float)json["x"], (float)json["y"], (float)json["z"]));
                break;
            case NetworkProtocol.Chat:
                string color = "FFFFFF";
                switch ((int)json["chattype"])
                {
                    case ChatType.Notice:
                        color = "DBA901";
                        break;
                    case ChatType.Whisper:
                        color = "F781BE";
                        break;
                    case ChatType.System:
                        color = "82FA58";
                        break;
                }
                if (json["no"] != null)
                    Preset.objects.ChatBox.Add(string.Format("["+ color + "]{0} : {1}[-]", json["sender"], json["message"]));
                else
                    Preset.objects.ChatBox.Add(string.Format("[" + color + "]{0}[-]", json["message"]));
                GetGameObject(json).ChatMessage((string)json["message"]);
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
        thread.Abort();
        server.Dispose();

    }
}
