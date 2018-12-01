package gachon.cafe.gavigation;

import org.json.JSONObject;

import java.util.LinkedList;
import java.util.List;

public class MessageQueue {
    private List<JSONObject> list = new LinkedList<>();
    public synchronized void Add(JSONObject json)
    {
        list.add(json);
    }
    public synchronized List<JSONObject> Get()
    {
        if (list.size() > 0)
        {
            List<JSONObject> result = new LinkedList<>();
            while(list.size() != 0) {
                result.add(list.get(0));
                list.remove(0);
            }
            return result;
        }
        else
        {
            return null;
        }
    }
}
