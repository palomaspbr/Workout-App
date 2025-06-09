using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonColor : MonoBehaviour
{
    public void ChangeColor()
    {
        string color = "#7CF6BA";
        Button button = GetComponent<Button>();
        Color newcolor;
        if (ColorUtility.TryParseHtmlString(color, out newcolor))
        {
            button.image.color = newcolor;
        }
    }

    public void ResetColor()
    {
        string color = "#FFFFFF";
        Button button = GetComponent<Button>();
        Color newcolor;
        if (ColorUtility.TryParseHtmlString(color, out newcolor))
        {
            button.image.color = newcolor;
        }
    }
}
