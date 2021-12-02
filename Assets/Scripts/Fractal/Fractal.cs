using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    [SerializeField] int maxDepth = 0;
    private int depth = 0;

    public int MaxDepth => maxDepth;
    public int Depth => depth;

    void Start()
    {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
        if (depth < maxDepth)
            new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this);
    }

    void Initialize(Fractal _parent)
    {
        mesh = _parent.mesh;
        material = _parent.material;
        maxDepth = _parent.maxDepth;
        depth = _parent.depth + 1;
        transform.parent = _parent.transform;
    }
}