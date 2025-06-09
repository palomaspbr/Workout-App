using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExerciseParameters
{
    public int exerciseIndex;
    public string exerciseName;
    public int exerciseSeries;
    public int exercisesPerSeries;

    public int restLocal;
    public int restGlobal;
}

public class UI_Logic : MonoBehaviour
{
    [Header("Panel Start")]
    [SerializeField] private GameObject _Panel_Start;

    [Header("Panel Input Exercises")]
    [SerializeField] private GameObject _Panel_InputExercises;
    [SerializeField] private TMP_InputField _InputField_Exercises;

    [Header("Panel Input Exercise")]
    [SerializeField] private GameObject _Panel_InputExercise;
    [SerializeField] private TMP_InputField _InputField_EX;
    [SerializeField] private TMP_InputField _InputField_EX_Amount;
    [SerializeField] private TMP_InputField _InputField_EX_Amount_PerSerie;
    [SerializeField] private TMP_InputField _InputField_EX_RestLocal;
    [SerializeField] private TMP_InputField _InputField_EX_RestGlobal;

    [Header("Panel Exercise")]
    [SerializeField] private GameObject _Panel_Exercise;

    [Header("Panel Timer")]
    [SerializeField] private GameObject _Panel_Timer;
    [SerializeField] private Timer _Timer;


    public event EventHandler<ExerciseParameters> OnLoadExercise = delegate { };
    public event EventHandler<int> OnStartTimer = delegate { };

    private int _amountOfExercises;
    private int _currentExerciseInput;
    private int _currentExercise;

    public Dictionary<int, ExerciseParameters> Exercises = new();

    private void Start()
    {
        _Timer.OnFinishTimer += _Timer_OnFinishTimer;
    }

    private void OnDestroy()
    {
        _Timer.OnFinishTimer -= _Timer_OnFinishTimer;
    }

    public void StartUI()
    {
        _Panel_Start?.SetActive(false);
        _Panel_InputExercises?.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void SetAmoutOfExercises()
    {
        int.TryParse(_InputField_Exercises.text, out _amountOfExercises);
        _Panel_InputExercises?.SetActive(false);
        _Panel_InputExercise.SetActive(true);
        _currentExerciseInput = 1;
    }

    public void InputExercise()
    {
        ExerciseParameters exercise = new ExerciseParameters();
        // Get information from exercise 
        exercise.exerciseIndex = _currentExerciseInput;
        exercise.exerciseName = _InputField_EX.text;
        int.TryParse(_InputField_EX_Amount.text, out exercise.exerciseSeries);
        int.TryParse(_InputField_EX_Amount_PerSerie.text, out exercise.exercisesPerSeries);
        int.TryParse(_InputField_EX_RestLocal.text, out exercise.restLocal);
        int.TryParse(_InputField_EX_RestGlobal.text, out exercise.restGlobal);

        Exercises.Add(_currentExerciseInput, exercise);

        // Start training
        _currentExerciseInput += 1;

        _InputField_EX.text = null;
        _InputField_EX_Amount.text = null;
        _InputField_EX_Amount_PerSerie.text = null;
        _InputField_EX_RestLocal.text = null;
        _InputField_EX_RestGlobal.text = null;

        if (_currentExerciseInput > _amountOfExercises)
        {
            _currentExerciseInput = 0;
            _Panel_InputExercise?.SetActive(false);
            _Panel_Exercise?.SetActive(true);
            _currentExercise = 0;
            StartCoroutine(CO_LoadNextExercise());
        }

    }

    public void NextExercise()
    {
        StartCoroutine(CO_LoadNextExercise());
    }

    IEnumerator CO_LoadNextExercise()
    {
        yield return null;
        _currentExercise += 1;
        if (_currentExercise > _amountOfExercises)
        {
            // Finish training
            Restart();
            yield break;
        }
        OnLoadExercise(this, Exercises[_currentExercise]);
    }

    public void StartTimerLocal()
    {
        StartCoroutine(CO_StartTimer(true));
    }

    public void StartTimerGlobal()
    {
        StartCoroutine(CO_StartTimer(false));
        OnStartTimer(this, Exercises[_currentExercise].restGlobal);
    }

    IEnumerator CO_StartTimer(bool isLocal)
    {
        _Panel_Timer?.SetActive(true);
        _Panel_Exercise?.SetActive(false);
        yield return null;
        if (isLocal) {
            OnStartTimer(this, Exercises[_currentExercise].restLocal);
        }
        else {
            OnStartTimer(this, Exercises[_currentExercise].restGlobal);
        }
    }

    private void _Timer_OnFinishTimer(object sender, bool b)
    {
        _Panel_Timer?.SetActive(false);
        _Panel_Exercise?.SetActive(true);

    }

}
