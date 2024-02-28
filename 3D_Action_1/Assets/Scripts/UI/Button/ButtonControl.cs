using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ButtonControl : MonoBehaviour
{
    public enum ButtonEvent // ��ư�̺�Ʈ ����
    {
        Start,
        End
    }
    public ButtonEvent buttonEvent;

    Button button;
    GameObject UImanager;

    void Start()
    {
        button = gameObject.GetComponent<Button>();
        UImanager = FindAnyObjectByType<GameUIManager>().gameObject;
        GetClickEvent(buttonEvent);
    }

    void OnEnable()
    {
        if(UImanager != null)
            GetClickEvent(buttonEvent);
    }

    /// <summary>
    /// Ŭ�� �̺�Ʈ�� ����ϴ� �Լ�
    /// </summary>
    void GetClickEvent(ButtonEvent eventName)
    {
        switch (eventName)
        {
            case ButtonEvent.Start:
                button.onClick.AddListener(UImanager.GetComponent<GameUIManager>().StartGame);
                break;
            case ButtonEvent.End:
                button.onClick.AddListener(UImanager.GetComponent<GameUIManager>().EndGame);
                break;
        }
    }
}