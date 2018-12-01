package gachon.cafe.gavigation;

import android.util.Log;

import org.json.JSONObject;

import java.io.File;
import java.io.PrintWriter;
import java.util.Scanner;

public class LoginFunction {
    public static void LoginSave(String data) {
        try {
            Log.d("테스트2", "파일 여는중");
            File file = new File("Gachon-Village");
            if( !file.exists() ) {
                file.mkdirs();
            }
            PrintWriter pw = new PrintWriter(ESocketActivity.path.getAbsolutePath() + "iddata.txt");
            pw.write(data + "\r\n");
            pw.flush();
            pw.close();
        } catch (Exception e) {
            Log.d("테스트2", "에러" + e.getMessage());
            e.printStackTrace();
        }
    }

    public static void LoginLoad() {
        try {
            File file = new File(ESocketActivity.path.getAbsolutePath() + "iddata.txt");
            Scanner sc = new Scanner(file);
            String data = sc.nextLine();
            sc.close();
            String[] data2 = data.split(":");
            JSONObject json = new JSONObject();
            json.put("type", 1115);
            json.put("id", data2[0]);
            json.put("password", data2[1]);
            NetworkMain.Send(json);
        }
        catch (Exception e)
        {
            Log.d("테스트2", "2에러" + e.getMessage());
            e.printStackTrace();

        }
    }
}

