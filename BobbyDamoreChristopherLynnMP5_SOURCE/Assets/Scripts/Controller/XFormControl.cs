using UnityEngine;
using UnityEngine.UI;

public class XFormControl : MonoBehaviour
{
    public Toggle T, R, S;
    public SliderWithEcho X, Y, Z;
    public Text ObjectName;

    private MyMesh mMesh;
    private Vector3[] mPreviousSliderValues = new Vector3[3];
    private bool modeChanging = false;

    // Use this for initialization
    void Start()
    {
        T.onValueChanged.AddListener(SetToTranslation);
        R.onValueChanged.AddListener(SetToRotation);
        S.onValueChanged.AddListener(SetToScaling);
        X.SetSliderListener(XValueChanged);
        Y.SetSliderListener(YValueChanged);
        Z.SetSliderListener(ZValueChanged);

        T.isOn = true;
        R.isOn = false;
        S.isOn = false;
        SetToTranslation(true);
    }

    void SetToTranslation(bool v)
    {
        if (!modeChanging)
        {
            modeChanging = true;
            T.isOn = true;
            R.isOn = false;
            S.isOn = false;
            Vector3 p = mPreviousSliderValues[0];
            X.InitSliderRange(-2, 2, p.y);
            Y.InitSliderRange(-2, 2, p.x);
            Z.InitSliderRange(-2, 2, p.z);
            Z.SetInactive();
            modeChanging = false;
        }
    }

    void SetToScaling(bool v)
    {
        if (!modeChanging)
        {
            modeChanging = true;
            T.isOn = false;
            R.isOn = false;
            S.isOn = true;
            Vector3 s = mPreviousSliderValues[1];
            X.InitSliderRange(0.1f, 20, s.y);
            Y.InitSliderRange(0.1f, 20, s.x);
            Z.InitSliderRange(0.1f, 20, s.z);
            Z.SetInactive();
            modeChanging = false;
        }
    }

    void SetToRotation(bool v)
    {
        if (!modeChanging)
        {
            modeChanging = true;
            T.isOn = false;
            R.isOn = true;
            S.isOn = false;
            Vector3 r = mPreviousSliderValues[2];
            X.InitSliderRange(-180, 180, r.x);
            Y.InitSliderRange(-180, 180, r.y);
            Z.InitSliderRange(-180, 180, r.z);
            X.SetInactive();
            Y.SetInactive();
            modeChanging = false;
        }
    }

    void XValueChanged(float v)
    {
        if (!modeChanging)
        {
            if(T.isOn) mPreviousSliderValues[0].x = v;
            else if (S.isOn) mPreviousSliderValues[1].x = v;
            else mPreviousSliderValues[2].x = v;

            Vector3 p = GetSelectedXformParameter();
            p.y = v;
            SetSelectedXform(ref p, v);
        }
    }

    void YValueChanged(float v)
    {
        if (!modeChanging)
        {
            if (T.isOn) mPreviousSliderValues[0].y = v;
            else if (S.isOn) mPreviousSliderValues[1].y = v;
            else mPreviousSliderValues[2].y = v;

            Vector3 p = GetSelectedXformParameter();
            p.x = v;
            SetSelectedXform(ref p, v);
        }
    }

    void ZValueChanged(float v)
    {
        if (!modeChanging)
        {
            if (T.isOn) mPreviousSliderValues[0].z = v;
            else if (S.isOn) mPreviousSliderValues[1].z = v;
            else mPreviousSliderValues[2].z = v;

            Vector3 p = GetSelectedXformParameter();
            p.z = v;
            SetSelectedXform(ref p, v);
        }
    }

    public void SetSelectedMesh(MyMesh mesh)
    {
        mMesh = mesh;
        if (mesh != null)
        {
            ObjectName.text = "Texture UV Controls";
            mPreviousSliderValues[0] = Vector3.zero;
            mPreviousSliderValues[1] = Vector3.one;
            mPreviousSliderValues[2] = Vector3.zero;
        }
        else
            ObjectName.text = "Disconnected From Mesh";
        ObjectSetUI();
    }

    public void ObjectSetUI()
    {
        Vector3 p = GetSelectedXformParameter();
        X.SetSliderValue(p.x);
        Y.SetSliderValue(p.y);
        Z.SetSliderValue(p.z);
    }

    private Vector3 GetSelectedXformParameter()
    {
        Vector3 p;

        if (T.isOn)
        {
            if (mMesh != null)
            {
                p = mMesh.GetTranslation();
            }
            else
                p = Vector3.zero;
        }
        else if (S.isOn)
        {
            if (mMesh != null)
            {
                p = mMesh.GetScale();
                p.z = 1;
            }
            else
                p = Vector3.one;
        }
        else
        {
            if (mMesh != null)
            {
                p = Vector3.zero;
                p.z = mMesh.GetRotation();
            }
            else
                p = Vector3.zero;
        }
        return p;
    }

    private void SetSelectedXform(ref Vector3 p, float r)
    {
        if (mMesh == null)
            return;

        Vector2 xform = new Vector2(p.x, p.y);

        if (T.isOn)
        {
            mMesh.SetTranslation(xform);
        }
        else if (S.isOn)
        {
            mMesh.SetScale(xform);
        }
        else
        {
            mMesh.SetRotation(r);
        }
    }

}