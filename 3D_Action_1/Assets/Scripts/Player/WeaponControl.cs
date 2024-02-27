using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �ݶ��̴��� �����ϴ� ��ũ��Ʈ
/// </summary>
public class WeaponControl : MonoBehaviour
{
    Collider coll;

    bool isEnable = false;
    [SerializeField] bool isDefenced = false;

    void Awake()
    {
        coll = GetComponent<Collider>();
        coll.enabled = false;
    }

    void FixedUpdate()
    {
        if (!coll.enabled) // �ݶ��̴��� ��Ȱ��ȭ �Ǹ� ���� ��Ȱ��ȭ
            isDefenced = false;
    }

    // ���п� ��Ҵ��� üũ
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shield"))
        {
            isDefenced = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Shield"))
        {
            isDefenced = false;
        }
    }

    /// <summary>
    /// ���� �ݶ��̴��� Ȱ��ȭ, ��Ȱ��ȭ �ϴ� �Լ� (�����ϸ� bool���� ��ȯ��, �ʱⰪ: false)
    /// </summary>
    public void ChangeColliderEnableState()
    {
        isEnable = !isEnable;
        coll.enabled = isEnable;
    }

    //public void ChangeIsDefencedState()
    //{
    //    isDefenced = !isDefenced;
    //}

    /// <summary>
    /// ������ ���п� �������� ���θ� ��ȯ�ϴ� �Լ� (true : ����, false : �ȸ���)
    /// </summary>
    /// <returns>��� ���ߴ��� Ȯ���ϴ� bool��</returns>
    public bool CheckIsDefenced()
    {
        return isDefenced;
    }
}
