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

import org.json.JSONArray;
import org.json.JSONObject;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;

public class Fragment_Notifications extends Fragment implements ReceiveFragment {
    private View view = null;
    private ListViewAdapter adapter;
    public void AddItem(int no, String title, String sender, String date)
    {
        ListView listview = view.findViewById(R.id.post_listView);
        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.contact), no, title, sender, date);
        listview.setAdapter(adapter);
    }
    public Fragment_Notifications() {
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

        Log.d("SQL", "로딩 완료");
        DBHelper helper = DBHelper.GetMain(getActivity());

        Log.d("SQL", "로딩 완료2");
        List<Object[]> data = helper.getAllData();

        Log.d("SQL", "로딩 완료");
        this.view = view;
        for(Object[] object : data)
        {
            AddItem((int)object[0],(String)object[1],(String)object[3],(String)object[5]);
        }
        return view;
    }
    @Override
    public void ReceiveMessage(JSONObject json) {
        try {
            int type = json.getInt("type");
            switch (type)
            {
                case 1220: // 그룹 정보

                    JSONArray array = json.getJSONArray("items");
                    for(int i = 0 ; i < array.length();i++) // 새로 들어온 자료에 대해서 리스트 추가
                    {
                        JSONObject item = (JSONObject)array.get(i);
                        int no = item.getInt("no");
                        String title = item.getString("title");
                        String sender = item.getString("sender");
                        Date date = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").parse(item.getString("date"));
                        AddItem(no, title, sender, date.toString());
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

