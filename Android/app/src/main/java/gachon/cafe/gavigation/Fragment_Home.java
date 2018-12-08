package gachon.cafe.gavigation;
import android.app.Fragment;
import android.content.Intent;
import android.net.Network;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import org.json.JSONArray;
import org.json.JSONObject;
public class Fragment_Home  extends Fragment implements ReceiveFragment{
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        NetworkMain.SendTypeMessage(1208);
        return inflater.inflate(R.layout.home_menu,null);
    }
    @Override
    public void ReceiveMessage(JSONObject json) {
        try {
            if (json.getInt("type") == 1208)
            {
                ((TextView)getView().findViewById(R.id.info_name)).setText(json.getString("name"));
                ((TextView)getView().findViewById(R.id.info_dept)).setText(json.getString("department"));
                ((TextView)getView().findViewById(R.id.info_stdid)).setText("(" + json.getString("number") + ")");
            }
        }
        catch (Exception e)
        {

        }
    }
}
