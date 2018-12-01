package gachon.cafe.gavigation;

import android.app.Fragment;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import org.json.JSONArray;
import org.json.JSONObject;

public class Fragment_Keyword extends ListFragment implements ReceiveFragment {
    private View myView = null;
    public Fragment_Keyword() {
        NetworkMain.SendTypeMessage(1202);
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        myView = inflater.inflate(R.layout.setting_menu,null);



        Button login_button = (Button) myView.findViewById(R.id.keyword_button);
        login_button.setOnClickListener(new Button.OnClickListener() {
            @Override
            public void onClick(View view) {
                JSONObject json = new JSONObject();
                TextView text = (TextView) myView.findViewById(R.id.setKeyword_editText);
                try
                {
                    json.put("type",1225);
                    json.put("keyword",text.getText());
                }
                catch (Exception e)
                {

                }
                text.setText("");
                NetworkMain.Send(json);
            }
        });

        return myView;
    }
    @Override
    public void ReceiveMessage(JSONObject json) {
        try {
            int type = json.getInt("type");
            switch (type)
            {
                case 1202: // 그룹 정보
                    UpdateList(json.getJSONArray("list"),R.id.keyword_listView);
                    break;
            }
        }
        catch (Exception e)
        {
            Log.d("테스트3", e.getMessage());
        }
    }
}

