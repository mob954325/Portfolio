using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ UI ���̽� ��ũ��Ʈ
/// </summary>
public class BaseGauge : MonoBehaviour
{
    void Awake()
    {
        Init();
    }

    void Update()
    {
        UpdateUI();
    }

    /// <summary>
    /// ��ũ��Ʈ�� ����� �� ���� ���� ����� �ʱ�ȭ �Լ�
    /// </summary>
    protected virtual void Init()
    {

    }

    /// <summary>
    /// Update �Լ��� �� UI������Ʈ �Լ�
    /// </summary>
    protected virtual void UpdateUI()
    {

    }
}
