package gachon.cafe.gavigation;

import android.util.Log;

import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import java.util.List;

public class ESocket extends Thread {
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
                Log.d("승화","시작 준비");
                socket = new Socket("192.168.1.61", 1118);
                Log.d("승화","연결함");
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

                                MainActivity.handler.sendEmptyMessage(0);
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
                while (socket != null) {

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
                    Thread.sleep(50);
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
        if (ReceiveThread != null) {
            try {
                ReceiveThread.interrupt();
            } catch (Exception e) {
            }
            ReceiveThread = null;
        }
        if (inFromClient != null) {
            try {
                inFromClient.close();
            } catch (Exception e) {
            }
            inFromClient = null;
        }
        if (outToClient != null) {
            try {
                outToClient.close();
            } catch (Exception e) {
            }
            outToClient = null;
        }
        if (socket != null) {
            try {
                socket.close();
            } catch (Exception e) {
            }
            socket = null;
        }
    }

    public void Dispose()
    {
        isdispose = true;
        this.interrupt();
        Dispose();
    }
}
