using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Player player;

    /// <summary>
    /// ������ �����ߴ��� Ȯ���ϴ� ���� (true : ������, false : ����)
    /// </summary>
    public bool isBattle = false;

    /// <summary>
    /// ���ӽð�
    /// </summary>
    public float timer = 0.0f;

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();
        timer = 0.0f;
    }

    void Start()
    {
        isBattle = true;
    }

    void Update()
    {
        if(isBattle)
            timer += Time.deltaTime;
    }

    public void BattleBegin()
    {
        isBattle = true;
    }

    public void BattleEnd()
    {
        isBattle = false;
    }
}
