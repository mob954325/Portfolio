using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : EnemyStateBase
{
    public override EnemyStateBase EnterCurrentState()
    {
        enemy.Anim.SetTrigger(enemy.DieToHash);
        GameUIManager.Instance.ShowResult();
        GameManager.Instance.player.DisablePlayerAction(); // �÷��̾� �Է� ��Ȱ��ȭ
        GameManager.Instance.BattleEnd(); // ���� Ÿ�̸� ����
        return this;
    }

    public override EnemyStateBase ExitCurrentState()
    {
        return this;
    }

    public override EnemyStateBase RunCurrentState()
    {
        return this;
    }
}