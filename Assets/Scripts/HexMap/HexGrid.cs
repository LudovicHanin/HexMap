using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Serialization;

public class HexGrid : MonoBehaviour
{
    #region F/P

    [SerializeField] private int cellCountX = 6;
    [SerializeField] private int cellCountZ = 6;
    [SerializeField] private int chunkCountX = 4, chunkCountZ = 3;
    [SerializeField] private HexCell cellPrefab = null;
    [SerializeField] private TMP_Text cellLabelPrefab = null;

    [SerializeField] private Texture2D noiseSource = null;
    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] HexGridChunk chunkPrefab = null;


    // private Canvas gridCanvas = null;
    // private HexMesh hexMesh = null;

    private HexCell[] cells;

    private HexGridChunk[] chunks;

    //
    public int CellCountX => cellCountX;
    public int CellCountZ => cellCountZ;
    public int ChunkCountX => chunkCountX;
    public int ChunkCountZ => chunkCountZ;
    public HexCell CellPrefab => cellPrefab;
    public TMP_Text CellLabelPrefab => cellLabelPrefab;
    // public Canvas GridCanvas => gridCanvas;

    public HexGridChunk ChunkPrefab => chunkPrefab;

    #endregion

    #region UnityMethods

    private void OnEnable()
    {
        HexMetrics.noiseSource = noiseSource;
    }

    private void Awake()
    {
        HexMetrics.noiseSource = noiseSource;
        
        // gridCanvas = GetComponentInChildren<Canvas>();
        // hexMesh = GetComponentInChildren<HexMesh>();

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();
    }



    // private void Start()
    // {
    //     hexMesh.Triangulate(cells);
    // }

    #endregion

    #region CustomMethods
    private void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    private void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }
    private void CreateCell(int x, int z, int i)
    {
        Vector3 _position = Vector3.zero;
        _position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        _position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell _cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        // _cell.transform.SetParent(transform, false);
        _cell.transform.localPosition = _position;
        _cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        _cell.ColorField = defaultColor;

        if (x > 0)
        {
            _cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }

        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                _cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    _cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                _cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    _cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        TMP_Text _label = Instantiate<TMP_Text>(cellLabelPrefab);
        // _label.rectTransform.SetParent(gridCanvas.transform, false);
        _label.rectTransform.anchoredPosition = new Vector2(_position.x, _position.z);
        _label.text = _cell.coordinates.ToStringOnSeparateLines();

        _cell.UiRect = _label.rectTransform;

        _cell.Elevation = 0;

        AddCellToChunk(x, z, _cell);
    }

    private void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    // public void Refresh()
    // {
    //     hexMesh.Triangulate(cells);
    // }
    #endregion
}