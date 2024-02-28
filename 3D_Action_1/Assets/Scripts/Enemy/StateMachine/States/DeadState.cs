using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : EnemyStateBase
{
    public override EnemyStateBase EnterCurrentState()
    {
        enemy.Anim.SetTrigger(enemy.DieToHash);

        if(enemy.type == EnemyBase.Type.Boss)
        {
            GameManager.Instance.BattleEnd(); // ���� Ÿ�̸� ����
            GameManager.Instance.player.DisablePlayerAction(); // �÷��̾� �Է� ��Ȱ��ȭ
            GameUIManager.Instance.ShowResult();
        }

        StartCoroutine(DisableObj());

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

    IEnumerator DisableObj()
    {
        float time = enemy.GetAnimClipLength("Death");
        yield return new WaitForSeconds(time + 0.5f);
        enemy.gameObject.SetActive(false);
    }
}
