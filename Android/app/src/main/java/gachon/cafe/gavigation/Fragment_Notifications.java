package gachon.cafe.gavigation;

import android.app.Fragment;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import org.json.JSONObject;

public class Fragment_Notifications extends Fragment implements ReceiveFragment {

    public Fragment_Notifications() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        return inflater.inflate(R.layout.setting_menu, container, false);
    }
    @Override
    public void ReceiveMessage(JSONObject json) {

    }
}

