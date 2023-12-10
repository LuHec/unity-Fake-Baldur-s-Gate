using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeDebugPanel : UIPanelBase
{
    [SerializeField] private ScrollRect logRect;
    [SerializeField] private TMP_Text logText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button rcvInput;

    private string inputBuffer;

    protected override void Init()
    {
        rcvInput.onClick.AddListener(() =>
        {
            inputBuffer = inputField.text;
            CommandInvoker.InvokeCommand(typeof(Script1), inputBuffer, null);
        });
    }

    public void UpdateLog(string text)
    {
        logText.text = text;
        logRect.verticalNormalizedPosition = 0f;
    }
}