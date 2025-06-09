using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private UI_Logic _UI_Logic;

    [SerializeField] private TMP_Text _Timer_Text;
    [SerializeField] private Image _Timer_Image;
    public event EventHandler<bool> OnFinishTimer = delegate { };

    private void Start()
    {
        _UI_Logic = FindObjectOfType<UI_Logic>();
        _UI_Logic.OnStartTimer += _UI_Logic_OnStartTimer1;
    }

    private void OnDestroy()
    {
        _UI_Logic.OnStartTimer -= _UI_Logic_OnStartTimer1;
    }

    private void _UI_Logic_OnStartTimer1(object sender, int seconds)
    {
        Debug.Log($"[Timer] Started timer of {seconds} seconds.");
        StartCoroutine(CO_Timer(seconds));
    }

    IEnumerator CO_Timer(int seconds)
    {
        _Timer_Text.text = "00:00";
        float time = Time.time;
        while (Time.time - time <= seconds)
        {
            int min = Mathf.FloorToInt((Time.time - time) / 60);
            int sec = Mathf.FloorToInt((Time.time - time) % 60);
            _Timer_Text.text = string.Format("{00:00}:{1:00}", min, sec);
            _Timer_Image.fillAmount = Mathf.Min(1, (Time.time - time)/seconds);
            yield return null;
        }
        OnFinishTimer(this, true);
    }
}
