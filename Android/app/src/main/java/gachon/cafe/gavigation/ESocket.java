package gachon.cafe.gavigation;

import android.os.Debug;
import android.util.Log;

import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.Socket;
import java.util.List;

public class ESocket extends Thread {
    public static int no_count = 0;
    public int no;
    public ESocket(NetworkService service)
    {
        this.no = no_count++;
        this.service = service;
    }
    public void Log(String Message)
    {
        Log.d("테스트1 (Socket " + no + ")",Message);
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
                Log("서버에 연결 시도");
                socket = new Socket("easyrobot.co.kr", 1119);
                inFromClient = new BufferedReader(new InputStreamReader(socket.getInputStream()));
                outToClient = new PrintWriter(socket.getOutputStream(), true);
                FileFunction.LoginLoad();
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
        Log("소켓종료");
    }

    public void Dispose()
    {
        isdispose = true;
        SocketClose();
        this.interrupt();
        Log("해당 연결은 완전히 종료됨");
    }
}
