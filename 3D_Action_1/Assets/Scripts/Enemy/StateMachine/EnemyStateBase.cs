using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyStateBase : MonoBehaviour
{
    /// <summary>
    /// �� ���� ��ũ��Ʈ
    /// </summary>
    public EnemyBase enemy;

    /// <summary>
    /// ���� State�� ���� �� �� ����Ǵ� ����
    /// </summary>
    /// <returns></returns>
    public abstract EnemyStateBase EnterCurrentState();

    /// <summary>
    /// ���� State�� ���� �� �� ����Ǵ� ����
    /// </summary>
    /// <returns></returns>
    public abstract EnemyStateBase ExitCurrentState();

    /// <summary>
    /// ���� State���� ����Ǵ� ���� (Update)
    /// </summary>
    /// <returns></returns>
    public abstract EnemyStateBase RunCurrentState();
}