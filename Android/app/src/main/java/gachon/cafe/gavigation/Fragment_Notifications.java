package gachon.cafe.gavigation;

import android.app.Fragment;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.support.v4.content.ContextCompat;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ListView;

import org.json.JSONObject;

import java.text.SimpleDateFormat;
import java.util.Date;

public class Fragment_Notifications extends Fragment implements ReceiveFragment {
    private ListViewAdapter adapter;
    public void AddItem(String title, String content, String sender, String date)
    {
        ListView listview = getView().findViewById(R.id.post_listView);
        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.contact),title,content, sender, date);
        listview.setAdapter(adapter);
    }
    public void AddItem(JSONObject json)
    {
        try
        {
            Log.d("테스트3", json.toString());
            AddItem(json.getString("title"), json.getString("content"), json.getString("sender"), json.getString("date"));
        }
        catch (Exception e)
        {
            Log.d("테스트3", e.getMessage());
        }
    }
    public Fragment_Notifications() {
        NetworkMain.SendTypeMessage(1220);
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        ListView listview;
        View view = inflater.inflate(R.layout.post_menu, null) ;
        listview = view.findViewById(R.id.post_listView);

        adapter = new ListViewAdapter();
        listview.setAdapter(adapter);

        listview.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                ListViewItem item = (ListViewItem) parent.getItemAtPosition(position);

                String titleStr =  item.getTitle();
                Drawable iconDrawable = item.getIcon();
            }
        });
        return view;
    }
    @Override
    public void ReceiveMessage(JSONObject json) {
        try {
            int type = json.getInt("type");
            switch (type)
            {
                case 11: // 그룹 정보
                    for(int i = 0 ; i < json.getJSONArray("items").length();i++)
                    {
                        AddItem(json.getJSONArray("items").getJSONObject(i));
                    }
                    break;
            }
        }
        catch (Exception e)
        {
            Log.d("테스트3", e.getMessage());
        }
    }
}

