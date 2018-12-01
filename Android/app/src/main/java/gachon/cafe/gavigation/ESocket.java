package gachon.cafe.gavigation;

import android.os.Debug;
import android.util.Log;

import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import java.util.List;

public class ESocket extends Thread {
    public ESocket(NetworkService service)
    {
        this.service = service;
    }
    NetworkService service;
    boolean isdispose = false;
    Socket socket = null;
    BufferedReader inFromClient = null;
    PrintWriter outToClient = null;
    Thread ReceiveThread = null;
    public void Send(JSONObject json)
    {
        if (outToClient != null) {
            outToClient.write(json + "\r\n");
            outToClient.flush();
        }
    }
    public void run()
    {
        while(!isdispose) {
            try {
                Log.d("테스트1","시작 준비");
                socket = new Socket("easyrobot.co.kr", 1119);
                Log.d("테스트1","연결함");
                inFromClient = new BufferedReader(new InputStreamReader(socket.getInputStream()));
                outToClient = new PrintWriter(socket.getOutputStream(), true);

                ReceiveThread = new Thread()
                {
                    public void run()
                    {
                        while(socket != null)
                        {
                            try
                            {
                                String data = inFromClient.readLine();
                                if (data == null)
                                {
                                    SocketClose();
                                    return;
                                }
                                JSONObject json = new JSONObject(data);
                                NetworkMain.ReceiveQueue.Add(json);
                                Log.d("테스트1","핸들러 작업");
                                service.mHandler.sendEmptyMessage(0);
                                Log.d("테스트1","핸들러 완료");
                                Thread.sleep(10);
                            }
                            catch (Exception e)
                            {
                                Log.d("승화","리시브쓰레드 에러 " + e.getMessage());
                            }
                        }
                    }
                };
                ReceiveThread.start();
                while (socket != null ) {

                    JSONObject json = new JSONObject();
                    try {
                        json.put("type", 1000);
                        json.put("message", "ping");
                    }
                    catch (Exception e)
                    {

                    }
                    Log.d("A", "테스트");
                    NetworkMain.Send(json);

                    List<JSONObject> list = NetworkMain.SendQueue.Get();
                    if (list != null) {
                        for (JSONObject item : list) {
                            Send(item);
                        }
                    }
                    Thread.sleep(1000);
                }
            } catch (InterruptedException e) {
                Dispose();
            }
            catch (Exception e) {
                Log.d("승화","에러" + e.getMessage());
                e.printStackTrace();
                Log.d("A", "망했따.." + e.toString());
            }
            finally {
                SocketClose();
            }
        }
    }
    private void SocketClose() {
        if (socket != null) {
            try {
                socket.close();
            } catch (Exception e) {
            }
            socket = null;
        }
        if (ReceiveThread != null) {
            try {
                ReceiveThread.interrupt();
            } catch (Exception e) {
            }
            ReceiveThread = null;
        }
        Log.d("테스트1", "ReceiveThread 종료됨");
        Log.d("테스트1", "outToClient 종료됨");

        Log.d("테스트1", "socket 종료됨");
    }

    public void Dispose()
    {
        Log.d("테스트1", "소켓 닫기 시작");
        isdispose = true;
        SocketClose();
        Log.d("테스트1", "소켓 닫기 시작중");
        this.interrupt();
        Log.d("테스트1", "소켓 닫기 시작끝");
    }
}
