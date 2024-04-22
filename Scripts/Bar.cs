using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void UpdateBar(float currentValue, float maxValue)
    {
        if (slider != null)
        {
            if (maxValue == 0)
            {
                Debug.Log("Setting slider value to 1 because maxValue is 0.");
                slider.value = 1f;
            }
            else
            {
                slider.value = currentValue / maxValue;
            }
        }
        else
        {
            Debug.LogWarning("Slider reference is null.");
        }
    }
}
