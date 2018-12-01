package gachon.cafe.gavigation;

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;

import org.json.JSONObject;

import java.io.File;

public class ESocketActivity extends AppCompatActivity
{
    public static File path = new File("/data/data/gachon.cafe.gavigation/files/");
    public void ReceiveMessage(JSONObject json)
    {

    }
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        path = getFilesDir();
        // 서비스 등록
        super.onCreate(savedInstanceState);
        Intent intent = new Intent(
                getApplicationContext(),//현재제어권자
                NetworkService.class); // 이동할 컴포넌트
        startService(intent); // 서비스 시작
        NetworkService.receivers.add(this);
    }
    @Override
    protected void onDestroy() {
        super.onDestroy();
        NetworkService.receivers.remove(this);
    }
}
