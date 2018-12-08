package gachon.cafe.gavigation;
import android.app.Fragment;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

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
