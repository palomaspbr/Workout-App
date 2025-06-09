using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonColor : MonoBehaviour
{
    private bool _isGreen = false;
    public void ToggleColor()
    {
        if (_isGreen)
        {
            string color = "#FFFFFF";
            Button button = GetComponent<Button>();
            Color newcolor;
            if (ColorUtility.TryParseHtmlString(color, out newcolor))
            {
                button.image.color = newcolor;
            }
            _isGreen = false;
        }
        else
        {
            string color = "#7CF6BA";
            Button button = GetComponent<Button>();
            Color newcolor;
            if (ColorUtility.TryParseHtmlString(color, out newcolor))
            {
                button.image.color = newcolor;
            }
            _isGreen = true;
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
        _isGreen = false;
    }
}
