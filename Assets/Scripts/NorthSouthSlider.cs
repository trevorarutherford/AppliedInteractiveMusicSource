using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NorthSouthSlider : MonoBehaviour
{
    public AK.Wwise.RTPC quadNorthSouth;
    private float lastSliderVal = .5f;

    private void Update()
    {
        float sliderVal = GetComponent<Slider>().value;
        if(lastSliderVal != sliderVal) SetRTPC(sliderVal);
    }

    public void SetRTPC(float sliderVal)
    {
        quadNorthSouth.SetGlobalValue(sliderVal);
    }


}
