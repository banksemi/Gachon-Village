using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axis : MonoBehaviour
{
    public Quaternion TargetRotation;
    public Vector3 Gap;               // 회전 축적 값.
    public float RotationSpeed;  
    // 공개
    public float Distance;          // 카메라와의 거리.

    // 비공개
    private Vector3 AxisVec;        // 축의 벡터.
    private Transform MainCamera;   // 카메라 컴포넌트.
    public float ZoomSpeed = 3;
    public Transform CameraVector;
    private bool First_Click_is_UI = false;
    private Vector3 LastFrameMouse;
    void Start()
    {
        MainCamera = Camera.main.transform;
        CameraVector = transform.parent;
        TargetRotation = Quaternion.Euler(Gap);

        transform.rotation = TargetRotation;
    }

    // Update is called once per frame
    void Update()
    {

        DisCamera();
        CameraRotation();
    }
    void CameraRotation()
    {
        if (transform.rotation != TargetRotation)
            transform.rotation = TargetRotation;
        if (UIInput.selection == null && (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)))
        {
            if (Input.GetKey(KeyCode.Q))
                Gap.y -= 0.5f * RotationSpeed;
            else
                Gap.y += 0.5f * RotationSpeed;

            TargetRotation = Quaternion.Euler(Gap);

            // 카메라벡터 객체에 Axis객체의 x,z회전 값을 제외한 y값만을 넘긴다.
            Quaternion q = TargetRotation;
            q.x = q.z = 0;

            CameraVector.transform.rotation = q;
        }
        if (Input.GetMouseButton(0) == false && Input.GetMouseButton(1) == false) First_Click_is_UI = false;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (UICamera.Raycast(Input.mousePosition))
                First_Click_is_UI = true;
            LastFrameMouse = Input.mousePosition;
        }
        if (First_Click_is_UI == false && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
        {
            Vector3 axis =  Input.mousePosition - LastFrameMouse;
            axis /= 10;
            LastFrameMouse = Input.mousePosition;
            // 값을 축적.
            Gap.x += axis.y * RotationSpeed * -1;
            Gap.y += axis.x * RotationSpeed;
            // 카메라 회전범위 제한.
            Gap.x = Mathf.Clamp(Gap.x, -5f, 85f);
            // 회전 값을 변수에 저장.
            TargetRotation = Quaternion.Euler(Gap);

            // 카메라벡터 객체에 Axis객체의 x,z회전 값을 제외한 y값만을 넘긴다.
            Quaternion q = TargetRotation;
            q.x = q.z = 0;
            CameraVector.transform.rotation = q;
           //Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {

            //Cursor.lockState = CursorLockMode.None;
        }
    }

        void DisCamera()
    {
        if (UICamera.Raycast(Input.mousePosition) == false)
            Distance += Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * -1;
        Distance = Mathf.Clamp(Distance, 5f, 30f);


        AxisVec = transform.forward * -1;
        AxisVec *= Distance;
        MainCamera.position = transform.position + AxisVec;
        
    }

}
