package gachon.cafe.gavigation;

import org.json.JSONObject;

public class NetworkMain {
    public static MessageQueue SendQueue = new MessageQueue();
    public static MessageQueue ReceiveQueue = new MessageQueue();
    public static void Send(JSONObject json) {
        SendQueue.Add(json);
    }
    public static void SendTypeMessage(int type)
    {
        try
        {
            JSONObject json = new JSONObject();
            json.put("type",type);
            Send(json);
        }
        catch (Exception e)
        {

        }
    }
}
