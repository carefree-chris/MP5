using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCylinderMesh : MonoBehaviour {

    private int degrees = 275;
    public Material black;
    public Vector3 origin = Vector3.zero; //public because we might want our cylinder to have a different origin
    private float initialRadiusOffset = 3f;

    private Vector3 StartPoint;
    public int N, M;
    public Vector2 Size;
    protected Mesh mMesh;
    Vector2[] originalUV; //TODO delete?
    protected LineSegment[] mNormals;
    protected GameObject[] mControllers;
    protected Matrix3x3 mXForm;

    // Use this for initialization
    void Start () {
        Debug.Assert(N >= 4 && N <= 20 && M >= 4 && M <= 20);
        mXForm = Matrix3x3Helpers.CreateTRS(Vector2.zero, 0.0f, Vector2.one);
        mMesh = GetComponent<MeshFilter>().mesh;
        //StartPoint = new Vector3(-Size.x / 2.0f, -Size.y / 2.0f, 0.0f); //Todo, switch these - done
        StartPoint = Vector3.zero + new Vector3(0f, -10f, -10f);
        SetResolution(N, M);
    }
	

    public bool SetDegrees(float nDegrees)
    {
        int nVal = (int)nDegrees;

        if (nVal < 10 || nVal > 360)
        {
            return false;
        }
        else
        {
            degrees = nVal;
            SetDegreesHelper();
            Mesh theMesh = GetComponent<MeshFilter>().mesh;
            UpdateNormals(theMesh.vertices, theMesh.normals);
            return true;
        }

        
    }

    //TODO stop arrows from pointing in the wrong direction.
    private void SetDegreesHelper()
    {
        int adjustedDegrees = 0;
        
         for (int i = 0; i < (N + 1) * (M + 1); i = i + M + 1)
         {
            for (int j = 0; j < M + 1; j++)
            {
                Vector3 original = mControllers[j + i].transform.localPosition;

                Vector3 adjustedPosition = new Vector3(
                    origin.x + Mathf.Cos(Mathf.Deg2Rad * adjustedDegrees),
                    0f,
                    origin.z + Mathf.Sin(Mathf.Deg2Rad * adjustedDegrees));

                Vector3 myOrigin = Vector3.zero;
                myOrigin.y = mControllers[j + i].transform.localPosition.y;
                
                Vector3 direction = adjustedPosition.normalized;

                float distance = (mControllers[j + i].transform.localPosition - myOrigin).magnitude;
                
                mControllers[j + i].transform.localPosition = (myOrigin) + (direction * distance);
            }

            adjustedDegrees += (degrees / N);
         }

    }

    private void InitCylinder(int initialDegrees)
    {
        int adjustedDegrees = 0;

        for (int i = 0; i < (N + 1) * (M + 1); i = i + M + 1)
        {


            for (int j = 0; j < M + 1; j++)
            {

                Vector3 adjustedPosition = new Vector3(
                    origin.x + Mathf.Cos(Mathf.Deg2Rad * adjustedDegrees),
                    mControllers[j + i].transform.localPosition.y,
                    origin.z + Mathf.Sin(Mathf.Deg2Rad * adjustedDegrees));

                Vector3 offset = (new Vector3(adjustedPosition.x, 0f, adjustedPosition.z).normalized * initialRadiusOffset);
                mControllers[j + i].transform.localPosition = adjustedPosition + offset;
            }

            adjustedDegrees += (initialDegrees / N);
        }
    }
     
        

    // Initialize triangles using vertex index
    // This follows a "Left-To-Right-Counter-Clockwise" pattern
    private void InitTriangles(int[] t)
    {
        for (int i = 0, index = 0; index < t.Length; ++i)
        {
            // One rectangle (two triangles) at a time
            t[index++] = i;
            t[index++] = i + M + 1;
            t[index++] = i + M + 2;

            t[index++] = i;
            t[index++] = i + M + 2;
            t[index++] = i + 1;

            // Increment i twice when at end of row
            if (i % (M + 1) == M - 1)
            {
                ++i;
            }
        }
    }

    // Initialize vertices based on resolution and size
    // Traverse in order from left to right, top to bottom
    private void InitVertices(Vector2 triangleSize, Vector3[] v, Vector2[] uv)
    {


        /*for (int i = 0; i <= M; ++i)
        {

        }*/


        for (int i = 0; i <= N; ++i)
        {
            for (int j = 0; j <= M; ++j)
            {
                v[(M + 1) * i + j] = StartPoint + (triangleSize.x * j * Vector3.up) + (triangleSize.y * i * Vector3.forward);
                float uvi = i, uvj = j, uvN = N, uvM = M;    //Force float percision on uv
                uv[(M + 1) * i + j] = new Vector2(uvi / uvN, uvj / uvM);
            }
        }
    }

    // Initialize a sphere controller at each vertex
    private void InitControllers(Vector3[] v)
    {
        mControllers = new GameObject[v.Length];
        for (int i = 0; i < v.Length; ++i)
        {
            mControllers[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mControllers[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            mControllers[i].transform.localPosition = v[i];
            mControllers[i].transform.parent = this.transform;
            mControllers[i].transform.name = "Vertex[" + i + "]";

            if (i > M)
            {
                //Make black controllers ignore raycasting, so they can't be selected.
                mControllers[i].gameObject.layer = 2; 
                mControllers[i].gameObject.GetComponent<MeshRenderer>().material = black;

                
            }

        }

        InitCylinder(degrees);
    }

    // Initialize the LineSegments used to show the normals
    private void InitNormals(Vector3[] v, Vector3[] n)
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

    // Update is called once per frame
    void Update()
    {
        
        Mesh theMesh = GetComponent<MeshFilter>().mesh;
        Vector3[] v = theMesh.vertices;
        int[] t = theMesh.triangles;
        

        // Here we update each vertex, according to the controlling vertices
        for (int i = 0; i <= M; i++)
        {
            
            for (int j = i + M + 1; j <= ((N + 1) * (M + 1)) - 1; j += M + 1)
            {

                Vector3 myOrigin = Vector3.zero;
                myOrigin.y = mControllers[j].transform.localPosition.y;

                Vector3 direction = (mControllers[j].transform.localPosition - myOrigin).normalized;

                Vector3 ctrlOrigin = Vector3.zero;
                ctrlOrigin.y = mControllers[i].transform.localPosition.y;
                float distance = (mControllers[i].transform.localPosition - ctrlOrigin).magnitude;


                //Debug.DrawRay(myOrigin, (mControllers[j].transform.position - myOrigin).normalized * distance, Color.red);
                mControllers[j].transform.localPosition = direction * distance;
                
                mControllers[j].transform.localPosition = new Vector3(
                    mControllers[j].transform.localPosition.x,
                    mControllers[i].transform.localPosition.y,
                    mControllers[j].transform.localPosition.z);
                    
            }

        }

        // Set each vertex to the same position as it's controller
        for (int i = 0; i < mControllers.Length; ++i)
        {
            v[i] = mControllers[i].transform.localPosition;
        }

        // Recompute normals and set values to mesh
        theMesh.vertices = v;
        theMesh.normals = ComputeNormals(v, t);

        
        
    }

    public void SetResolution(int nRes, int mRes)
    {
        mMesh.Clear();

        if (mControllers != null && mNormals != null)
        {
            for (int i = 0; i < mControllers.Length; ++i)
            {
                Destroy(mControllers[i]);
                Destroy(mNormals[i].gameObject);
            }
        }

        N = nRes;
        M = mRes;

        int numVerts = (N + 1) * (M + 1);
        Vector3[] v = new Vector3[numVerts];
        Vector3[] n = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts];
        int[] t = new int[6 * M * N];
        Vector2 triangleSize = new Vector2(Size.x / M, Size.y / N);

        // Initialize vertices, uvs, and triangles
        InitVertices(triangleSize, v, uv);
        InitTriangles(t);

        // Initialize normals to all point in the positive Y direction
        for (int i = 0; i < n.Length; ++i)
        {
            n[i] = Vector3.up;
        }

        // Set values to mesh for shader to use
        mMesh.vertices = v;
        mMesh.triangles = t;
        mMesh.normals = n;

        //mMesh.uv = uv;
        //originalUV = uv;
        //UpdateUV();

        // Initialize vertex contols and normal line segments
        InitControllers(v);
        InitNormals(v, n);
    }

    public void DeactivateControls()
    {
        for (int i = 0; i < mControllers.Length; i++)
        {
            mControllers[i].SetActive(false);
            mNormals[i].gameObject.SetActive(false);
        }
    }

    public void ActivateControls()
    {
        for (int i = 0; i < mControllers.Length; i++)
        {
            mControllers[i].SetActive(true);
            mNormals[i].gameObject.SetActive(true);
        }
    }

    // Check if gameobject is one of this meshes controllers
    public bool IsMyControl(GameObject control)
    {
        foreach (GameObject mControl in mControllers)
        {
            if (mControl.Equals(control)) return true;
        }
        return false;
    }

    private Vector3 FaceNormal(Vector3[] v, int i0, int i1, int i2)
    {
        Vector3 a = v[i1] - v[i0];
        Vector3 b = v[i2] - v[i0];
        return Vector3.Cross(a, b).normalized;
    }

    // Using an average of each face the vertex touches, compute a normal
    private Vector3[] ComputeNormals(Vector3[] v, int[] t)
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
            //Negated because normals were facing the wrong way for our cylinder
            normalSums[i] = -normalSums[i];
        }

        // Update normal LineSegments and return normals
        UpdateNormals(v, normalSums);
        return normalSums;
    }

    // Update LineSegments to reflect new vertices and normals
    private void UpdateNormals(Vector3[] v, Vector3[] n)
    {
        for (int i = 0; i < v.Length; i++)
        {
            mNormals[i].SetEndPoints(v[i], v[i] + 1.0f * n[i]);
        }
    }
}
