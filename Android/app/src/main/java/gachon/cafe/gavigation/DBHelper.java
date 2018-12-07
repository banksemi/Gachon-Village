package gachon.cafe.gavigation;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.util.Log;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

public class DBHelper extends SQLiteOpenHelper {
    private static HashMap<Context,DBHelper> Mains = new HashMap<Context, DBHelper>();
    public static DBHelper GetMain(Context context)
    {
        Log.d("SQL", "디비 목록 추가 시작");
        if (!Mains.containsKey(context)) {
            Log.d("SQL", "디비 목록 추가");
            DBHelper main = new DBHelper(context, "gavi", null, 10);
            Log.d("SQL", "디비 목록 추가2");
            main.testDB();
            Log.d("SQL", "디비 목록 추가3");
            Mains.put(context,main);
        }
        Log.d("SQL", "디비 목록 끝");
        return Mains.get(context);
    }
    private Context context;
    public DBHelper(Context context, String name, SQLiteDatabase.CursorFactory factory, int version)
    {
        super(context, name, factory, version);
        this.context = context;
        Log.d("SQL", "SQL 추ㄴ가 ");
}
    /** * Database가 존재하지 않을 때, 딱 한번 실행된다. * DB를 만드는 역할을 한다. * @param db */
    @Override
    public void onCreate(SQLiteDatabase db) {
        Log.d("SQL", "SQL 생성중 ");
        // String 보다 StringBuffer가 Query 만들기 편하다.
        StringBuffer sb = new StringBuffer();
        sb.append(" CREATE TABLE POST ( ");
        sb.append(" NO INTEGER PRIMARY KEY, ");
        sb.append(" TITLE TEXT, ");
        sb.append(" CONTENT TEXT, ");
        sb.append(" SENDER TEXT , ");
        sb.append(" SENDER_ID TEXT , ");
        sb.append(" SDATE DATETIME ) "); // SQLite Database로 쿼리 실행
        db.execSQL(sb.toString());
        Log.d("SQL", "SQL 생성 끝 ");
        Toast.makeText(context, "Table 생성완료", Toast.LENGTH_SHORT).show();
    }
    /** * Application의 버전이 올라가서 * Table 구조가 변경되었을 때 실행된다. * @param db * @param oldVersion * @param newVersion */
    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
    {
        db.execSQL("DROP TABLE IF EXISTS POST");
        onCreate(db);
        Toast.makeText(context, "내부 데이터베이스의 버전이 변경되어 그동안의 모든 데이터가 삭제되었습니다.", Toast.LENGTH_SHORT).show();
    } /** * */

    public void AddData(int no, String title, String content, String sender,String id, Date date) {
        // 1. 쓸 수 있는 DB 객체를 가져온다.
        Log.d("SQL", "SQL 추가1 ");
         SQLiteDatabase db = getWritableDatabase(); // 2. Person Data를 Insert한다.
        // _id는 자동으로 증가하기 때문에 넣지 않습니다.
        try {
            StringBuffer sb = new StringBuffer();
            sb.append(" INSERT INTO POST ( ");
            sb.append(" NO, TITLE, CONTENT, SENDER, SENDER_ID, SDATE ) ");
            sb.append(" VALUES ( ?, ?, ?, ?, ?, ?) "); // sb.append(" VALUES ( #NAME#, #AGE#, #PHONE# ) "); // // // Age는 Integer이기 때문에 홀따옴표(')를 주지 않는다. // String query = sb.toString(); // query.replace("#NAME#", "'" + person.getName() + "'"); // query.replace("#NAME#", person.getAge()); // query.replace("#NAME#", "'" + person.getPhone() + "'"); // // db.execSQL(query); db.execSQL(sb.toString(), new Object[]{ person.getName(), Integer.parseInt(person.getAge()), person.getPhone()});; Toast.makeText(context, "Insert 완료", Toast.LENGTH_SHORT).show(); }
            db.execSQL(sb.toString(), new Object[]{no, title, content, sender, id, date});

        }
        catch (Exception e)
        {

        }
        Log.d("SQL", "SQL 추가 ");
    }
    public List getAllData() {
        Log.d("SQL", "SQL 로드  시작");
        StringBuffer sb = new StringBuffer();
        sb.append(" SELECT NO, TITLE, CONTENT, SENDER, SENDER_ID, SDATE FROM POST");
        // 읽기 전용 DB 객체를 만든다.
        SQLiteDatabase db = getReadableDatabase();
        Cursor cursor = db.rawQuery(sb.toString(), null);
        Log.d("SQL", "SQL 로드 ");
        List datalist = new ArrayList();
        while( cursor.moveToNext() )
        {
            Object temp = new Object[]{cursor.getInt(0),cursor.getString(1),cursor.getString(2),cursor.getString(3),cursor.getString(4),cursor.getString(5)};
            datalist.add(temp);
        } return datalist;
    }
    public Object[] GetData(int no)
    {

        SQLiteDatabase db = getReadableDatabase();
        Cursor cursor = db.rawQuery("SELECT NO, TITLE, CONTENT, SENDER, SENDER_ID, SDATE FROM POST WHERE NO=?",new String[]{""+no});
        Object[] temp = null;
        while( cursor.moveToNext() )
        {
            temp = new Object[]{cursor.getInt(0),cursor.getString(1),cursor.getString(2),cursor.getString(3),cursor.getString(4),cursor.getString(5)};
        } return temp;
    }
    public int getLastNo()
    {
        SQLiteDatabase db = getReadableDatabase();
        Cursor cursor = db.rawQuery("SELECT MAX(NO) FROM POST", null);
        Log.d("SQL", "SQL 로드 ");
        List people = new ArrayList();
        while( cursor.moveToNext() )
        {
            return cursor.getInt(0);
        }
        return 0;

    }
    public void testDB() {
        Log.d("SQL", "SQL 테스트1 ");
        SQLiteDatabase db = getReadableDatabase();
        Log.d("SQL", "SQL 테스트2 ");
    }
}


