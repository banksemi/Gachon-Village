using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class Player : MonoBehaviour {
    private CharacterController cc;
    public float Speed;
    public GameObject man;
    // Use this for initialization
    public float VSpeed = 0;
    public float movetime = 0;
    public bool moveok = false;

    float q = 0;
    private BoxCollider col;
    void Start () {
        cc = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (col == null)
        {
            if (transform.childCount == 0) return;
            else
            {
                col = transform.GetChild(0).GetComponent<BoxCollider>();
                cc.center = col.center;
            }
        }
        Run();
    }
    void Run()
    {
        bool JumpButton = false;
        Vector3 v = new Vector3();
        if (UIInput.selection == null) // 다른 UIInput에 입력중이 아닌경우
        {
            if (Input.GetKey(KeyCode.A)) v += Vector3.left;
            if (Input.GetKey(KeyCode.D)) v -= Vector3.left;
            if (Input.GetKey(KeyCode.W)) v += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) v -= Vector3.forward;
            if (Input.GetKey(KeyCode.Space)) JumpButton = true;
        }
        if (cc.isGrounded)
        {
            if (JumpButton) VSpeed = 14;
            if (VSpeed < 0) VSpeed = 0;
        }
        if (Input.GetMouseButton(1))
        {
            transform.rotation = man.transform.rotation;
        }
        v = v.normalized * Speed;
        VSpeed -= 40 * Time.deltaTime;
        v.y = VSpeed;
        v = transform.rotation * v;
        Vector3 vc = transform.position;
        cc.Move(v * Time.deltaTime);
        if (moveok)
        {
            if (movetime >= NetworkMain.MoveDeley)
            {
                moveok = false;
                JObject json = new JObject();
                json["type"] = NetworkProtocol.Move;
                json["x"] = vc.x;
                json["y"] = vc.y;
                json["z"] = vc.z;
                json["q"] = transform.rotation.eulerAngles.y;
                NetworkMain.SendMessage(json);
                movetime -= NetworkMain.MoveDeley;
            }
        }
        if (vc != transform.position || q != transform.rotation.eulerAngles.y)
        {
            q = transform.rotation.eulerAngles.y;
            if (movetime > NetworkMain.MoveDeley)
            {
                movetime = 0;
            }

            moveok = true;
        }
        movetime += Time.deltaTime;

    }
}
