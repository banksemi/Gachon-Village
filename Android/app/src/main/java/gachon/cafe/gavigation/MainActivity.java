package gachon.cafe.gavigation;

import android.app.FragmentManager;
import android.app.FragmentTransaction;
import android.os.Bundle;
import android.os.Handler;
import android.support.annotation.NonNull;
import android.support.design.widget.BottomNavigationView;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.TextView;
import android.widget.Toast;

import org.json.JSONObject;

import java.io.File;

public class MainActivity extends ESocketActivity {

    private TextView mTextMessage;
    private ReceiveFragment receiverFragment = null;
    boolean doubleBackToExitPressedOnce = false;
    private BottomNavigationView.OnNavigationItemSelectedListener mOnNavigationItemSelectedListener
            = new BottomNavigationView.OnNavigationItemSelectedListener() {

        @Override
        public boolean onNavigationItemSelected(@NonNull MenuItem item) {
            switch (item.getItemId()) {
                case R.id.navigation_home:
                    SwitchView(Fragment_Home.class);
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

        SwitchView(Fragment_Home.class);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // 메뉴 추가
        getMenuInflater().inflate(R.menu.actionbar_actions,menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.action_logout:
                Logout();
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }
    private void Logout()
    {
        //FileFunction.LoginRemove();
        File cache = getCacheDir();
        File appDir = new File(cache.getParent());
        if (appDir.exists())
        {
            String[] children = appDir.list();
            for(String s : children)
            {
                if (!s.equals("lib") && !s.equals("files"))
                {
                    Log.d("파일",new File(appDir,s).getAbsolutePath());
                    deleteFile_Seunghwa(new File(appDir,s).getAbsolutePath());
                }
            }
        }
        ResetNetwork(); // 연결 소켓을 초기화
        DBHelper.ResetMain(); // 열린 데이터베이스를 종료해줘야 다음번에 새로 가져옴
        finishAffinity();
    }
    public static void deleteFile_Seunghwa(String path) {
        File deleteFolder = new File(path);

        if(deleteFolder.exists()){
            if (deleteFolder.isDirectory()) {
                File[] deleteFolderList = deleteFolder.listFiles();
                for (int i = 0; i < deleteFolderList.length; i++) {
                    if (deleteFolderList[i].isFile()) {
                        deleteFolderList[i].delete();
                    } else {
                        deleteFile_Seunghwa(deleteFolderList[i].getPath());
                    }
                    deleteFolderList[i].delete();
                }
            }
            deleteFolder.delete();
        }
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
    public void onBackPressed() {
        //  뒤로가기 키를 막기 위한 오버라이드
        if (doubleBackToExitPressedOnce) {
            finishAffinity();
            return;
        }

        this.doubleBackToExitPressedOnce = true;
        Toast.makeText(this, "프로그램을 종료하려면 뒤로가기 버튼을 한번 더 눌러주세요.", Toast.LENGTH_SHORT).show();

        // 2초 뒤에 다시 원래대로, 2초만에 나가기 눌리면 어차피 프로그램이 종료되서 상관없음 (아래가 실행되어도)
        new Handler().postDelayed(new Runnable() {
            @Override
            public void run() {
                doubleBackToExitPressedOnce=false;
            }
        }, 2000);
    }

    @Override
    public void ReceiveMessage(JSONObject json) {
        if (receiverFragment != null) receiverFragment.ReceiveMessage(json);
    }
}
