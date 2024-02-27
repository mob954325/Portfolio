using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// EnemyBarObject�� �� ��ũ��Ʈ
/// </summary>
public class EnemyBar : BaseGauge
{
    // HSEnemy
    EnemyBase enemy;

    Image currentEnemyHP;
    Image currentEnemyToughness;

    protected override void Init()
    {
        enemy = FindAnyObjectByType<EnemyBase>(); // enemy ��ũ��Ʈ�� �ִ� ������Ʈ ã��
        if(enemy == null)
        {
            // �ӽ�
            enemy = FindAnyObjectByType<EnemyBase>(); // enemy ��ũ��Ʈ�� �ִ� ������Ʈ ã��
            gameObject.SetActive(false); // �����
        }

        Transform hpChild = transform.GetChild(0).GetChild(1);
        Transform toughChild = transform.GetChild(1).GetChild(1);

        currentEnemyHP = hpChild.GetComponent<Image>();
        currentEnemyToughness = toughChild.GetComponent<Image>();
    }

    protected override void UpdateUI()
    {
        if(enemy.HP < 1) // ���� ����߰ų� ������
        {
            gameObject.SetActive(false); // �����
        }

        currentEnemyHP.fillAmount = enemy.HP / (float)enemy.maxHp;
        currentEnemyToughness.fillAmount = enemy.Toughness / (float)enemy.maxToughness;

    }
}
