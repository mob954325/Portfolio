using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �ݸ��̴��� �����ϴ� ��ũ��Ʈ
/// </summary>
public class ShieldControl : MonoBehaviour
{
    Collider coll;

    public float timer = 0f; // �и��� �� �ִ� �ݶ��̴��� üũ�ϴ� Ÿ�̸�

    bool isEnable = false;

    void Awake()
    {
        coll = GetComponent<Collider>();
    }

    void Update()
    {
        if (isEnable)
            timer += Time.deltaTime;
        else
            timer = 0f;

        if (timer > 0.5f)
        {
            coll.enabled = false;
        }
    }

    public void ChangeColliderEnableState()
    {
        isEnable = !isEnable;
        coll.enabled = isEnable;
    }

    public void DisableCollider()
    {
        coll.enabled = false;
    }
}
