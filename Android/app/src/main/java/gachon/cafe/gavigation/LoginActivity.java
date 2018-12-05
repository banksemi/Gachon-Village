package gachon.cafe.gavigation;

import android.app.FragmentManager;
import android.app.FragmentTransaction;
import android.content.Intent;
import android.os.Bundle;
import android.support.design.widget.BottomNavigationView;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import org.json.JSONObject;

public class LoginActivity extends ESocketActivity{
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.login);
        Button login_button = (Button) findViewById(R.id.login_button);
        login_button.setOnClickListener(new Button.OnClickListener() {
            @Override
            public void onClick(View view) {
                JSONObject json = new JSONObject();
                TextView login_text = (TextView) findViewById(R.id.login_id_edittext);
                TextView login_text2 = (TextView) findViewById(R.id.login_password_edittext);
                try
                {
                    json.put("type",1115);
                    json.put("id",login_text.getText());
                    json.put("password",login_text2.getText());
                }
                catch (Exception e)
                {

                }
                NetworkMain.Send(json);
            }
        });
    }
    @Override
    public void ReceiveMessage(JSONObject json)
    {
        try {
            int type = json.getInt("type");
            switch (type)
            {
                case 1115: // 로그인
                    // 로그인 성공시 액티비티 이동
                    Intent intent=new Intent(LoginActivity.this,MainActivity.class);
                    startActivity(intent);
                    break;

            }
        }
        catch (Exception e)
        {
            Log.d("테스트3", e.getMessage());
        }
    }
}


