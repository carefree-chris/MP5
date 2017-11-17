using UnityEngine;

public class LineSegment : MonoBehaviour
{
    public void SetWidth(float v)
    {
        transform.localScale = new Vector3(v, transform.localScale.y, v);
    }

    public void SetEndPoints(Vector3 start, Vector3 end)
    {
        transform.up = (end - start).normalized;
        transform.localPosition = (start + end) / 2;
    }
}