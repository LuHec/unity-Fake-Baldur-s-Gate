using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private MessageCenter _messageCenter;

    [SerializeField] private Button switchState;
    [SerializeField] private LayoutGroup playerLayoutGroup;
    [SerializeField] private Button backTurn;

    private TMP_Text _text_button;

    // 切换模式
    private bool turnMode = false;

    #region #sender

    private event EventHandler<EventArgsType.GameModeSwitchMessage> GameModeHandler;
    private event EventHandler<EventArgsType.PlayerSelectMessage> PlayerSelectHandler;
    private event EventHandler<EventArgsType.PlayerBackTurnMessage> PlayerBackTurnHandler; 

    #endregion

    public void Start()
    {
        _text_button = switchState.GetComponentInChildren<TMP_Text>();
        switchState.onClick.AddListener(OnGameModeSwitchButtonClick);
        _messageCenter = MessageCenter.Instance;

        AddListener();
    }

    // 按钮切换，通知事件中心
    // 事件中心通知TurnManager

    /// <summary>
    /// 订阅事件中心
    /// </summary>
    void AddListener()
    {
        _messageCenter.ListenOnGameModeSwitch(ref GameModeHandler);
        _messageCenter.ListenOnPlayerSelect(ref PlayerSelectHandler);
        _messageCenter.ListenOnPlayerBackTurn(ref PlayerBackTurnHandler);

        int childCnt = playerLayoutGroup.transform.childCount;
        for (int i = 0; i < childCnt; i++)
        {
            int idx = i;
            playerLayoutGroup.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayerSelectHandler?.Invoke(this, new EventArgsType.PlayerSelectMessage(idx));
            });
        }

        backTurn.onClick.AddListener(() =>
        {
            PlayerBackTurnHandler?.Invoke(this, new EventArgsType.PlayerBackTurnMessage());
        });
    }

    /// <summary>
    /// 切换游戏模式
    /// </summary>
    public void OnGameModeSwitchButtonClick()
    {
        if (turnMode)
        {
            turnMode = false;
            GameModeHandler?.Invoke(this,
                new EventArgsType.GameModeSwitchMessage(EventArgsType.GameModeSwitchMessage.GameMode._3RD));
        }
        else
        {
            turnMode = true;
            GameModeHandler?.Invoke(this,
                new EventArgsType.GameModeSwitchMessage(EventArgsType.GameModeSwitchMessage.GameMode.Turn));
        }
    }
}