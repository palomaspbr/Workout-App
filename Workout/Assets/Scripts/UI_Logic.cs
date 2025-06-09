using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExerciseParameters
{
    public int ExerciseIndex;
    public string ExerciseName;
    public int ExerciseSeries;
    public int ExercisesPerSeries;
    public float Load;

    public float[] Load_Series;

    public int RestLocal;
    public int RestGlobal;
}

public class Workout
{
    public ExerciseParameters[] Exercises;
}

public class UI_Logic : MonoBehaviour
{
    [Header("Panel Start")]
    [SerializeField] private GameObject _Panel_Start;

    [Header("Panel Load")]
    [SerializeField] private GameObject _Panel_Load;

    [Header("Panel Input Exercises")]
    [SerializeField] private GameObject _Panel_InputExercises;
    [SerializeField] private TMP_InputField _InputField_Exercises;

    [Header("Panel Input Exercise")]
    [SerializeField] private GameObject _Panel_InputExercise;
    [SerializeField] private TMP_InputField _InputField_EX;
    [SerializeField] private TMP_InputField _InputField_EX_Amount;
    [SerializeField] private TMP_InputField _InputField_EX_Amount_PerSerie;
    [SerializeField] private TMP_InputField _InputField_EX_Load;
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

    [SerializeField] private GameObject[] _Button_Workout;
    private Workout _workout;

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
        _workout = new Workout();
    }

    public void InputExercise()
    {
        ExerciseParameters exercise = new ExerciseParameters();
        // Get information from exercise 
        exercise.ExerciseIndex = _currentExerciseInput;
        exercise.ExerciseName = _InputField_EX.text;
        int.TryParse(_InputField_EX_Amount.text, out exercise.ExerciseSeries);
        int.TryParse(_InputField_EX_Amount_PerSerie.text, out exercise.ExercisesPerSeries);
        float.TryParse(_InputField_EX_Amount_PerSerie.text, out exercise.Load);
        int.TryParse(_InputField_EX_RestLocal.text, out exercise.RestLocal);
        int.TryParse(_InputField_EX_RestGlobal.text, out exercise.RestGlobal);

        for(int i = 0; i < exercise.ExerciseSeries; i++)
        {
            exercise.Load_Series[i] = exercise.Load;
        }

        _workout.Exercises[_currentExerciseInput] = exercise;

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

            for(int i = 0; i<10; i++)
            {
                if (Serializer.Load<Workout>("Workout" + i) == default)
                {
                    Serializer.Save("Workout" + i, _workout);
                    break;
                }
            }
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
        OnStartTimer(this, Exercises[_currentExercise].RestGlobal);
    }

    IEnumerator CO_StartTimer(bool isLocal)
    {
        _Panel_Timer?.SetActive(true);
        _Panel_Exercise?.SetActive(false);
        yield return null;
        if (isLocal) {
            OnStartTimer(this, Exercises[_currentExercise].RestLocal);
        }
        else {
            OnStartTimer(this, Exercises[_currentExercise].RestGlobal);
        }
    }

    private void _Timer_OnFinishTimer(object sender, bool b)
    {
        _Panel_Timer?.SetActive(false);
        _Panel_Exercise?.SetActive(true);

    }

    public void Load()
    {
        _Panel_Start?.SetActive(false);
        _Panel_Load?.SetActive(true);

        for (int i = 0; i < 10; i++)
        {
            if (Serializer.Load<Workout>("Workout" + i) == default)
            {
                break;
            }

            _Button_Workout[i].SetActive(true);
        }
    }

    public void LoadWorkout(int index)
    {
        string fileToLoad = "Workout" + index;
        _workout = Serializer.Load<Workout>(fileToLoad);

        _currentExerciseInput = 1;
        foreach (ExerciseParameters exercise in _workout.Exercises)
        {
            Exercises.Add(_currentExerciseInput, exercise);
            _currentExerciseInput ++;
        }

        _Panel_Load?.SetActive(false);
        _Panel_Exercise?.SetActive(true);
        _currentExercise = 0;
        StartCoroutine(CO_LoadNextExercise());
    }

}

public class Serializer
{
    public static T Load<T>(string filename) where T : class
    {
        if (File.Exists(filename))
        {
            try
            {
                using (Stream stream = File.OpenRead(filename))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return formatter.Deserialize(stream) as T;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        return default(T);
    }

    public static void Save<T>(string filename, T data) where T : class
    {
        using (Stream stream = File.OpenWrite(filename))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
        }
    }
}