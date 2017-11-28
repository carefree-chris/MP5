using UnityEngine;

public class ResolutionControl : MonoBehaviour {

    public SliderWithEcho NResolutionSlider;
    public SliderWithEcho MResolutionSlider;
    public MyMesh mMesh;
    public MainControl mMainControl;

	// Use this for initialization
	void Start () {
        InitSliders();
    }

    void InitSliders()
    {
        NResolutionSlider.InitSliderRange(2.0f, 20.0f, mMesh.N);
        MResolutionSlider.InitSliderRange(2.0f, 20.0f, mMesh.M);

        NResolutionSlider.SetSliderListener(NResolutionChanged);
        MResolutionSlider.SetSliderListener(MResolutionChanged);
    }

    void NResolutionChanged(float n)
    {
        mMesh.SetResolution((int)n, mMesh.M);
        mMainControl.Unselect();
    }

    void MResolutionChanged(float m)
    {
        mMesh.SetResolution(mMesh.N, (int)m);
        mMainControl.Unselect();
    }
}
