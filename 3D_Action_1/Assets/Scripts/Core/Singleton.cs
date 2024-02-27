using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// ����ó���� ������ Ȯ���ϴ� ����
    /// </summary>
    private static bool isShutDown = false;

    /// <summary>
    /// �̱����� ��ü
    /// </summary>
    private static T instance;

    /// <summary>
    /// �̱��� ��ü�� �б� ���� ������Ƽ
    /// </summary>
    public static T Instance
    {
        get
        {
            if(isShutDown)
            {
                Debug.LogWarning("�̱����� ���� ���Դϴ�");
                return null;
            }
            if(instance == null)
            {
                T singleton = FindAnyObjectByType<T>(); // �̱��� ������Ʈ ã��

                if(singleton == null) // ������ �ӽ� ������Ʈ ����
                {
                    GameObject obj = new GameObject();
                    obj.name = "Singleton";
                    singleton = obj.AddComponent<T>();
                }
                instance = singleton;
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    // flag

    /// <summary>
    /// �̱����� �ʱ�ȭ �Ǿ��ִ��� Ȯ���ϴ� bool
    /// </summary>
    bool isInitionalize = false;

    void Awake()
    {
        if(instance == null) // ���� ��ġ�� �ٸ� �̱����� ���� ��
        {
            instance = this as T;
            DontDestroyOnLoad(instance.gameObject);
        }
        else if(instance != null) // ���� �̱����� ��ġ�� ������
        {
            Destroy(this.gameObject); // �ڽ� �ı�
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// �� �ε�ɶ� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="scene">�� ����</param>
    /// <param name="mode">�ε� ���</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(!isInitionalize)
        {
            OnPreInitialize();
        }
        if(mode != LoadSceneMode.Additive)
        {
            OnInitialize();
        }
    }

    /// <summary>
    /// ���� �ε�ǰ� �ѹ� ����� �Լ�
    /// </summary>
    protected virtual void OnPreInitialize()
    {
        isInitionalize = true;
    }

    /// <summary>
    /// �̱����� ��������� ���� ����� �� ���� ����� �Լ�
    /// </summary>
    protected virtual void OnInitialize()
    {
    }

    /// <summary>
    /// ���α׷� ����� �� ����� �Լ�
    /// </summary>
    private void OnApplicationQuit()
    {
        isShutDown = true;
    }
}
