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

    private bool isShow;

    private string inputBuffer;
    private Queue<DebugStringBuffer> logQueue = new Queue<DebugStringBuffer>();
    private StringBuilder logStringBuilder = new StringBuilder(80 * 15);
    private int screenTxtCounter;

    // 计时更新log
    private float timer;

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

            UIPanelManager.Instance.HidePanel<RuntimeDebugPanel>();
        }

        else
        {
            // 暂停游戏
            if (pause)
                Time.timeScale = 0;

            isShow = true;

            UIPanelManager.Instance.ShowPanel<RuntimeDebugPanel>();

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

    public void LogMessage(string message)
    {
        if (logQueue.Count > queueTxtMaxSize)
            logQueue.Dequeue();

        var debugStringBuffer = new DebugStringBuffer(message);
        logQueue.Enqueue(debugStringBuffer);

        logStringBuilder.Append('\n');
        logStringBuilder.Append(debugStringBuffer);
    }


    private void UpdateLog()
    {
        UIPanelManager.Instance.GetPanel<RuntimeDebugPanel>().UpdateLog(logStringBuilder.ToString());
    }
}

// sbuffer为缓冲区，每次print缓冲区复制到输出窗口
// sbuffer