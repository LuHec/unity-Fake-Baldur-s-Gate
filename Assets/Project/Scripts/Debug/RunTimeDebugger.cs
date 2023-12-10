using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RunTimeDebugger : Singleton<RunTimeDebugger>
{
    [SerializeField] private bool pause = false;
    [SerializeField] private float logUpdateTime = 0.5f;
    [SerializeField] private int queueTxtMaxSize = 10;
    [SerializeField] private int screeTxtMaxSize = 50;
    
    [Space]
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
    
    // 计时更新log
    private float timer;

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

        if (isShow)
        {
            timer += Time.deltaTime;
            if (timer > logUpdateTime)
            {
                UpdateLog();
                timer = 0;
            }
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
            if (pause)
                Time.timeScale = 0;

            isShow = true;

            debugCanvasGroup.alpha = 1;
            debugCanvasGroup.interactable = true;
            debugCanvasGroup.blocksRaycasts = true;
            
            UpdateLog();
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

    public async void LogMessage(string message)
    {
        if (logQueue.Count > queueTxtMaxSize)
            logQueue.Dequeue();

        var debugStringBuffer = new DebugStringBuffer(message);
        logQueue.Enqueue(debugStringBuffer);
        await Task.Yield();

        logStringBuilder.Append('\n');
        logStringBuilder.Append(debugStringBuffer);
    }

    public void SetColor()
    {
    }

    private async void UpdateLog()
    {
        logText.text = logStringBuilder.ToString();
        await Task.Yield();
        logRect.verticalNormalizedPosition = 0f;
    }
}

// sbuffer为缓冲区，每次print缓冲区复制到输出窗口
// sbuffer