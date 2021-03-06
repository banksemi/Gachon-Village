package gachon.cafe.gavigation;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.content.ReceiverCallNotAllowedException;
import android.graphics.Color;
import android.os.Build;
import android.os.Handler;
import android.os.IBinder;
import android.os.Message;
import android.support.v4.app.NotificationCompat;
import android.util.Log;
import android.widget.Toast;

import org.json.JSONArray;
import org.json.JSONObject;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Locale;

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
        String Classname = null;
        boolean Reset = false;
        try {
            Classname = intent.getExtras().getString("Class");
        }
        catch (Exception e)
        {

        }
        if (Classname == null || Classname.equals("LoginActivity"))
        {
            if (esocket != null)
            {
                esocket.Dispose();
            }
            esocket = new ESocket(this);
            esocket.start();
        }
        else
        {
            if (esocket == null)
            {
                esocket = new ESocket(this);
                esocket.start();
            }
        }
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
    public void Test(String title, String content, String Sender)
    {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            NotificationManager notificationManager = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);
            NotificationChannel notificationChannel = new NotificationChannel("post_channel", "우편 알림", NotificationManager.IMPORTANCE_DEFAULT);
            notificationChannel.setDescription("우편함에 새로운 소식이 올때 이 채널을 통해 알려드립니다.");
            notificationChannel.enableLights(true);
            notificationChannel.setLightColor(Color.GREEN);
            notificationChannel.enableVibration(true); notificationChannel.setVibrationPattern(new long[]{100, 200, 100, 200});
            notificationChannel.setLockscreenVisibility(Notification.VISIBILITY_PRIVATE);
            notificationManager.createNotificationChannel(notificationChannel);
        }





        NotificationCompat.Builder builder = new NotificationCompat.Builder(getApplicationContext(),"post_channel")
                .setSmallIcon(R.drawable.login_logo)
                .setContentTitle(Sender)
                .setContentText(title);
        NotificationCompat.BigTextStyle style = new NotificationCompat.BigTextStyle(builder)
                .bigText(title + "\r\n\r\n" + content);


        builder.setStyle(style);

        NotificationManager manager = (NotificationManager)getSystemService(NOTIFICATION_SERVICE);
        manager.notify(FileFunction.NextNo(),builder.build());

    }
    private void GetPost_Last()
    {
        try
        {
            JSONObject json2 = new JSONObject();
            json2.put("type",1220);
            json2.put("no",DBHelper.GetMain(this).getLastNo());
            NetworkMain.Send(json2);
        }
        catch (Exception e)
        {

        }
    }
    private void ReceiveMessage(JSONObject json)
    {
        try {
            int type = json.getInt("type");
            switch (type)
            {
                case 1:
                    Toast.makeText(getApplicationContext(),json.getString("message"),Toast.LENGTH_SHORT).show();
                    break;
                case 9: // 우편함 내용 실시간 알림
                    // 새로운 우편이 도착함. 혹시 그동안 수신 못한 데이터가 있는지 요청
                    GetPost_Last();
                    //Test(json.getString("title"),json.getString("content"),json.getString("sender"));
                    //Toast.makeText(getApplicationContext(),json.getString("title"),Toast.LENGTH_SHORT).show();
                    break;
                case 1115: // 로그인
                    // 로그인에 성공했습니다!!
                    FileFunction.LoginSave(json.getString("data"));
                    Toast.makeText(getApplicationContext(),"로그인에 성공했습니다!!",Toast.LENGTH_SHORT).show();
                    // 로그인했을때 최근 게시글이 있는지 요청
                    GetPost_Last();

                    break;
                case 1220: // 메세지 수신
                    JSONArray array = json.getJSONArray("items");
                    for(int i = 0 ; i < array.length();i++)
                    {
                        JSONObject item = (JSONObject)array.get(i);
                        int no = item.getInt("no");
                        String title = item.getString("title");
                        String content = item.getString("content");
                        String sender = item.getString("sender");
                        String sender_id = item.getString("sender_id");
                        Date date = new SimpleDateFormat("Z yyyy-MM-dd HH:mm:ss", Locale.KOREA).parse("+0900 "+item.getString("date"));
                        DBHelper.GetMain(this).AddData(no,title,content,sender,sender_id,date);
                        Test(title,content,sender);
                        //Toast.makeText(getApplicationContext(),title,Toast.LENGTH_SHORT).show();
                        //AddItem(json.getJSONArray("items").getJSONObject(i));
                    }
                    break;

            }
        }
        catch (Exception e)
        {e.printStackTrace();
        }
        for(ESocketActivity item : receivers)
        {
            item.ReceiveMessage(json);
        }
        //Toast.makeText(getApplicationContext(),json.toString(),Toast.LENGTH_SHORT).show();
    }
}
