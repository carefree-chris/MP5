using UnityEngine;

public class LineSegment : MonoBehaviour
{
	private float mWidth;

	public void SetWidth(float v)
    {
        transform.localScale = new Vector3(v, transform.localScale.y, v);
		mWidth = v;
    }

    public void SetEndPoints(Vector3 start, Vector3 end)
    {
        transform.up = (end - start).normalized;
        transform.localPosition = (start + end) / 2;
		transform.localScale = new Vector3 (mWidth, (end - start).magnitude / 2.0f, mWidth);
    }
}