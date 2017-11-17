using UnityEngine;

public class MyMesh : MonoBehaviour {

	public Vector3 StartPoint;
	public int N;
	public float Size;
	protected Mesh mMesh;
    protected LineSegment[] mNormals;
    protected GameObject[] mControllers;

    // Use this for initialization
    void Start () {
		Debug.Assert (StartPoint != null);
		Debug.Assert (N >= 2 && N <= 20);

        mMesh = GetComponent<MeshFilter>().mesh;   // get the mesh component
        mMesh.Clear();    // delete whatever is there!!

        int numVerts = (int)Mathf.Pow(N + 1, 2);
        Vector3[] v = new Vector3[numVerts];
        Vector3[] n = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts];
        int[] t = new int[6 * (int)Mathf.Pow(N, 2)];
		float triangleSize = Size / N;

        // Initialize vertices based on resolution and size
        // Traverse in order from left to right, top to bottom
		for (int i = 0; i <= N; ++i) {
			for (int j = 0; j <= N; ++j) {
				v [(N + 1) * i + j] = StartPoint + triangleSize * (j * Vector3.right + i * Vector3.forward);
                float uvi = i, uvj = j, uvN = N;    //Force float percision on uv
                uv[(N + 1) * i + j] = new Vector2(uvi / uvN, uvj / uvN);
			}
		}

        // Initialize triangles using vertex index
        // This follows a "Left-To-Right-Counter-Clockwise" pattern
        for (int i = 0, index = 0; index < 6 * Mathf.Pow (N, 2); ++i) {
            // One rectangle at a time
            t [index++] = i;
			t [index++] = i + N + 1;
			t [index++] = i + N + 2;

			t [index++] = i;
			t [index++] = i + N + 2;
			t [index++] = i + 1;

            // Increment i twice when at end of row
			if (i % (N + 1) == N - 1) {
				++i;
			}
		}
        
        // Initial normals should all be in the positive Y direction
        for (int i = 0; i < n.Length; ++i)
        {
            n[i] = new Vector3(0, 1, 0);
        }

        // Set values to mesh for shader to use
        mMesh.vertices = v;
		mMesh.triangles = t;
		mMesh.normals = n;
        mMesh.uv = uv;

        // Initialize vertex contols and normal line segments
        InitControllers(v);
        InitNormals(v, n);
    }

    // Update is called once per frame
    void Update () {
        Mesh theMesh = GetComponent<MeshFilter>().mesh;
        Vector3[] v = theMesh.vertices;
        int[] t = theMesh.triangles;

        // Set each vertex to the same position as it's controller
        for (int i = 0; i < mControllers.Length; ++i)
        {
            v[i] = mControllers[i].transform.localPosition;
        }

        // Recompute normals and set values to mesh
        theMesh.vertices = v;
        theMesh.normals = ComputeNormals(v, t);
    }

    void InitControllers(Vector3[] v)
    {
        mControllers = new GameObject[v.Length];
        for (int i = 0; i < v.Length; ++i)
        {
            mControllers[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mControllers[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            mControllers[i].transform.localPosition = v[i];
            mControllers[i].transform.parent = this.transform;
        }
    }

    void InitNormals(Vector3[] v, Vector3[] n)
    {
        mNormals = new LineSegment[v.Length];
        for (int i = 0; i < v.Length; i++)
        {
            GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mNormals[i] = o.AddComponent<LineSegment>();
            mNormals[i].SetWidth(0.1f);
            mNormals[i].transform.SetParent(this.transform);
        }
        UpdateNormals(v, n);
    }

    void UpdateNormals(Vector3[] v, Vector3[] n)
    {
        for (int i = 0; i < v.Length; i++)
        {
            mNormals[i].SetEndPoints(v[i], v[i] + 1.0f * n[i]);
        }
    }

    Vector3 FaceNormal(Vector3[] v, int i0, int i1, int i2)
    {
        Vector3 a = v[i1] - v[i0];
        Vector3 b = v[i2] - v[i0];
        return Vector3.Cross(a, b).normalized;
    }

    Vector3[] ComputeNormals(Vector3[] v, int[] t)
    {
        // Compute the normal of each triangle
        Vector3[] triNormal = new Vector3[t.Length / 3];
        for (int i = 0, index = 0; index < triNormal.Length; i += 3)
        {
            triNormal[index++] = FaceNormal(v, t[i], t[i + 1], t[i + 2]);
        }

        // Compute running sum for each vertex normal
        Vector3[] normalSums = new Vector3[v.Length];
        for (int i = 0, index = 0; index < triNormal.Length; i += 3, ++index)
        {
            normalSums[t[i]] += triNormal[index];
            normalSums[t[i + 1]] += triNormal[index];
            normalSums[t[i + 2]] += triNormal[index];
        }

        // Normalize each vertex normal
        for (int i = 0; i < normalSums.Length; ++i)
        {
            normalSums[i].Normalize();
        }

        // Update normal line segments and return normals
        UpdateNormals(v, normalSums);
        return normalSums;
    }
}
