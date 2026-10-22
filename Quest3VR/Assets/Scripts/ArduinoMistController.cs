using System;
using UnityEngine;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using System.IO.Ports;
#endif

public class ArduinoMistController : MonoBehaviour
{
    [Header("Serial Settings")]
    [SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 115200;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    private SerialPort serialPort;
    private string receiveBuffer = "";
    private float serialReadyTime;
#endif

    private void Start()
    {
        OpenSerialPort();
    }

    private void Update()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        ReadArduinoMessages();
#endif
    }

    private void OpenSerialPort()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        try
        {
            serialPort = new SerialPort(portName, baudRate)
            {
                NewLine = "\n",
                ReadTimeout = 20,
                WriteTimeout = 100,
                DtrEnable = true
            };

            serialPort.Open();

            // Nano打开串口时通常会重启，等待其初始化完成
            serialReadyTime = Time.realtimeSinceStartup + 2f;

            Debug.Log($"Arduino connected: {portName}");
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Cannot open Arduino on {portName}: " +
                exception.Message
            );
        }
#else
        Debug.LogWarning(
            "Direct COM control is only enabled in Windows Editor/Windows build."
        );
#endif
    }

    public void DiffuseForFiveSeconds()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        if (serialPort == null || !serialPort.IsOpen)
        {
            Debug.LogError("Arduino serial port is not connected.");
            return;
        }

        if (Time.realtimeSinceStartup < serialReadyTime)
        {
            Debug.LogWarning("Arduino is still restarting. Try again.");
            return;
        }

        try
        {
            serialPort.WriteLine("MIST_5S");
            Debug.Log("Sent to Arduino: MIST_5S");
        }
        catch (Exception exception)
        {
            Debug.LogError("Serial write failed: " + exception.Message);
        }
#endif
    }

    public void StopMist()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine("STOP");
        }
#endif
    }

    [ContextMenu("TEST: Diffuse For 5 Seconds")]
    private void TestDiffuse()
    {
        DiffuseForFiveSeconds();
    }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    private void ReadArduinoMessages()
    {
        if (serialPort == null || !serialPort.IsOpen)
            return;

        try
        {
            if (serialPort.BytesToRead <= 0)
                return;

            receiveBuffer += serialPort.ReadExisting();

            int newlineIndex;

            while ((newlineIndex = receiveBuffer.IndexOf('\n')) >= 0)
            {
                string message = receiveBuffer
                    .Substring(0, newlineIndex)
                    .Trim();

                receiveBuffer = receiveBuffer
                    .Substring(newlineIndex + 1);

                if (!string.IsNullOrEmpty(message))
                {
                    Debug.Log("Arduino: " + message);
                }
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Serial read warning: " + exception.Message);
        }
    }
#endif

    private void OnDestroy()
    {
        CloseSerialPort();
    }

    private void OnApplicationQuit()
    {
        CloseSerialPort();
    }

    private void CloseSerialPort()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        if (serialPort == null)
            return;

        try
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("STOP");
                serialPort.Close();
            }
        }
        catch
        {
            // Unity退出时不再抛出额外错误
        }

        serialPort = null;
#endif
    }
}