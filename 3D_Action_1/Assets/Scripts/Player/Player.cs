using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾� ��ǲ ���� �ൿ�� �ٷ�� Ŭ����
/// </summary>
public class Player : MonoBehaviour
{
    // Action
    /// <summary>
    /// �÷��̾ ������ �� �����ϴ� ��������Ʈ
    /// </summary>
    Action OnAttack;
    /// <summary>
    /// �÷��̾ ����� �� �����ϴ� ��������Ʈ
    /// </summary>
    Action OnDefence;

    /// <summary>
    /// �÷��̾� ���ͷ��� ��������Ʈ
    /// </summary>
    public Action OnInteractionAction;

    // components
    PlayerInputActions actions;
    Animator animator;
    Rigidbody rigid;
    EnemyBase enemy;
    SoundControl soundControl;
    
    WeaponControl weapon;
    ShieldControl shield;

    // �÷��̾� ��ǲ���� �޴� ����
    [Header("Input Value")]
    public Vector3 playerInput;
    public Vector2 mouseInput;

    // player Stats
    [Header("Input Stats")]
    public float moveSpeed = 5.0f; // �÷��̾��� ���� ���ǵ� ����
    float baseSpeed; // �÷��̾� ���ǵ� ���� ����

    [Range(0f,1f)]
    public float rotationPower = 5.0f; // �÷��̾� ȸ����
    public float rotSpeed = 15f; // �÷��̾� ȸ�� �ӵ�
    public float jumpPower = 5.0f; // ������

    [Space(12f)]
    [Range(1.5f,3f)]
    public float attackDelayTime = 3.0f; // ���� ������ �ð�
    [SerializeField] float attackDelayTimer; // ���� ������ Ÿ�̸�
    /// <summary>
    /// attackDelayTimer ������Ƽ
    /// </summary>
    float AttackDelayTimer
    {
        get => attackDelayTimer;
        set
        {
            attackDelayTimer = value;

            if(attackDelayTimer < 0f)
            {
                attackDelayTimer = 0f;
            }
        }
    }

    public float defenceDelayTime = 3.0f; // ��� ������ �ð�
    [SerializeField] float defenceDelayTimer; // ��� ������ Ÿ�̸�
    /// <summary>
    /// defenceDelayTimer ������Ƽ
    /// </summary>
    float DefenceDelayTimer
    {
        get => defenceDelayTimer;
        set
        {
            defenceDelayTimer = value;

            if (defenceDelayTimer < 0f)
            {
                defenceDelayTimer = 0f;
            }
        }
    }
    

    [Space(12f)]
    // Hp
    public int maxhp = 5;
    int hp;
    public int HP // �÷��̾� ü�� ������Ƽ
    {
        get => hp;
        set
        {
            hp = value;
            //Debug.Log($"�÷��̾��� ü���� [{hp}]��ŭ ���ҽ��ϴ�");

            if (hp <= 0)
            {
                hp = 0;
                isDie = true;
                Die();
            }
        }
    }

    [Header("Objects")]
    // player's Transform objects
    public Transform cameraFollowTransform;
    public Transform playerModel;

    // player movement
    [Header("Movement")]
    Vector3 moveDirection; // �÷��̾� ������ ����
    float inputVertical = 0f; // �÷��̾��� �� �� ��ǲ��
    float inputHorizontal = 0f; // �÷��̾��� �� �� ��ǲ��

    // player animator
    readonly int inputVerticalToHash = Animator.StringToHash("Vertical"); // input.z
    readonly int inputHorizontalToHash = Animator.StringToHash("Horizontal"); // input.x
    readonly int jumpToHash = Animator.StringToHash("Jump");
    readonly int attackToHash = Animator.StringToHash("Attack");
    readonly int damagedToHash = Animator.StringToHash("Damaged");
    readonly int defenceToHash = Animator.StringToHash("isDefence"); // ���� ��ġ�⸦ �ϴ� animator parameter ( true : ���� ���, false : ���з� ġ��)
    readonly int ActiveDefenceToHash = Animator.StringToHash("ActiveDefence");
    readonly int DieToHash = Animator.StringToHash("Die");

    // player flag
    bool isJump = false;
    bool isAttack = false;
    [SerializeField]bool isDamaged = false;
    public bool isDefence = false; // �÷��̾ �� �ϴ��� Ȯ���ϴ� bool
    bool isLockOn = false; // �÷��̾ ������ Ȱ��ȭ �ߴ��� Ȯ���ϴ� bool
    public bool isDefenceAttack = false; // �÷��̾ ���й�ġ�⸦ �ߴ��� Ȯ���ϴ� bool
    bool isDie = false;
    //float checkEnemyAngle = 0f;

    void Awake()
    {
        // ������Ʈ
        actions = new PlayerInputActions();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        enemy = FindAnyObjectByType<EnemyBase>();
        weapon = GetComponentInChildren<WeaponControl>();
        shield = GetComponentInChildren<ShieldControl>();
        cameraFollowTransform = FindAnyObjectByType<FollowCamera>().transform;
        soundControl = GetComponentInChildren<SoundControl>();
        playerModel = transform.GetChild(0);

        // ���� �ʱ�ȭ
        HP = maxhp;
        baseSpeed = moveSpeed;

        // ��������Ʈ
        OnAttack += weapon.ChangeColliderEnableState; 
        OnDefence += shield.ChangeColliderEnableState;
    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; // Ŀ�� ����
        //Cursor.visible = false; // Ŀ�� ������
        rigid.freezeRotation = true;
    }

    void OnEnable()
    {
        actions.Player.Enable();
        actions.Player.Move.performed += OnMoveInput;
        actions.Player.Move.canceled += OnMoveInput;
        actions.Player.Jump.performed += OnJumpInput;
        actions.Player.Jump.canceled += OnJumpInput;
        actions.Player.Look.performed += OnLookInput;
        actions.Player.Look.canceled += OnLookInput;
        actions.Player.Attack.performed += OnAttackInput;
        actions.Player.Attack.canceled += OnAttackInput;
        actions.Player.Defence.performed += OnDefenceInput;
        actions.Player.Defence.canceled += OnDefenceInput;
        actions.Player.LockOn.performed += OnLockCameraInput;
        actions.Player.LockOn.canceled += OnLockCameraInput;
        actions.Player.Interaction.performed += OnInteraction;
        actions.Player.Interaction.canceled += OnInteraction;
    }

    private void OnInteraction(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            OnInteractionAction?.Invoke();
        }
    }

    void OnDisable()
    {
        actions.Player.Interaction.canceled -= OnInteraction;
        actions.Player.Interaction.performed -= OnInteraction;
        actions.Player.LockOn.canceled -= OnLockCameraInput;
        actions.Player.LockOn.performed -= OnLockCameraInput;
        actions.Player.Defence.canceled -= OnDefenceInput;
        actions.Player.Defence.performed -= OnDefenceInput;
        actions.Player.Attack.canceled -= OnAttackInput;
        actions.Player.Attack.performed -= OnAttackInput;
        actions.Player.Look.canceled -= OnLookInput;
        actions.Player.Look.performed -= OnLookInput;
        actions.Player.Jump.canceled -= OnJumpInput;
        actions.Player.Jump.performed -= OnJumpInput;
        actions.Player.Move.canceled -= OnMoveInput;
        actions.Player.Move.performed -= OnMoveInput;
        actions.Player.Disable();
    }

    void Update()
    {
        // Timer
        DefenceDelayTimer -= Time.deltaTime;
        AttackDelayTimer -= Time.deltaTime;

    }

    void FixedUpdate()
    {
        playerMove();
        GetPlayerMoveInput();
        PlayerRotate();
        // rotatecamera
        RotateCamera();
        PlayAnimMove();

        // ī�޶� ����
        if(isLockOn)
            cameraFollowTransform.LookAt(enemy.transform);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isJump = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            rigid.position = transform.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // �� ���� ����
        if (other.CompareTag("EnemyAttack") && !isDamaged && !isDie)
        {
            if (!isDefence) // �ǰݴ��� ��, ���и� �ȵ���� ��, ���� ��ġ�⸦ �������� �ʾ��� ��
            {
                animator.SetTrigger(damagedToHash);
                HP--;

                rigid.AddForce(enemy.transform.forward * 70f, ForceMode.Impulse); // 24.02.25 , �� �������� �˹�
            }
            else if (other.CompareTag("EnemyAttack") && isDefence && !isDie)
            {
                Debug.Log("asdf");
                isDamaged = true;
                rigid.AddForce(enemy.transform.forward * 45f, ForceMode.Impulse); // �� �������� �˹�
            }
            //StartCoroutine(HitDelay());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyAttack") && isDefence && !isDie)
        {
            rigid.AddForce(enemy.transform.forward * 45f, ForceMode.Impulse); // �� �������� �˹�
        }
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        playerInput = context.ReadValue<Vector3>();
    }

    void GetPlayerMoveInput()
    {
        // CALL AFTER playerMove()
        inputVertical = playerInput.z;
        inputHorizontal = playerInput.x;
    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        if(!isJump)
        {
            animator.SetTrigger(jumpToHash);
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJump = true;
        }
    }

    void RotateCamera()
    {
        if(isLockOn)
            return;

        #region Vertical Rotation

        // Rotate the follow target transfrom based on input

        // leftright
        cameraFollowTransform.transform.rotation *= Quaternion.AngleAxis(mouseInput.x * rotationPower, Vector3.up);

        // updown
        cameraFollowTransform.transform.rotation *= Quaternion.AngleAxis(mouseInput.y * -rotationPower, Vector3.right);

        Vector3 angles = cameraFollowTransform.transform.localEulerAngles;
        angles.z = 0;

        float angle = cameraFollowTransform.transform.localEulerAngles.x;

        // Clamp the up/down rotation
        if(angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if(angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        cameraFollowTransform.transform.localEulerAngles = angles;

        #endregion

        // Reset the y rotation of the look transform
        cameraFollowTransform.transform.localEulerAngles = new Vector3(angles.x, angles.y, 0);

    }

    /// <summary>
    /// player model rotate
    /// </summary>
    void PlayerRotate()
    {
        if (isDefence || isAttack)
            return;

        Vector3 rotDirection = Vector3.zero;
        rotDirection.x = inputHorizontal;
        rotDirection.z = inputVertical;
        rotDirection.Normalize(); // ȸ�� ���� ����

        // �Է�Ű ���� �� ȸ���� + ī�޶� ȸ���� = ���� �÷��̾� ���� ȸ���� y��
        if(!isLockOn)
        {
            if(rotDirection.magnitude > 0.01f)
            {
                float lookAngle = Mathf.Atan2(rotDirection.x, rotDirection.z) * Mathf.Rad2Deg; // ȸ���� ����
                float lerpLookAngle = Mathf.LerpAngle(playerModel.localRotation.eulerAngles.y, 
                                                      lookAngle + cameraFollowTransform.rotation.eulerAngles.y, 
                                                      rotSpeed * Time.fixedDeltaTime);

                playerModel.localRotation = Quaternion.Euler(0, lerpLookAngle, 0); // rotate Player model
            }

        }
        else if(isLockOn)
        {
            // ī�޶� ���� ���� �� ȸ��
            float angle = Mathf.LerpAngle(playerModel.localRotation.eulerAngles.y, cameraFollowTransform.rotation.eulerAngles.y, rotSpeed * Time.fixedDeltaTime);
            playerModel.localRotation = Quaternion.Euler(0, angle, 0); // rotate Player model
        }
    }

    void playerMove()
    {
        if (isDefence)
            return;

        // calculate movement direction
        moveDirection = cameraFollowTransform.forward * inputVertical + cameraFollowTransform.right * inputHorizontal; // Player Move Direction
        moveDirection.y = 0f;

        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * moveDirection.normalized * moveSpeed); // player Move position
    }

    void PlayAnimMove()
    {
        // check input value
        animator.SetFloat(inputVerticalToHash, inputVertical);
        animator.SetFloat(inputHorizontalToHash, inputHorizontal);
    }

    private void OnAttackInput(InputAction.CallbackContext context)
    {
        if(context.performed && !isAttack
        && !isDefence && !isDefenceAttack)// ��� ���� �ƴ� ��, ���� ��ġ�⸦ ���� ���� ��
        {
            if (AttackDelayTimer > 0f)
            {
                Debug.Log($"���� ��Ÿ���� [{AttackDelayTimer}] ���ҽ��ϴ� !!!!");
                return;
            }

            moveSpeed = 0f; // ������ �� ����.
            animator.SetTrigger(attackToHash); // �ִϸ��̼� ����
            StartCoroutine(AttackDelay()); 
        }
    }

    IEnumerator AttackDelay()
    {
        AttackDelayTimer = attackDelayTime; // ���� ��Ÿ��
        isAttack = true;
        OnAttack?.Invoke();

        float time = GetAnimClipLength("Player_Attack"); // �ִϸ��̼� �ð� �ޱ�
        yield return new WaitForSeconds(time);

        OnAttack?.Invoke();
        isAttack = false;

        moveSpeed = baseSpeed;
    }

    // Defence
    private void OnDefenceInput(InputAction.CallbackContext context)
    {

        if (context.performed && !isDefence
            && !isAttack) // �����ϴ� ���� �ƴҶ�
        {
            if (DefenceDelayTimer > 0f) // ��Ÿ��
                return;

            DefenceDelayTimer = defenceDelayTime; // ��Ÿ��

            // Set animator paramaters
            animator.SetTrigger(ActiveDefenceToHash); // ���� ��� Ʈ����
            animator.SetBool(defenceToHash, true); // ���� ��ġ�� �غ�

            OnDefence?.Invoke(); // ���� �ݶ��̴� Ȱ��ȭ

            isDefence = true; // ���Ȱ��ȭ

        }
        if (context.canceled && isDefence)
        {
            animator.SetBool(defenceToHash, false); // ���� ��ġ�� �غ�
            isDamaged = true; // ���� Ȱ��ȭ 
            StartCoroutine(AfterDefence());
        }
    }

    /// <summary>
    /// ��� ���� ���� �ð� �ڷ�ƾ(Defencing attack + 0.5f)
    /// </summary>
    /// <returns></returns>
    IEnumerator AfterDefence()
    {
        animator.SetBool(defenceToHash, false);

        isDefence = false; // ���� ��ġ�� �ִϸ��̼� ����
        float defenceAnimTime = GetAnimClipLength("Player_Defence_Attack"); // 0.9s

        isDefenceAttack = true;
        yield return new WaitForSeconds(defenceAnimTime);

        OnDefence?.Invoke(); // ���� �ݶ��̴� ��Ȱ��ȭ
        isDefenceAttack = false;
        isDamaged = false; // ���� ���� ��Ȱ��ȭ
    }

    private void OnLockCameraInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isLockOn = !isLockOn;
        }
    }

    IEnumerator HitDelay()
    {
        isDamaged = true;
        yield return new WaitForSeconds(2f);
        isDamaged = false;
    }

    /// <summary>
    /// �÷��̾� ��� �Լ�
    /// </summary>
    void Die()
    {
        DisablePlayerAction();
        animator.SetTrigger(DieToHash);
        GameUIManager.Instance.ShowResult(true);
        GameManager.Instance.BattleEnd();
    }

    public void DisablePlayerAction()
    {
        actions.Player.Disable();
    }

    /// <summary>
    /// Ư�� �ִϸ��̼� �ð��� ã�� �Լ�
    /// </summary>
    /// <param name="clipName">ã�� �ִϸ��̼� �̸�</param>
    /// <returns>clipName �ִϸ��̼� �ð�</returns>
    float GetAnimClipLength(string clipName)
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
