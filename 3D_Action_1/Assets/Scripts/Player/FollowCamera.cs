using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// ī�޶� ��ġ �����ϴ� Ŭ����
/// </summary>
public class FollowCamera : MonoBehaviour
{
    public Player player;
    public Vector3 Offset;
    float length; // racast �Ÿ�
    float vCameraDistance; // vitualCamera's Distance
    Cinemachine3rdPersonFollow vCamera;

    void Start()
    {
        player = FindAnyObjectByType<Player>();
        vCamera = FindAnyObjectByType<Cinemachine3rdPersonFollow>();
        vCameraDistance = vCamera.CameraDistance;
    }

    void FixedUpdate()
    {
        transform.position = player.gameObject.transform.position + Offset;

        // ī�޶� ����
        Vector3 dir = (player.gameObject.transform.position + Offset) - Camera.main.transform.position;
        length = dir.magnitude;

        Ray ray = new Ray(Camera.main.transform.position, dir);

        if(Physics.Raycast(ray, out RaycastHit hitInfo, length))
        {
            if(hitInfo.collider.gameObject.layer != 6) // Ray�� �÷��̾ �ƴ� �ٸ� ���̾�(������Ʈ)�� ����� ��
            {
                //Debug.DrawRay(Camera.main.transform.position, dir, Color.red);
                //Debug.Log(vCamera.CameraDistance);
                vCamera.CameraDistance = vCamera.CameraDistance - (Camera.main.transform.position - hitInfo.point).magnitude; // ������ ī�޶� ��ġ
            }
            else
            {
                vCamera.CameraDistance = vCameraDistance;
            }
        }
    }
}