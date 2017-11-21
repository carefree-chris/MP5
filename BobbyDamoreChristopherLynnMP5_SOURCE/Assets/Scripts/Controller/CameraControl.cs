using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Camera thisCam;
    private const float MIN_ZOOM_DIST = 0.1f;
    private const float MAX_ZOOM_DIST = 100.0f;
    private Vector3 clickFrom = Vector3.zero, clickTo = Vector3.zero;
    protected Vector3 lookAtPoint;

    void Start()
    {
		lookAtPoint = Vector3.zero;
		thisCam.transform.LookAt (lookAtPoint);
    }

    void Update()
    {
		if (Input.GetKey (KeyCode.LeftAlt)) 
		{
			UpdateTumble ();
			UpdateTrack ();
			UpdateDolly ();
		} 
    }

    // Uses a raycast to return the gameobject that the mouse is pointing to
	public GameObject UpdateSelection()
	{
		Ray mouseToWorld = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(mouseToWorld, out hit))
		{
			return hit.collider.gameObject;
		}
		return null;
	}

    // Rotate camera around look at point
    private void UpdateTumble()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickFrom = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            // get click drag delta
            clickTo = Input.mousePosition;
            Vector2 delta = clickTo - clickFrom;
            clickFrom = clickTo;

            // use transformation matrix to transform camera
            // rotating around look at point (pivot)
            Matrix4x4 pivot = Matrix4x4.TRS(lookAtPoint, Quaternion.identity, Vector3.one);
            Matrix4x4 invPivot = Matrix4x4.TRS(-lookAtPoint, Quaternion.identity, Vector3.one);
            Matrix4x4 xRotation = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(delta.x, Vector3.up), Vector3.one);
            Matrix4x4 yRotation = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(-delta.y, thisCam.transform.right), Vector3.one);
            Matrix4x4 tumbleTransformation = pivot * xRotation * yRotation * invPivot;
            Matrix4x4 mTransform = Matrix4x4.TRS(thisCam.transform.localPosition, thisCam.transform.localRotation, thisCam.transform.localScale);
            mTransform = tumbleTransformation * mTransform;

            // apply the transformation to transform
            thisCam.transform.localPosition = mTransform.GetColumn(3);
            thisCam.transform.localScale = new Vector3(mTransform.GetColumn(0).magnitude, mTransform.GetColumn(1).magnitude, mTransform.GetColumn(2).magnitude);
            thisCam.transform.localRotation = Quaternion.LookRotation(mTransform.GetColumn(2), mTransform.GetColumn(1));
        }
    }

    // Translate camera right/left/up/down directions
    private void UpdateTrack()
    {
        if (Input.GetMouseButtonDown(1))
        {
            clickFrom = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            // get click drag delta
            clickTo = Input.mousePosition;
            Vector3 delta = -(clickTo - clickFrom) / 5.0f;
            clickFrom = clickTo;

            // calculate track delta based on camera orientation
            Vector3 rightDelta = Vector3.right * delta.x;
            Vector3 upDelta = Vector3.up * delta.y;

            // translate the camera and look at point
            Vector3 toLookAtPoint = lookAtPoint - thisCam.transform.localPosition;
            thisCam.transform.Translate(rightDelta + upDelta);
            lookAtPoint = thisCam.transform.localPosition + toLookAtPoint;
        }
    }

    // Zoom camera, not past look at point, not too far away from look at point
    private void UpdateDolly()
    {
        // translate camera towards/away from look at point
        Vector3 toLookAtPoint = lookAtPoint - thisCam.transform.localPosition;
        float delta = Input.mouseScrollDelta.y;
        if (toLookAtPoint.magnitude + delta < MAX_ZOOM_DIST && toLookAtPoint.magnitude + delta > MIN_ZOOM_DIST)
        {
            thisCam.transform.localPosition += toLookAtPoint.normalized * delta;
        }
    }
}