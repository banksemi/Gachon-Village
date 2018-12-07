package gachon.cafe.gavigation;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import org.json.JSONObject;
public class MessageViewActivity extends ESocketActivity{
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.post_item_content);
        Intent intent=new Intent(this.getIntent());
        int no = -1;
        try {
            no = intent.getIntExtra("no",-1);
        }
        catch (Exception e)
        {

        }

        Object[] data = DBHelper.GetMain(this).GetData(no);
        // 하나씩 대입
        TextView post_content_title = (TextView) findViewById(R.id.post_content_title);
        TextView post_content_sender = (TextView) findViewById(R.id.post_content_sender);
        TextView post_content_date = (TextView) findViewById(R.id.post_content_date);
        TextView post_content_content = (TextView) findViewById(R.id.post_content_content);

        post_content_title.setText(data[1].toString());
        post_content_content.setText(data[2].toString());
        post_content_sender.setText(data[3].toString());
        post_content_date.setText(data[5].toString());

        /*
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
        */

    }
    @Override
    public void ReceiveMessage(JSONObject json)
    {

    }
}