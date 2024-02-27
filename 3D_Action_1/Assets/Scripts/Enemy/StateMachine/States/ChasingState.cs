using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾ ���󰡴� ����
/// </summary>
public class ChasingState : EnemyStateBase
{
    public bool isStepBack = true;
    public float stepBackTimer = 0f; // 24.02.25 - �ڷ� �������� Ÿ�̹�

    public override EnemyStateBase EnterCurrentState()
    {
        // �ڷ� ��������
        //Debug.Log("chasing enter");

        isStepBack = true;

        stepBackTimer = 0f;
        enemy.speed = enemy.baseSpeed;
        return this;
    }

    public override EnemyStateBase RunCurrentState()
    {
        stepBackTimer += Time.deltaTime;
        enemy.direction = enemy.Player.transform.position - enemy.transform.position; // �÷��̾� ���� ����
        enemy.Anim.SetFloat(enemy.SpeedToHash, enemy.speed);
        
        if(isStepBack)
        {
            if (stepBackTimer > 1.5f)
            {
                isStepBack = false;
                stepBackTimer = 1.5f;
            }
        
            MoveToPlayer(enemy.baseSpeed * -1);
        }
        else if (!isStepBack)
        {
            // �ִϸ��̼� ��ü�� �̵��� �������
            //enemy.Anim.SetFloat(enemy.SpeedToHash, enemy.speed); // �̵� �ִϸ��̼�

            MoveToPlayer(enemy.baseSpeed);

            if (enemy.direction.magnitude < enemy.attackRange) // �÷��̾� ��ó�� ����
            {
                Debug.Log("Attack���� ��ȯ");

                return enemy.SetEnemyState(EnemyBase.State.Attack);
            }
        }

        RotateToPlayer();
        return this;
    }

    public override EnemyStateBase ExitCurrentState()
    {
        return this;
    }

    /// <summary>
    /// �÷��̾����� �̵��ϴ� �Լ�
    /// </summary>
    void MoveToPlayer(float speed)
    {
        enemy.speed = speed;
        enemy.Rigid.MovePosition(enemy.Rigid.position + Time.fixedDeltaTime * enemy.direction.normalized * speed);
    }

    /// <summary>
    /// �÷��̾ ���� ȸ���ϴ� �Լ�
    /// </summary>
    void RotateToPlayer()
    {
        Vector3 rotDirection = Vector3.zero;
        rotDirection.x = enemy.direction.x;
        rotDirection.z = enemy.direction.z;
        rotDirection.Normalize();

        if (rotDirection.magnitude > 0.01f)
        {
            enemy.lookAngle = Mathf.Atan2(rotDirection.x, rotDirection.z) * Mathf.Rad2Deg; // ȸ���� ����
        }

        float angle = Mathf.LerpAngle(enemy.transform.localRotation.eulerAngles.y, enemy.lookAngle, enemy.rotateSpeed * Time.fixedDeltaTime);
        enemy.transform.localRotation = Quaternion.Euler(0, angle, 0); // rotate Player model
    }
}
