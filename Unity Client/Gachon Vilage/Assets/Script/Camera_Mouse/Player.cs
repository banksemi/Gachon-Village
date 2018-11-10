using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private CharacterController cc;
    public float Speed;
    public GameObject man;
    // Use this for initialization
    public float VSpeed = 0;
	void Start () {
        cc = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        Run();

    }
    void Run()
    {
        int ButtonDown = 0;
        Vector3 v = new Vector3();
        if (Input.GetKey(KeyCode.A)) v += Vector3.left;
        if (Input.GetKey(KeyCode.D)) v -= Vector3.left;
        if (Input.GetKey(KeyCode.W)) v += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) v -= Vector3.forward;
        if (cc.isGrounded)
        {
            if (Input.GetKey(KeyCode.Space)) VSpeed = 14;
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
        cc.Move(v * Time.deltaTime);

    }
}
