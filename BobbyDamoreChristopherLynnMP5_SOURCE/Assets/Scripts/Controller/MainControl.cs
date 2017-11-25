using System;
using UnityEngine;

public class MainControl : MonoBehaviour {

	public CameraControl mCamControl;
    public XFormControl mXFormControl;
	public GameObject mSelected, mSelectedControl;
    private Color mControlColor;
    public MyMesh mMesh;
    public GameObject ControlAxes;
    private bool mControlAxesSet, mControlsActive;
    private Vector2 clickPosition;

	// Use this for initialization
	void Start() {
        Debug.Assert(mCamControl != null);
        Debug.Assert(mMesh != null);
        mSelected = mSelectedControl = null;
        mXFormControl.SetSelectedMesh(mMesh);
        ControlAxes.SetActive(false);
        mControlAxesSet = mControlsActive = false;
	}
	
	// Update is called once per frame
	void Update() {
        if (Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftAlt))
        {
            if (!mControlsActive)
            {
                ActivateControls();
                mControlsActive = true;
            }
            
            // Try to update the selection on every left mouse click
            if (Input.GetMouseButtonDown(0))
            {
                GameObject selectedObject = mCamControl.UpdateSelection();
                if (selectedObject)
                {
                    if (mMesh.IsMyControl(selectedObject))
                        changeSelection(selectedObject);
                    else if (selectedObject.transform.parent.gameObject == ControlAxes)
                        StartManipulation(selectedObject);
                }
                else
                    Unselect();

            }
            else if (Input.GetMouseButton(0))
            {
                if (mSelectedControl != null)
                    ContinueManipulation();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (mSelectedControl != null)
                    EndManipulation();
            }
        }
        else
        {
            DeactivateControls();
            mControlsActive = false;
        }
	}

    // Only change selection when clicking on a new mesh control
    private void changeSelection(GameObject selected)
	{
        // Update last selected color back to white
        if (mSelected != null)
        {
            mSelected.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            mSelected = null;
        }
        // Change newly selected color to yellow
        mSelected = selected;
        mSelected.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        ControlAxes.transform.localPosition = mSelected.transform.localPosition;
        mControlAxesSet = true;
        ControlAxes.SetActive(true);
	}

    private void StartManipulation(GameObject control)
    {
        // Change newly selected control to yellow
        mSelectedControl = control;
        mControlColor = mSelectedControl.GetComponent<Renderer>().material.color;
        mSelectedControl.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        clickPosition = Input.mousePosition;
    }

    private void EndManipulation()
    {
        mSelectedControl.GetComponent<Renderer>().material.SetColor("_Color", mControlColor);
        mSelectedControl = null;
    }

    private void ContinueManipulation()
    {
        Vector2 mousePosition = Input.mousePosition;
        float delta = (mousePosition - clickPosition).magnitude;
        if (mousePosition.y < clickPosition.y)
            delta *= -1;
        clickPosition = mousePosition;

        Vector3 translation = mSelectedControl.transform.up * delta / 10.0f;
        ControlAxes.transform.localPosition += translation;
        mSelected.transform.localPosition += translation;
    }

    public void Unselect()
    {
        // Update last selected color back to white
        if (mSelected != null)
        {
            mSelected.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            mSelected = null;
        }

        ControlAxes.SetActive(false);
        mControlAxesSet = false;
    }

    private void DeactivateControls()
    {
        ControlAxes.SetActive(false);
        mMesh.DeactivateControls();
    }

    private void ActivateControls()
    {
        if (mControlAxesSet)
            ControlAxes.SetActive(true);
        mMesh.ActivateControls();
    }
}
