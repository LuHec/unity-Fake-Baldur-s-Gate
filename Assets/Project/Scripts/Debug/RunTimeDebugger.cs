using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RunTimeDebugger : Singleton<RunTimeDebugger>
{
    [SerializeField] private int queueTxtMaxSize = 10;
    [SerializeField] private int screeTxtMaxSize = 50;
    [SerializeField] private CanvasGroup debugCanvasGroup;
    [SerializeField] private ScrollRect logRect;
    [SerializeField] private TMP_Text logText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button rcvInput;
    private bool isShow;

    private string inputBuffer;
    private Queue<DebugStringBuffer> logQueue = new Queue<DebugStringBuffer>();
    private StringBuilder logStringBuilder = new StringBuilder(80 * 15);
    private int screenTxtCounter;

    private void Start()
    {
        rcvInput.onClick.AddListener(() =>
        {
            inputBuffer = inputField.text;
            CommandInvoker.InvokeCommand(typeof(Script1), inputBuffer, null);
        });
    }

    private void Update()
    {
        if (PlayerInput.Instance.DebugPress)
        {
            isShow = !isShow;
            ShowDebug(isShow);
        }
    }

    public void ShowDebug(bool show)
    {
        if (!show)
        {
            Time.timeScale = 1;
            isShow = false;

            debugCanvasGroup.alpha = 0;
            debugCanvasGroup.interactable = false;
            debugCanvasGroup.blocksRaycasts = false;
        }

        else
        {
            // 暂停游戏
            Time.timeScale = 0;
            
            isShow = true;

            debugCanvasGroup.alpha = 1;
            debugCanvasGroup.interactable = true;
            debugCanvasGroup.blocksRaycasts = true;
        }
    }

    public void Flush()
    {
        FlushQueueTxt();
        FlushScreenTxt();
    }

    public void FlushScreenTxt()
    {
        logStringBuilder.Clear();
    }

    public void FlushQueueTxt()
    {
        logQueue.Clear();
    }

    public void LogMessage(string message)
    {
        if (logQueue.Count > queueTxtMaxSize)
            logQueue.Dequeue();

        var debugStringBuffer = new DebugStringBuffer(message);
        logQueue.Enqueue(debugStringBuffer);

        logStringBuilder.Append('\n');
        logStringBuilder.Append(debugStringBuffer);


        UpdateLog();
    }

    public void SetColor()
    {
    }

    private void UpdateLog()
    {
        logText.text = logStringBuilder.ToString();
        logRect.verticalNormalizedPosition = 0f;
    }
}

// sbuffer为缓冲区，每次print缓冲区复制到输出窗口
// sbuffer