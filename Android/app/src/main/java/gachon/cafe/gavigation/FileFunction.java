package gachon.cafe.gavigation;

import android.util.Log;

import org.json.JSONObject;

import java.io.File;
import java.io.PrintWriter;
import java.util.Scanner;

public class FileFunction {
    public static void LoginSave(String data) {
        Save("iddata.txt",data);
    }

    public static void LoginLoad() {
        try {
            String[] data2 = Load("iddata.txt").split(":");
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
    public static void LoginRemove()
    {
        Remove("iddata.txt");
    }
    private static void Save(String file, String data)
    {
        try {
            PrintWriter pw = new PrintWriter(ESocketActivity.path.getAbsolutePath() + file);
            pw.write(data + "\r\n");
            pw.flush();
            pw.close();
        } catch (Exception e) {
            Log.d("테스트2", "에러" + e.getMessage());
            e.printStackTrace();
        }
    }
    public static String Load(String file2) {
        try {
            File file = new File(ESocketActivity.path.getAbsolutePath() + file2);
            Scanner sc = new Scanner(file);
            String data = sc.nextLine();
            sc.close();
            return data;
        }
        catch (Exception e)
        {
            Log.d("테스트2", "2에러" + e.getMessage());
            e.printStackTrace();

        }
        return null;
    }
    public static void Remove(String file2)
    {
        try {
            File file = new File(ESocketActivity.path.getAbsolutePath() + file2);
            file.delete();
        }
        catch (Exception e)
        {
            Log.d("테스트2", "2에러" + e.getMessage());
            e.printStackTrace();

        }
    }
    public static int NextNo()
    {
       String a = Load("a.txt");
       if (a == null)
       {
           Save("a.txt","0");
           return 0;
       }
       else
       {
          int no = Integer.parseInt(a) + 1;
          Save("a.txt",no + "");
          return no;
       }
    }
}

