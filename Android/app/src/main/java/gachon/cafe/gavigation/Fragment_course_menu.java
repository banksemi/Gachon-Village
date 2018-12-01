package gachon.cafe.gavigation;

import android.app.Fragment;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.Toast;

import org.json.JSONArray;
import org.json.JSONObject;

public class Fragment_course_menu extends ListFragment implements ReceiveFragment {
    private View myView = null;
    public Fragment_course_menu() {
        try
        {
            JSONObject json = new JSONObject();
            json.put("type",1119);
            NetworkMain.Send(json);
        }
        catch (Exception e)
        {

        }
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        myView = inflater.inflate(R.layout.course_menu,null);
        return myView;
    }
    @Override
    public void ReceiveMessage(JSONObject json) {
        try {
            int type = json.getInt("type");
            switch (type)
            {
                case 1119: // 그룹 정보
                    UpdateList(json.getJSONArray("group"),R.id.listView_study);
                    UpdateList(json.getJSONArray("class"),R.id.listView_course);
                    break;
            }
        }
        catch (Exception e)
        {
            Log.d("테스트3", e.getMessage());
        }
    }
}

