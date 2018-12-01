package gachon.cafe.gavigation;

import android.app.Fragment;
import android.content.Context;
import android.os.Bundle;
import android.support.v4.content.ContextCompat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ListView;

import org.json.JSONObject;

public class Fragment_Notifications extends Fragment implements ReceiveFragment {

    public Fragment_Notifications() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        ListView listview;
        ListViewAdapter adapter;

        View view = inflater.inflate(R.layout.post_menu, null) ;
        adapter = new ListViewAdapter();

        listview = (ListView) getView().findViewById(R.id.post_listView);
        listview.setAdapter(adapter);

        adapter.addItem(ContextCompat.getDrawable(this, R.drawable.ic_launcher_background));
        adapter.addItem(ContextCompat.getDrawable(this, R.drawable.ic_launcher_background));
        adapter.addItem(ContextCompat.getDrawable(this, R.drawable.ic_launcher_background));

        return view;
    }
    @Override
    public void ReceiveMessage(JSONObject json) {

    }
}

