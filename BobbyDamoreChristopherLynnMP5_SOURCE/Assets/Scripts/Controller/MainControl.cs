using System;
using UnityEngine;
using UnityEngine.UI;

public class MainControl : MonoBehaviour {

    

    public CameraControl mCamControl;
    public XFormControl mXFormControl;
	public GameObject mSelected, mSelectedControl;
    private Color mControlColor;
    public MyMesh mMesh;
    public MyCylinderMesh mCylinder; //Similar to mMesh, but in cylinderical form
    public GameObject ControlAxes;
    private bool mControlAxesSet, mControlsActive;
    private Vector2 clickPosition;

 
    public Dropdown CylinderPlaneToggle;
    public GameObject CylinderResolutionControl;
    public GameObject RotationCtrl;


    //These enum values help us keep track of whether we're manipulating the
    //plane, or the cylinder
    enum quadType {Plane, Cylinder};
    quadType selectedMode;

	// Use this for initialization
	void Start() {

        //We start off manipulating the plane
        selectedMode = quadType.Plane;

        Debug.Assert(mCamControl != null);
        Debug.Assert(mMesh != null);
        Debug.Assert(mCylinder != null);

        mSelected = mSelectedControl = null;
        mXFormControl.SetSelectedMesh(mMesh);
        ControlAxes.SetActive(false);
        mControlAxesSet = mControlsActive = false;
	}

    public void SwitchMode()
    {

        int mode = CylinderPlaneToggle.value;

        switch(mode)
        {
            case 0: //Plane
                mMesh.gameObject.SetActive(true);
                mCylinder.gameObject.SetActive(false);
                DeactivateControls();
                selectedMode = quadType.Plane;
                RotationCtrl.SetActive(false);
                CylinderResolutionControl.SetActive(false);

                break;
            case 1: //Cylinder
                mMesh.gameObject.SetActive(false);
                mCylinder.gameObject.SetActive(true);
                DeactivateControls();
                selectedMode = quadType.Cylinder;
                RotationCtrl.SetActive(true);
                CylinderResolutionControl.SetActive(true);

                break;
            default: Debug.Log("ERROR: This statement should not be reached");
                break;

        }

    }
    
	
	// Update is called once per frame
	void Update() {
        ProcessInput();
	}

    //Handle user input when we're manipulating the plane/cylinder
    private void ProcessInput()
    {
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
                    if (selectedMode == quadType.Plane)
                    {
                        if (mMesh.IsMyControl(selectedObject))
                            changeSelection(selectedObject);
                        else if (selectedObject.transform.parent.gameObject == ControlAxes)
                            StartManipulation(selectedObject);
                    } else if (selectedMode == quadType.Cylinder)
                    {
                        if (mCylinder.IsMyControl(selectedObject))
                            changeSelection(selectedObject);
                        else if (selectedObject.transform.parent.gameObject == ControlAxes)
                            StartManipulation(selectedObject);
                    }

                    
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
        ControlAxes.transform.localPosition = mSelected.transform.localPosition; //TODO: Maybe this shouldn't be local?
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

    //TODO fix bug with z axis
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
        if (selectedMode == quadType.Plane)
        {
            mMesh.DeactivateControls();
        } else if (selectedMode == quadType.Cylinder)
        {
            mCylinder.DeactivateControls();
        }
        
    }

    private void ActivateControls()
    {
        if (mControlAxesSet)
            ControlAxes.SetActive(true);

        ControlAxes.SetActive(false);
        if (selectedMode == quadType.Plane)
        {
            mMesh.ActivateControls();
        }
        else if (selectedMode == quadType.Cylinder)
        {
            mCylinder.ActivateControls();
        }

        //mMesh.ActivateControls();
    }
}
