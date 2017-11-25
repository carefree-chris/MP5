using UnityEngine;

public class MainControl : MonoBehaviour {

	public CameraControl mCamControl;
    public XFormControl mXFormControl;
	public GameObject mSelected;
    public MyMesh mMesh;

	// Use this for initialization
	void Start() {
        Debug.Assert(mCamControl != null);
        Debug.Assert(mMesh != null);
        mSelected = null;
        mXFormControl.SetSelectedMesh(mMesh);
	}
	
	// Update is called once per frame
	void Update() {
        // Try to update the selection on every left mouse click
        if (Input.GetMouseButtonDown(0)) 
		{
	        changeSelection(mCamControl.UpdateSelection());
		}
	}

    // Only change selection when clicking on a new mesh control
	private void changeSelection(GameObject selected)
	{
        if (selected != null && mMesh.IsMyControl(selected))
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
        }
	}

}
