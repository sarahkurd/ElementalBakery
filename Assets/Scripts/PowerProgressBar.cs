using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerProgressBar : MonoBehaviour
{
    public float currentPowerValue;
    public float maxPowerValue;
    public Image mask;

    public void SetPowerValue(float value, float maxValue)
    {
        float scaleX = value / maxValue;
        mask.transform.localScale = new Vector3(scaleX, 1, 1);
    }
}
