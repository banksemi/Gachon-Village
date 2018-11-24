package gachon.cafe.gavigation;

import org.json.JSONObject;

public class NetworkMain {
    public static MessageQueue SendQueue = new MessageQueue();
    public static MessageQueue ReceiveQueue = new MessageQueue();
    public static void Send(JSONObject json) {
        SendQueue.Add(json);
    }
}
