package gachon.cafe.gavigation;

import android.app.Service;
import android.content.Intent;
import android.content.ReceiverCallNotAllowedException;
import android.os.Handler;
import android.os.IBinder;
import android.os.Message;
import android.util.Log;
import android.widget.Toast;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

public class NetworkService  extends Service {
    public static List<ESocketActivity> receivers = new ArrayList();
    private static ESocket esocket = null;
    @Override
    public IBinder onBind(Intent intent) {
        // Service 객체와 (화면단 Activity 사이에서)
        // 통신(데이터를 주고받을) 때 사용하는 메서드
        // 데이터를 전달할 필요가 없으면 return null;
        return null;
    }
    @Override
    public void onCreate() {
        super.onCreate();
        // 서비스에서 가장 먼저 호출됨(최초에 한번만)
        Log.d("test", "서비스의 onCreate");
    }
    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        // 서비스가 호출될 때마다 실행
        Log.d("테스트1", "서비스의 onStartCommand");
        if (esocket != null)
        {
            esocket.Dispose();
        }
        Log.d("테스트1", "종료 완료");
        esocket = new ESocket(this);
        esocket.start();
        Log.d("테스트1", "서비스의 onStartCommand 끝");

        return super.onStartCommand(intent, flags, startId);
    }
    // 이벤트 처리를 위한 핸들러. 패킷 수신이 ReceiveMessage를 호출
    Handler mHandler = new Handler() {
        public void handleMessage(Message msg) {
            Log.d("테스트1","핸들러 작업중");
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
    private void ReceiveMessage(JSONObject json)
    {
        try {
            int type = json.getInt("type");
            switch (type)
            {
                case 1:
                    Toast.makeText(getApplicationContext(),json.getString("message"),Toast.LENGTH_SHORT).show();
                    break;
                case 1115: // 로그인
                    // 로그인에 성공했습니다!!
                    LoginFunction.LoginSave(json.getString("data"));
                    Toast.makeText(getApplicationContext(),"로그인에 성공했습니다!!",Toast.LENGTH_SHORT).show();
                    break;

            }
        }
        catch (Exception e)
        {

        }
        for(ESocketActivity item : receivers)
        {
            item.ReceiveMessage(json);
        }
        //Toast.makeText(getApplicationContext(),json.toString(),Toast.LENGTH_SHORT).show();
    }
}
