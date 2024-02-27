using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Toughness�� 0�̵Ǹ� ����Ǵ� ��ũ��Ʈ
/// </summary>
public class FaintState : EnemyStateBase
{
    float timer = 0f;
    float faintTime; // ���� �ð�

    bool isFaintEnd = true;

    public override EnemyStateBase EnterCurrentState()
    {
        // �ʱ�ȭ
        float animTime = enemy.GetAnimClipLength("Faint"); // �ִϸ��̼� ����ð�
        faintTime = animTime + 0.5f; // �Ͼ�� �ð�
        timer = 0f; // Ÿ�̸� �ʱ�ȭ
        isFaintEnd = true; // ���� �ʱ�ȭ

        enemy.Anim.SetTrigger(enemy.faintToHash); // ���� �ִϸ��̼�

        StopCoroutine(FaintAnimCoroutine());
        StartCoroutine(FaintAnimCoroutine());
        return this;
    }

    public override EnemyStateBase ExitCurrentState()
    {
        return this;
    }

    public override EnemyStateBase RunCurrentState()
    {
        if(!isFaintEnd) timer += Time.deltaTime;

        if(timer > faintTime)
        {
            enemy.Toughness = enemy.maxToughness; // ���μ� �ʱ�ȭ �� 
            return enemy.SetEnemyState(EnemyBase.State.Chasing); // ���� ����
        }

        return this;
    }

    IEnumerator FaintAnimCoroutine()
    {
        enemy.Anim.SetBool(enemy.isFaintToHash, isFaintEnd); // ���� bool �ִϸ��̼�
        yield return new WaitForSeconds(1.2f);

        isFaintEnd = false;
        enemy.Anim.SetBool(enemy.isFaintToHash, isFaintEnd); // ���� bool �ִϸ��̼�
    }
}
