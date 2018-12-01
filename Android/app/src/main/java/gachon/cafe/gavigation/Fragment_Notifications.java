package gachon.cafe.gavigation;

import android.app.Fragment;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.support.v4.content.ContextCompat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
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

        listview = (ListView) view.findViewById(R.id.post_listView);
        listview.setAdapter(adapter);

        listview.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                ListViewItem item = (ListViewItem) parent.getItemAtPosition(position);

                String titleStr =  item.getTitle();
                String descStr = item.getDesc();
                Drawable iconDrawable = item.getIcon();
            }
        });


        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.contact),"Title","description","최은아","2018-12-02");
        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.contact),"Title","description","최은아","2018-12-02");
        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.contact),"Title","description","최은아","2018-12-02");
        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.contact),"Title","description","최은아","2018-12-02");
        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.contact),"Title","description","최은아","2018-12-02");
        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.contact),"Title","description","최은아","2018-12-02");
        return view;
    }
    @Override
    public void ReceiveMessage(JSONObject json) {

    }
}

