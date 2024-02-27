using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾ �����ϴ� ����
/// </summary>
public class AttackState : EnemyStateBase
{
    public bool isAttack = true;
    bool isBlock = false;

    public override EnemyStateBase EnterCurrentState()
    {
        isBlock = false;
        isAttack = true;
        enemy.speed = 0f;
        enemy.Anim.SetFloat(enemy.SpeedToHash, enemy.speed);// �ִϸ��̼� �Ķ���� ����

        StopCoroutine(AttackCombo());
        StartCoroutine(AttackCombo()); // ���� ����

        return this;
    }

    public override EnemyStateBase ExitCurrentState()
    {

        return this;
    }

    public override EnemyStateBase RunCurrentState()
    {
        // �÷��̾ �и��� ������ �ǰ� �ִϸ��̼� ����
        if (enemy.isAttackBlocked && !isBlock)
        {
            isBlock = true;
            OnPlayerParrying();

            if(enemy.Toughness == 0)
                return enemy.SetEnemyState(EnemyBase.State.Faint);
        }

        // ������ ������ chasing���� ���ư���
        if (!isAttack)
            return enemy.SetEnemyState(EnemyBase.State.Chasing);

        return this;
    }

    IEnumerator AttackCombo()
    {
        enemy.Anim.SetTrigger(enemy.AttackToHash);
        int randAnimNum = UnityEngine.Random.Range(0, enemy.attackAnimNum); // ���� ����  �ִϸ��̼� �������� (0 - attackAnimNum �̸� ��)

        enemy.Anim.SetInteger(enemy.randomAttackToHash, randAnimNum);

        float animTime = enemy.GetAnimClipLength($"Attack{randAnimNum}");

        yield return new WaitForSeconds(animTime); // 2f / 24.02.25 - �ִϸ��̼� ����ð��� ���� �ڷ�ƾ ���ð� ���ϱ�
        isAttack = false;
    }

    void OnPlayerParrying()
    {
        enemy.Toughness -= 20;
        enemy.Anim.SetTrigger(enemy.DamagedToHash);
        enemy.changeWeaponCollider();
    }
}
