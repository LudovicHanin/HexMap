using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    [SerializeField] int maxDepth = 0;
    [SerializeField] float childScale = 0;

    private static Vector3[] childDirections =
    {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward, 
        Vector3.back
    };

    private static Quaternion[] childOrientations =
    {
        Quaternion.identity, 
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f,0f, 90f),
        Quaternion.Euler(90f,0f, 0f),
        Quaternion.Euler(-90f,0f, 0f)
    };
    
    private int depth = 0;

    public int MaxDepth => maxDepth;
    public int Depth => depth;

    void Start()
    {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
        if (depth < maxDepth)
        {
            StartCoroutine(CreateChildren());
        }
    }

    IEnumerator CreateChildren()
    {
        for(int i = 0; i < childDirections.Length; i++)
        {
            yield return new WaitForSeconds(0.5f);
            new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, i);
        }
    }
    
    void Initialize(Fractal _parent, int _childIndex)
    {
        mesh = _parent.mesh;
        material = _parent.material;
        maxDepth = _parent.maxDepth;
        depth = _parent.depth + 1;
        childScale = _parent.childScale;
        transform.parent = _parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = childDirections[_childIndex] * (0.5f + 0.5f * childScale);
        transform.localRotation = childOrientations[_childIndex];
    }
}