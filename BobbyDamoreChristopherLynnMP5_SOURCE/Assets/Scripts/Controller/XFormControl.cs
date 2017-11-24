using UnityEngine;
using UnityEngine.UI;

public class XFormControl : MonoBehaviour
{
    public Toggle T, R, S;
    public SliderWithEcho X, Y, Z;
    public Text ObjectName;

    private MyMesh mMesh;
    private Vector3 mPreviousSliderValues = Vector3.zero;
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
            Vector3 p = GetSelectedXformParameter();
            mPreviousSliderValues = p;
            X.InitSliderRange(-2, 2, p.x);
            Y.InitSliderRange(-2, 2, p.y);
            Z.InitSliderRange(-2, 2, p.z);
            Y.SetInactive();
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
            Vector3 s = GetSelectedXformParameter();
            mPreviousSliderValues = s;
            X.InitSliderRange(0.1f, 20, s.x);
            Y.InitSliderRange(0.1f, 20, s.y);
            Z.InitSliderRange(0.1f, 20, s.z);
            Y.SetInactive();
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
            Vector3 r = GetSelectedXformParameter();
            mPreviousSliderValues = r;
            X.InitSliderRange(-180, 180, r.x);
            Y.InitSliderRange(-180, 180, r.y);
            Z.InitSliderRange(-180, 180, r.z);
            mPreviousSliderValues = r;
            X.SetInactive();
            Z.SetInactive();
            modeChanging = false;
        }
    }

    void XValueChanged(float v)
    {
        if (!modeChanging)
        {
            Vector3 p = GetSelectedXformParameter();
            // if not in rotation, next two lines of work would be wasted
            float dx = v - mPreviousSliderValues.x;
            mPreviousSliderValues.x = v;
            p.x = v;
            SetSelectedXform(ref p, dx);
        }
    }

    void YValueChanged(float v)
    {
        if (!modeChanging)
        {
            Vector3 p = GetSelectedXformParameter();
            // if not in rotation, next two lines of work would be wasted
            float dy = v - mPreviousSliderValues.y;
            mPreviousSliderValues.y = v;
            p.y = v;
            SetSelectedXform(ref p, dy);
        }
    }

    void ZValueChanged(float v)
    {
        if (!modeChanging)
        {
            Vector3 p = GetSelectedXformParameter();
            // if not in rotation, next two lines of work would be wasterd
            float dz = v - mPreviousSliderValues.z;
            mPreviousSliderValues.z = v;
            p.z = v;
            SetSelectedXform(ref p, dz);
        }
    }

    public void SetSelectedMesh(MyMesh mesh)
    {
        mMesh = mesh;
        mPreviousSliderValues = Vector3.zero;
        if (mesh != null)
            ObjectName.text = "Texture UV Controls";
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

                // Change to reflect our 3D space
                p.z = p.y;
                p.y = 0.0f;
            }
            else
                p = Vector3.zero;
        }
        else if (S.isOn)
        {
            if (mMesh != null)
            {
                p = mMesh.GetScale();

                // Change to reflect our 3D space
                p.z = p.y;
                p.y = 1.0f;
            }
            else
                p = Vector3.one;
        }
        else
        {
            p = Vector3.zero;
        }
        return p;
    }

    private void SetSelectedXform(ref Vector3 p, float r)
    {
        if (mMesh == null)
            return;

        Vector2 xform = new Vector2(p.x, p.z);

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