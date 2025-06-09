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
        Debug.Log($"[LoadExercise] Loaded exercise {e.exerciseName}, with {e.exerciseSeries} series and {e.exercisesPerSeries} respetitions per serie.");

        int i = 0;
        foreach(var item in SeriesUI)
        {
            if (i <= e.exerciseSeries -1)
            {
                item.SetActive(true);
                item.GetComponentInChildren<TMP_Text>().text = e.exercisesPerSeries.ToString();
                item.GetComponent<ChangeButtonColor>().ResetColor();
            }
            else
            {
                item.SetActive(false);
                item.GetComponent<ChangeButtonColor>().ResetColor();
            }
            i++;
        }

        _Exercise_Title.text = "Exercício " + e.exerciseIndex;
        _Exercise_Name.text = e.exerciseName;

    }
}
