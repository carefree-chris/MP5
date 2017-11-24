using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class SliderWithEcho : MonoBehaviour
{
    public Slider mSlider;
    public Text mEcho;

    public void InitSliderRange(float v1, float v2, float x)
    {
        mSlider.minValue = v1;
        mSlider.maxValue = v2;
        mSlider.value = x;
        mSlider.enabled = true;
    }

    public void SetSliderListener(UnityAction<float> xValueChanged)
    {
        mSlider.onValueChanged.AddListener(xValueChanged);
        mSlider.onValueChanged.AddListener(EchoValue);
    }

    private void EchoValue(float arg0)
    {
        mEcho.text = arg0.ToString();
    }

    public void SetSliderValue(float x)
    {
        mSlider.value = x;
    }

    internal void SetInactive()
    {
        mSlider.enabled = false;
    }
}