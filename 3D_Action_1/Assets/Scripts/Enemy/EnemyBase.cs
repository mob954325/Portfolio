using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� ���̽� Ŭ����
/// </summary>
public class EnemyBase : MonoBehaviour
{
    // Delegate
    Action onAttack;

    // Components
    public Player player;
    WeaponControl weapon;
    Rigidbody rigid;
    Animator animator;
    SoundControl soundControl;
    public EnemyStateBase[] enemyStates;

    // ������Ƽ
    public Player Player => player;
    public WeaponControl Weapon => weapon;
    public Rigidbody Rigid => rigid;
    public Animator Anim => animator;

    // ����
    public enum State
    {
        Idle = 0,
        Chasing,
        Attack,
        Faint,
        Death
    }

    public State states;

    // Values
    public Vector3 direction = Vector3.zero;                // �ٶ󺸴� ���� (vector3)
    public float lookAngle;                                 // �ٶ󺸴� ���� (float)

    // Enemy stats
    [Header("Enemy Stats")]
    public float baseSpeed = 3.0f;                          // �� �̵� �ӵ��� �����ϴ� ����
    public float speed = 3.0f;                              // �� �̵� �ӵ�
    public float rotateSpeed = 5.0f;                        // �� ȸ�� �ӵ�
    [Space(10f)]
    public float attackRange = 2.0f;                        // ���� ����
    public float attackDelay = 2.5f;                        // ���� ������
    public float ToughnessDelay = 2f;                       // ���μ� ���� ������ �ð�
    [HideInInspector]public float StepBackTime = 0f;        // �������� �ð� (���� ��)

    int hp;
    public int maxHp = 10;
    public int HP
    {
        get => hp;
        set
        {
            hp = value;
            //Debug.Log($"���� ü���� [{hp}]��ŭ ���ҽ��ϴ�");

            if (hp <= 0)
            {
                hp = 0;
                isDie = true;
            }
        }
    }

    int toughness = 0; // ���μ� (0�̵Ǹ� ����)
    public int maxToughness = 100;

    /// <summary>
    /// ���μ�(toughness)�� �����ϴ� �Ķ���� (0�̵Ǹ� ���� �ִϸ��̼��� �����Ѵ�)
    /// </summary>
    public int Toughness
    {
        get => toughness;
        set
        {
            toughness = value;
            //Debug.Log($"���� ���μ� : [{toughness}]");

            // ����
            if (toughness <= 0)
            {
                toughness = 0;
                speed = 0;
            }
        }
    }

    /// <summary>
    /// ���� �ִϸ��̼� ����
    /// </summary>
    [Tooltip("Set Attack Animation Num")]
    public int attackAnimNum;

    // bool
    public bool isAttackBlocked => weapon.CheckIsDefenced(); // ������ �������� Ȯ���ϴ� ����

    private bool isDamaged = false;
    public bool IsDamaged => isDamaged; // ������ �Ծ����� Ȯ���ϴ� ������Ƽ

    private bool isDie = false;                 
    public bool IsDie => isDie;         // �׾����� Ȯ���ϴ� ������Ƽ

    // ���� �������� �������� �ִϸ��̼� �Ķ����
    // Hashes
    public readonly int SpeedToHash = Animator.StringToHash("Speed");
    public readonly int AttackToHash = Animator.StringToHash("Attack");
    public readonly int DamagedToHash = Animator.StringToHash("Damaged");
    public readonly int DieToHash = Animator.StringToHash("Die");
    public readonly int faintToHash = Animator.StringToHash("Faint"); // ���� trigger 
    public readonly int isFaintToHash = Animator.StringToHash("isFaint"); // ������ ������ ������ ����ϰ� �ϴ� animator bool��
    public readonly int randomAttackToHash = Animator.StringToHash("RandomAttack"); // ���� �ִϸ��̼� �� ��� �ִϸ��̼����� �����ϴ� int ��
    void Awake()
    {
        // �ʱ�ȭ
        HP = maxHp;
        Toughness = maxToughness;

        // component
        rigid = GetComponent<Rigidbody>();
        player = FindAnyObjectByType<Player>();
        animator = GetComponent<Animator>();
        weapon = GetComponentInChildren<WeaponControl>();
        soundControl = GetComponentInChildren<SoundControl>();

        // delegate
        onAttack += weapon.ChangeColliderEnableState;

        // states
        enemyStates = new EnemyStateBase[5]; // stat �迭 �ʱ�ȭ
        for(int i = 0; i < enemyStates.Length; i++)
        {
            enemyStates[i] = transform.GetChild(i).gameObject.GetComponent<EnemyStateBase>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerAttack") && !IsDamaged && !isDie) // �÷��̾��� ������ �޾����� ������ �ޱ�
        {
            isDamaged = true;
            Anim.SetTrigger(DamagedToHash);
            HP--;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("PlayerAttack") && IsDamaged && !isDie) // ���ݿ� ����� �ǰ� ��Ȱ��ȭ
        {
            isDamaged = false;  
        }
    }

    /// <summary>
    /// ���� �ݶ��̴��� ���¸� �����ϴ� �Լ� ( �̺�Ʈ �ִϸ��̼� )
    /// </summary>
    public void changeWeaponCollider()
    {
        onAttack?.Invoke();
    }

    /// <summary>
    /// ���¸� �޴� �Լ�
    /// </summary>
    /// <param name="state">���� ���� �Է�</param>
    public EnemyStateBase SetEnemyState(State state)
    {
        EnemyStateBase selectState = null;
        switch(state)
        {
            case State.Idle:
                selectState = enemyStates[(int)State.Idle].GetComponent<IdleState>();
                break;
            case State.Chasing:
                selectState = enemyStates[(int)State.Chasing].GetComponent<ChasingState>();
                break;
            case State.Attack:
                selectState = enemyStates[(int)State.Attack].GetComponent<AttackState>();
                break;
            case State.Faint:
                selectState = enemyStates[(int)State.Faint].GetComponent<FaintState>();
                break;
            case State.Death:
                selectState = enemyStates[(int)State.Death].GetComponent<DeadState>();
                break;
        }
        return selectState;
    }

    /// <summary>
    /// Ư�� �ִϸ��̼� �ð��� ã�� �Լ�
    /// </summary>
    /// <param name="clipName">ã�� �ִϸ��̼� �̸�</param>
    /// <returns>clipName �ִϸ��̼� �ð�</returns>
    public float GetAnimClipLength(string clipName)
    {
        float time = 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == clipName)
            {
                time = ac.animationClips[i].length;
            }
        }
        return time;
    }

    /// <summary>
    /// �Ҹ��� �����ϴ� �Լ�
    /// </summary>
    public void PlayAttackSound()
    {
        int rand = UnityEngine.Random.Range(0, soundControl.audioSources.Length);
        soundControl.audioSources[rand].Play();
    }
}
