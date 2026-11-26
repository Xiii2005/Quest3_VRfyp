using UnityEngine;
using TMPro;

public class CountdownDisplay : MonoBehaviour
{
    public TMP_Text timerText;
    public float timeRemaining = 86400f; // 24 * 60 * 60
    public bool isTimerRunning = true;

    void Update()
    {
        if (!isTimerRunning) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateDisplay(timeRemaining);
        }
        else
        {
            timeRemaining = 0;
            isTimerRunning = false;
            UpdateDisplay(timeRemaining);
        }
    }

    private void UpdateDisplay(float time)
    {
        float hours = Mathf.FloorToInt(time / 3600f);
        float minutes = Mathf.FloorToInt((time % 3600f) / 60f);
        float seconds = Mathf.FloorToInt(time % 60f);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}