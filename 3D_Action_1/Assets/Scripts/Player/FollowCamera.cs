using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ī�޶� ��ġ �����ϴ� Ŭ����
/// </summary>
public class FollowCamera : MonoBehaviour
{
    public Player player;
    public Vector3 Offset;

    void Start()
    {
        player = FindAnyObjectByType<Player>();
    }

    void FixedUpdate()
    {
        transform.position = player.gameObject.transform.position + Offset;
    }
}
