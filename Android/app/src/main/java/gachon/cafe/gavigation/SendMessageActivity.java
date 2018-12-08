package gachon.cafe.gavigation;

import android.content.Intent;
import android.net.Network;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import org.json.JSONObject;

public class SendMessageActivity extends ESocketActivity {
    public String receiver_id;
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.post_send);

        //현재 intent에 put으로 전해줬던 receiver value를 전달 받는다.
        Intent intent = new Intent(this.getIntent());
        final String receiver = intent.getStringExtra("receiver");
        receiver_id = intent.getStringExtra("receiver_id");

        //우편 보낼 때 받는이 이름을 설정해준다.
        TextView textView = (TextView) findViewById(R.id.post_item_receiver_name);
        textView.setText(receiver + " (" + receiver_id + ")");

        Button send_button = (Button) findViewById(R.id.send_button);
        send_button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                try
                {
                    TextView TitleView = (TextView) findViewById(R.id.editText_post_title);
                    TextView ContentView = (TextView) findViewById(R.id.editText_post_content);
                    JSONObject json = new JSONObject();
                    json.put("type",28);
                    json.put("title",TitleView.getText());
                    json.put("content",ContentView.getText());
                    json.put("receiver",receiver_id);
                    NetworkMain.Send(json);
                }
                catch (Exception e)
                {

                }
            }
        });
    }

    @Override
    public void ReceiveMessage(JSONObject json) {
        try {
            int type = json.getInt("type");
            switch (type)
            {
                case 28: // 보낸 결과
                    if (json.getBoolean("result")) {
                        Toast.makeText(this, "성공적으로 전송하였습니다.", Toast.LENGTH_SHORT).show();
                        onBackPressed();
                        //Intent intent = new Intent(this, MainActivity.class);
                        //startActivity(intent);
                    }
                    break;

            }
        }
        catch (Exception e)
        {
            Log.d("테스트3", e.getMessage());
        }
    }
}
