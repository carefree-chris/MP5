using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationControl : MonoBehaviour {

    public SliderWithEcho RotationSlider;
    public MainControl mMainControl;
    public MyCylinderMesh mCylinder;

    // Use this for initialization
    void Start()
    {
        RotationSlider.InitSliderRange(10f, 360f, 275f);
        
        RotationSlider.SetSliderListener(RotationChanged);
    }

    void RotationChanged(float n)
    {
        //mMesh.SetResolution((int)n, mMesh.M);
        //mMainControl.Unselect();
        if (!mCylinder.SetDegrees(n))
        {
            Debug.Log("ERROR: Cannot set degrees to " + n);
        }

    }
}
