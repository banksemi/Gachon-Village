using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMan : MonoBehaviour {

    // 공개
    public float MoveSpeed;     // 플레이어를 따라오는 카메라 맨의 스피드.
    // 비공개
    public Transform Target;   // 플레이어의 트랜스 폼.
    private Vector3 Pos;        // 자신의 위치.

    void Start()
    {
        // Player라는 태그를 가진 오브젝트의 transform을 가져온다.
    }

    // 플레이어를 따라다님.
    void Update()
    {
        Pos = transform.position;
        transform.position = Target.position;
       // transform.position += (Target.position - Pos) * MoveSpeed;
    }
}
