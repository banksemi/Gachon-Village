package gachon.cafe.gavigation;

import android.app.FragmentManager;
import android.app.FragmentTransaction;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.support.annotation.NonNull;
import android.support.design.widget.BottomNavigationView;
import android.support.v7.app.AppCompatActivity;
import android.view.MenuItem;
import android.widget.TextView;
import android.widget.Toast;

import org.json.JSONObject;

import java.util.List;

public class MainActivity extends AppCompatActivity {

    private TextView mTextMessage;

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
                case R.id.navigation_study_group:
                    SwitchView(Fragment_Notifications.class);
                    return true;
                case R.id.navigation_setting:
                    return true;
            }
            return false;
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        handler = this.mHandler;
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        mTextMessage = (TextView) findViewById(R.id.message);
        BottomNavigationView navigation = (BottomNavigationView) findViewById(R.id.navigation);
        navigation.setOnNavigationItemSelectedListener(mOnNavigationItemSelectedListener);


        JSONObject json = new JSONObject();
        try {
            json.put("type", 1000);
            json.put("message", "abab");
        }
        catch (Exception e)
        {

        }
        NetworkMain.Send(json);
        Intent intent = new Intent(
                getApplicationContext(),//현재제어권자
                NetworkService.class); // 이동할 컴포넌트
        startService(intent); // 서비스 시작


        FragmentManager fm = getFragmentManager();
        FragmentTransaction fragmentTransaction = fm.beginTransaction();
        fragmentTransaction.add(R.id.frameview, new Fragment_Login()); fragmentTransaction.commit();


    }
    private void ReceiveMessage(JSONObject json)
    {
        Toast.makeText(getApplicationContext(),json.toString(),Toast.LENGTH_SHORT).show();
    }
    private void SwitchView(Class fragment)
    {
        FragmentManager fm = getFragmentManager();
        FragmentTransaction fragmentTransaction = fm.beginTransaction();
        try {
            fragmentTransaction.replace(R.id.frameview,(android.app.Fragment)fragment.newInstance());
        }
        catch (Exception e)
        {

        }
        fragmentTransaction.commit();
    }
    // 이벤트 처리를 위한 핸들러. 패킷 수신이 ReceiveMessage를 호출
    public static Handler handler = null;
    Handler mHandler = new Handler() {
        public void handleMessage(Message msg) {
            List<JSONObject> list = NetworkMain.ReceiveQueue.Get();
            if (list != null) {
                for (JSONObject item : list) {

                    ReceiveMessage(item);
                }
            }
            // 메세지를 처리하고 또다시 핸들러에 메세지 전달 (1000ms 지연)
            //mHandler.sendEmptyMessageDelayed(0,1000);
        }
    };
}
