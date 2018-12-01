
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

import java.util.List;

public class ListFragment extends Fragment {
    private View myView = null;
    protected void UpdateList(JSONArray json, int ViewId)
    {
        myView = getView();
        JSONArray gl = null;
        try {
            gl = json;
        }
        catch (Exception e)
        {

        }
        String[]  LIST_STUDY = new String[gl.length()];
        try {
            for (int i = 0; i < LIST_STUDY.length; i++) {
                LIST_STUDY[i] = gl.getString(i);
            }
        }
        catch (Exception e) {

        }
        ArrayAdapter adapter = new ArrayAdapter(getActivity(), android.R.layout.simple_list_item_1, LIST_STUDY);
        ListView listView = (ListView) myView.findViewById(ViewId);
        listView.setAdapter(adapter);
    }
}

