package gachon.cafe.gavigation;

import android.app.FragmentManager;
import android.app.FragmentTransaction;
import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.design.widget.BottomNavigationView;
import android.support.v4.app.Fragment;
import android.support.v7.app.AppCompatActivity;
import android.view.MenuItem;
import android.widget.TextView;

import org.json.JSONObject;

public class MainActivity extends ESocketActivity {

    private TextView mTextMessage;
    private ReceiveFragment receiverFragment = null;

    private BottomNavigationView.OnNavigationItemSelectedListener mOnNavigationItemSelectedListener
            = new BottomNavigationView.OnNavigationItemSelectedListener() {

        @Override
        public boolean onNavigationItemSelected(@NonNull MenuItem item) {
            switch (item.getItemId()) {
                case R.id.navigation_home:
                    SwitchView(Fragment_Notifications.class);
                    return true;
                case R.id.navigation_notifications:
                    SwitchView(Fragment_Notifications.class);
                    return true;
                case R.id.navigation_course:
                    SwitchView(Fragment_course_menu.class);
                    return true;
                case R.id.navigation_setting:
                    SwitchView(Fragment_Keyword.class);
                    return true;
            }
            return false;
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        mTextMessage = (TextView) findViewById(R.id.message);
        BottomNavigationView navigation = (BottomNavigationView) findViewById(R.id.navigation);
        navigation.setOnNavigationItemSelectedListener(mOnNavigationItemSelectedListener);

        SwitchView(Fragment_Notifications.class);

    }
    private void SwitchView(Class fragment)
    {
        FragmentManager fm = getFragmentManager();
        FragmentTransaction fragmentTransaction = fm.beginTransaction();
        android.app.Fragment newf = null;
        try {
            newf = (android.app.Fragment)fragment.newInstance();
            fragmentTransaction.replace(R.id.frameview,newf);
        }
        catch (Exception e)
        {

        }
        receiverFragment = (ReceiveFragment) newf;
        fragmentTransaction.commit();
    }

    @Override
    public void ReceiveMessage(JSONObject json) {
        if (receiverFragment != null) receiverFragment.ReceiveMessage(json);
    }
}
