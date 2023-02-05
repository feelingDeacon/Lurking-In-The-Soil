using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private float _maxVal;
    private float _currVal;

    public void Init(float maxValue, float currValue)
    {
        _maxVal = maxValue;
        UpdateHealthBar(currValue);
    }
    
    public void UpdateHealthBar(float newVal)
    {
        _currVal = newVal;
        slider.value = _currVal / _maxVal;
    }
}
