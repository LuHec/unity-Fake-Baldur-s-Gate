using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPanel : UIPanelBase
{
    private MessageCenter messageCenter;

    [SerializeField] private Button switchState;
    [SerializeField] private LayoutGroup playerLayoutGroup;
    [SerializeField] private Button backTurn;

    private TMP_Text textButton;

    // 切换模式
    private bool turnMode = false;

    #region #sender

    private event EventHandler<EventArgsType.GameModeSwitchMessage> GameModeHandler;
    private event EventHandler<EventArgsType.PlayerSelectMessage> PlayerSelectHandler;
    private event EventHandler<EventArgsType.PlayerBackTurnMessage> PlayerBackTurnHandler;

    #endregion


    // 按钮切换，通知事件中心
    // 事件中心通知TurnManager
    protected override void Init()
    {
        textButton = switchState.GetComponentInChildren<TMP_Text>();
        switchState.onClick.AddListener(OnGameModeSwitchButtonClick);
        messageCenter = MessageCenter.Instance;

        AddListener();
    }

    /// <summary>
    /// 订阅事件中心
    /// </summary>
    void AddListener()
    {
        messageCenter.ListenOnGameModeSwitch(ref GameModeHandler);
        messageCenter.ListenOnPlayerSelect(ref PlayerSelectHandler);
        messageCenter.ListenOnPlayerBackTurn(ref PlayerBackTurnHandler);

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