package gachon.cafe.gavigation;

import android.content.Intent;
import android.os.Bundle;
import android.widget.TextView;

public class SendMessageActivity extends ESocketActivity {
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.post_send);

        //현재 intent에 put으로 전해줬던 receiver value를 전달 받는다.
        Intent intent = new Intent(this.getIntent());
        String receiver = intent.getStringExtra("receiver");

        //우편 보낼 때 받는이 이름을 설정해준다.
        TextView textView = (TextView) findViewById(R.id.post_item_receiver_name);
        textView.setText(receiver);
    }
}
