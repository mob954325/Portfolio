using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Mathematics;
[RequireComponent(typeof(Animator))]

public class Player : MonoBehaviour
{
    [Header("#AfterImage Info")]
    Action _dashAction;
    public float _dashDelay;
    public GameObject _afterImage;
    public Vector3 _dashSpot;
    public Sprite _lastSprite;
    Vector2 _lastInput;

    // Component
    PlayerInput_ _inputActions;
    Rigidbody2D _rigid;
    Animator _animator;
    SpriteRenderer _sprite;

    // _ray
    RaycastHit2D _ray;
    [SerializeField] float _distance = 0f; // 2.4f
    [SerializeField] float _distanceray = 0f; // 2.24f
    
    // delegate
    Action<int> _changeScore;

    [Header("Score")]
    [SerializeField] private int _maxScore = 9999;
    [SerializeField] private int _score;
    public int _Score
    {
        get => _score;
        set
        {
            if (_score != value)
            {
                _score = Math.Max(value, _maxScore);
                //_changeScore?.Invoke(_score); delegate to score UI
            }
        }

    }

    [Header("#Player Obj info")]
    Action _isMeetBoss;
    Action _changeHpUI;
    public GameObject _playerHealthUI;
    private int _hp = 3;
    public int _maxHp = 3;
    public int _Hp
    {
        get => _hp;
        set
        {
            if (_hp != value)
            {
                _hp = value;
                if (_hp <= 0)
                {
                    _hp = 0;
                    Die();
                }
                _changeHpUI?.Invoke();
            }
        }
    }

    public Vector2 _inputMove = Vector2.zero;
    public GameObject _bulletPos;
    public GameObject _bullet;
    public GameObject _bulletHitEffect;

    // Animator Parameter
    readonly int speed_String = Animator.StringToHash("speed");
    readonly int isJump_String = Animator.StringToHash("isJump");
    readonly int isCrouch_String = Animator.StringToHash("isCrouch");
    readonly int isClimb_String = Animator.StringToHash("isClimb");
    readonly int isHit_String = Animator.StringToHash("isHit");

    const string isShot_string = "isShot";
    const string isRunningShot_string = "isRunningShot";

    [Header("#Player Stats")]
    float _gravityScale = 3.5f;

    public float _dashPower;

    public float _bulletSpeed;
    public float _shotDelay = 0.2f;

    public float _moveSpeed;
    public float _moveX;

    public float _jumpPower;
    public float _hitPower = 1.2f; // 피격 밀림 크기

    [Header("#Bool Parameter")]
    public bool _isFlipX = false; // 스프라이트 뒤집혀짐 여부
    public bool _isShot = false; // 사격 여부
    public bool _isJump = false; // 점프 여부
    public bool _isCrouch = false; // 앉기키 누름 여부
    public bool _isHit = false; // 피격 여부
    public bool _canClimb = false;
    public bool _isDash = false;
    public bool _isClick = false;
    public bool _isPause = false;

    // Sound
    public ObjectSound _sound;

    void Awake()
    {
        _Hp = _maxHp;
        _gravityScale = 3.5f;

        _inputActions = new PlayerInput_();
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        _sound = GetComponent<ObjectSound>();

        _inputActions.Player.Enable();
        _inputActions.Player.PlayerClick.performed += OnClick;
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        _inputActions.Player.Jump.performed += OnJump;
        _inputActions.Player.Jump.canceled += OnJump;
        _inputActions.Player.Fire.performed += OnFire;
        _inputActions.Player.Fire.canceled += OnFire;
        _inputActions.Player.Crouch.performed += OnCrouch;
        _inputActions.Player.Crouch.canceled += OnCrouch;
        _inputActions.Player.Dash.performed += OnDash;
        _inputActions.Player.Dash.canceled += OnDash;
        _inputActions.Player.Menu.performed += OnMenu;
    }

    void Start()
    {
        _sound._audioSource[5].loop = true;
        _sound._audioSource[5].Play();

        if (_afterImage == null)
        {
            GameObject.Find("AfterEffect");
            if (_afterImage == null)
                Debug.LogError($"NEED _afterImage perfab object");
        }
        if (_playerHealthUI == null)
        {
            GameObject.Find("PlayerHealthUI");
            if (_playerHealthUI == null)
                Debug.LogError($"NEED _changeHpUI perfab object");
        }

        _dashAction += () => StartCoroutine(_afterImage.GetComponent<Player_Afterimage>().CreateAfterImage());
        _changeHpUI += () => _playerHealthUI.GetComponent<PlayerHealth>().ChangeHealth();
        _isMeetBoss += () => GameManager.instance.pManager.ShowBossHp();
    }

    void OnDisable()
    {
        _inputActions.Player.Menu.performed -= OnMenu;
        _inputActions.Player.Dash.canceled -= OnDash;
        _inputActions.Player.Dash.performed -= OnDash;
        _inputActions.Player.Crouch.canceled -= OnCrouch;
        _inputActions.Player.Crouch.performed -= OnCrouch;
        _inputActions.Player.Fire.canceled -= OnFire;
        _inputActions.Player.Fire.performed -= OnFire;
        _inputActions.Player.Jump.canceled -= OnJump;
        _inputActions.Player.Jump.performed -= OnJump;
        _inputActions.Player.Move.canceled -= OnMove;
        _inputActions.Player.Move.performed -= OnMove;
        _inputActions.Player.PlayerClick.performed -= OnClick;
        _inputActions.Player.Disable();
    }

    void Update()
    {
        transform.Translate(Time.deltaTime * _inputMove * _moveSpeed);

        _animator.SetBool(isJump_String, _isJump);
        _animator.SetFloat(speed_String, Mathf.Abs(_inputMove.x));

        CheckJump();

        if (_inputMove.x != 0)
        {
            _isFlipX = _inputMove.x < 0;
            _sprite.flipX = _isFlipX;
        }

        if(_isCrouch)
        {
            _inputMove = Vector2.zero;
        }

        if(!_canClimb)
        {
            _inputMove.y = 0f;
        }

        if(_isDash)
        {
            transform.Translate(Time.deltaTime * _lastInput * _dashPower * _moveSpeed);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // check enemy attack
        if((collision.gameObject.CompareTag("EnemyBullet") || collision.gameObject.CompareTag("Enemy")) && !_isHit)
        {
            Vector3 _hitDir = transform.position - collision.transform.position;
            _rigid.AddForce(_hitDir * _hitPower, ForceMode2D.Impulse);

            StartCoroutine(Hit_Corutine());
        }
        if(collision.gameObject.CompareTag("Platform"))
        {
            _sound._audioSource[3].Play();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("CheckPoint"))
        {
            _Hp = _maxHp;
        }

        if (collision.gameObject.CompareTag("Ladder"))
        {
            if (!_canClimb)
            {
                _animator.SetBool(isClimb_String, true);
            }
            _canClimb = true;
        }

        if(collision.gameObject.CompareTag("StageEnd") && !GameManager.instance.pManager._isBossAlive)
        {
            GameManager.instance.pManager.VictoryPanel.SetActive(true);
        }

        if(collision.gameObject.name == "EnterBossRoom")
        {
            GameManager.instance.pManager._isMeetBoss = true;

            _inputMove = Vector2.zero;

            StopAllSound();

            collision.gameObject.SetActive(false);
            _isMeetBoss?.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            _canClimb = false;
            _animator.SetBool(isClimb_String, false);
            _rigid.gravityScale = _gravityScale;
        }
    }

    private void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isCrouch = true;

            _inputMove.x = 0;

            if (_isFlipX)
            {
                _bulletPos.transform.localPosition = new Vector3(-1.9f, -0.7f, 0);
            }
            else
            {
                _bulletPos.transform.localPosition = new Vector3(1.9f, -0.7f, 0);
            }
        }

        if(context.canceled)
        {
            _isCrouch = false;

            if (_isFlipX)
            {
                _bulletPos.transform.localPosition = new Vector3(-1.9f, 0.3f, 0);
            }
            else
            {
                _bulletPos.transform.localPosition = new Vector3(1.9f, 0.3f, 0);
            }
        }

        _animator.SetBool(isCrouch_String, _isCrouch);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (_isShot || _isCrouch || _canClimb)
            return;

        if(context.performed && !_isJump)
        {
            _sound._audioSource[1].Stop();
            _sound._audioSource[2].Play();

            _rigid.AddForce(Vector3.up * _jumpPower, ForceMode2D.Impulse);
            _isJump = true;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (_isDash || _isPause)
            return;

        _inputMove = context.ReadValue<Vector2>();
        _inputMove.y = 0f;

        if (context.performed)
        {
            if(!_isJump && !_isCrouch)
            {
                _sound._audioSource[1].Play();
                _sound._audioSource[1].loop = true;
            }

            /// flip Sprite
            if(_inputMove.x < 0)
            {
                if (_isCrouch)
                {
                    _bulletPos.transform.localPosition = new Vector3(-1.9f, -0.7f, 0);
                }
                else
                {
                    _bulletPos.transform.localPosition = new Vector3(-1.9f, 0.3f, 0);
                }
            }
            else
            {
                if (_isCrouch)
                {
                    _bulletPos.transform.localPosition = new Vector3(1.9f, -0.7f, 0);
                }
                else
                {
                    _bulletPos.transform.localPosition = new Vector3(1.9f, 0.3f, 0);
                }
            }

            if(_canClimb)
            {
                _inputMove.y = context.ReadValue<Vector2>().y;

                _animator.SetTrigger("Climb");

                if (MathF.Abs(_inputMove.y) > 0)
                {
                    _rigid.gravityScale = 0f;
                }
            }
        } // performed

        if(context.canceled)
        {
            _sound._audioSource[1].Stop();
        }
    }
    void CheckJump()
    {
        _ray = Physics2D.Raycast(transform.position, Vector3.down, _distance, LayerMask.GetMask("Platform"));
        Debug.DrawRay(transform.position, Vector3.down * _distance, Color.green);
        if (_ray.collider != null && _ray.distance < _distanceray)
        {
            _isJump = false;
        }
        else if (_ray.collider == null && !_canClimb)
        {
            _isJump = true;
        }
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (_isJump || _isShot || _canClimb || _isPause || GameManager.instance.pManager._isActiveDialog)
            return;

        if (context.performed)
        {
            _sound._audioSource[0].Play();

            StartCoroutine(Shot_Corutine());

            if (_inputMove.x != 0 || _isCrouch)
            {
                _animator.SetBool(isRunningShot_string, true);
            }
            else
            {
                _animator.SetTrigger(isShot_string);
            }
        }

        if(context.canceled)
        {
            if (_inputMove.x != 0 || _isCrouch)
            {
                //_animator.SetTrigger(isRunningShot_string);
            }
            else
            {
                _animator.SetTrigger(isShot_string);
            }
            
        }
    }

    private void OnDash(InputAction.CallbackContext context) // Player Dash
    {
        if (_inputMove == Vector2.zero)
            return;

        if(context.performed)
        {
            if (_isDash)
                return;

            _lastInput = _inputMove;

            StartCoroutine(Dash_Corutine());
            _isDash = true;
            _dashSpot = transform.position;

            _dashAction?.Invoke();
        }
    }

    private void OnMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(GameManager.instance.pManager.isPauseActive())
            {
                GameManager.instance.pManager.PauseActive(false);   
                _isPause = false;
                Time.timeScale = 1f;
            }
            else
            {
                GameManager.instance.pManager.PauseActive(true);
                _isPause = true;
                Time.timeScale = 0f;
            }
        }
    }

    private void OnClick(InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            _isClick = true;
        }
    }

    /// <summary>
    /// 대쉬 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator Dash_Corutine()
    {
        this.gameObject.layer = 11;
        _lastSprite = _sprite.sprite;
        _isDash = true;
        _isHit = true;

        _sound._audioSource[4].Play();

        yield return new WaitForSeconds(_dashDelay);
        this.gameObject.layer = 8;
        _isDash = false;
        _isHit = false;
        _inputMove = Vector2.zero;
        if(_isFlipX)
        {
            _inputMove.x = -_animator.GetFloat(speed_String);
        }
        else
        {
            _inputMove.x = _animator.GetFloat(speed_String);
        }
    }

    /// <summary>
    /// 공격 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator Shot_Corutine()
    {
        GameObject _bulletObj = Instantiate(_bullet, _bulletPos.transform.position, Quaternion.identity);
        Bullet_Player _bulletPlayer = _bulletObj.AddComponent<Bullet_Player>();
        _bulletPlayer._speed = _bulletSpeed;
        _bulletPlayer._hitEffect = _bulletHitEffect;
        _bulletPlayer._player = this;
        _bulletPlayer.tag = "PlayerBullet";

        _isShot = true;
        yield return new WaitForSeconds(_shotDelay);
        _isShot = false;

        _animator.SetBool(isRunningShot_string, false);
    }

    /// <summary>
    /// 피격 코루틴 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator Hit_Corutine()
    {
        _sprite.color = new Color(0.5f, 0.5f, 0.5f, 1); // 피격 이펙트

        _isHit = true;
        _animator.SetTrigger(isHit_String);

        _Hp--;
    
        yield return new WaitForSeconds(1f);

        _sprite.color = Color.white;
        _isHit = false;
    }

    public void AddScore(int getScore)
    {
        _score += getScore;
    }

    void Die()
    {
        GameManager.instance.pManager.DefeatPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void StopAllSound()
    {
        for (int i = 0; i < _sound._audioClips.Length; i++)
        {
            _sound._audioSource[i].Stop();
        }
    }
}
