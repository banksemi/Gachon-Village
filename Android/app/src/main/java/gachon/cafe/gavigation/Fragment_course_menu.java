package gachon.cafe.gavigation;

import android.app.Fragment;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.Toast;

public class Fragment_course_menu extends Fragment {

    static final String[] LIST_COURSE ={"이슬람 문화의 의해","컴퓨터 네트워크와 실습","알고리즘","컴퓨터 네트워크와 실습","알고리즘","컴퓨터 네트워크와 실습","알고리즘"};
    static final String[] LIST_STUDY = {"우편함", "뤠디오가가","우편함", "뤠디오가가","우편함", "뤠디오가가","우편함", "뤠디오가가"};
    public Fragment_course_menu() {


        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        View view = inflater.inflate(R.layout.course_menu,null);
        ArrayAdapter adapter = new ArrayAdapter(getActivity(), android.R.layout.simple_list_item_1, LIST_COURSE);

        ArrayAdapter adapter2 = new ArrayAdapter(getActivity(), android.R.layout.simple_list_item_1, LIST_STUDY);
        ListView listView2 = (ListView) view.findViewById(R.id.listView_study);

        ListView listView = (ListView) view.findViewById(R.id.listView_course);
        listView.setAdapter(adapter);listView2.setAdapter(adapter2);

        return view;
    }
}

