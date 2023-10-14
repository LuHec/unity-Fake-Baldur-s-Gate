using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private MessageCenter _messageCenter;
    
    public Button switchState;
    private TMP_Text _text_button;

    public void Start()
    {
        _text_button = switchState.GetComponentInChildren<TMP_Text>();
        switchState.onClick.AddListener(OnButtonClick);
        _messageCenter = MessageCenter.Instance;
    }

    public void OnButtonClick()
    {
        _messageCenter.globalState.SetTurnMode(!_messageCenter.globalState.TurnMode);
        if (_messageCenter.globalState.TurnMode)
            _text_button.text = "SwitchTo3rd";
        else 
            _text_button.text = "SwitchToTurnMode";
    }
}