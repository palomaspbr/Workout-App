using TMPro;
using UnityEngine;

public class LoadExercise : MonoBehaviour
{
    private UI_Logic _UI_Logic;

    [SerializeField] private GameObject[] SeriesUI;
    [SerializeField] private TMP_Text _Exercise_Title;
    [SerializeField] private TMP_Text _Exercise_Name;

    private void Start()
    {
        _UI_Logic = FindObjectOfType<UI_Logic>();
        _UI_Logic.OnLoadExercise += _UI_Logic_OnLoadExercise1;
    }

    private void OnDestroy()
    {
        _UI_Logic.OnLoadExercise -= _UI_Logic_OnLoadExercise1;
    }

    private void _UI_Logic_OnLoadExercise1(object sender, ExerciseParameters e)
    {
        Debug.Log($"[LoadExercise] Loaded exercise {e.ExerciseName}, with {e.ExerciseSeries} series and {e.ExercisesPerSeries} respetitions per serie.");

        int i = 0;
        foreach(var item in SeriesUI)
        {
            item.GetComponent<ChangeButtonColor>().ResetColor();
            if (i <= e.ExerciseSeries -1)
            {
                item.SetActive(true);
                item.GetComponentInChildren<TMP_Text>().text = e.ExercisesPerSeries.ToString() + " (" + e.Load.ToString() + "kg)";
            }
            else
            {
                item.SetActive(false);
            }
            i++;
        }

        int index = e.ExerciseIndex + 1;
        _Exercise_Title.text = "Exercício " + index;
        _Exercise_Name.text = e.ExerciseName;

    }
}
