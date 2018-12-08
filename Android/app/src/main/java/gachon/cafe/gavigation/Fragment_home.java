package gachon.cafe.gavigation;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import org.json.JSONObject;

public class Fragment_home extends ListFragment implements ReceiveFragment {
    private View myView = null;

    public Fragment_home() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        myView = inflater.inflate(R.layout.home_menu,null);

        return myView;
    }
    @Override
    public void ReceiveMessage(JSONObject json) {
        try {
            int type = json.getInt("type");
            switch (type)
            {

            }
        }
        catch (Exception e)
        {

        }
    }
}

