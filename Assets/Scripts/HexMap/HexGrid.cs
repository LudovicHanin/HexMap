using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    #region F/P

    [SerializeField] private int width = 6;
    [SerializeField] private int height = 6;
    [SerializeField] private HexCell cellPrefab = null;
    [SerializeField] private TMP_Text cellLabelPrefab = null;

    [SerializeField] Color defaultColor = Color.white;

    //[SerializeField] Color touchedColor = Color.magenta;
    //
    private Canvas gridCanvas = null;
    private HexMesh hexMesh = null;

    private HexCell[] cells;

    //
    public int Width => width;
    public int Height => height;
    public HexCell CellPrefab => cellPrefab;
    public TMP_Text CellLabelPrefab => cellLabelPrefab;
    public Canvas GridCanvas => gridCanvas;

    #endregion

    #region UnityMethods

    private void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[height * width];

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    private void Start()
    {
        hexMesh.Triangulate(cells);
    }

    #endregion

    #region CustomMethods

    private void CreateCell(int x, int z, int i)
    {
        Vector3 _position = Vector3.zero;
        _position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        _position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell _cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        _cell.transform.SetParent(transform, false);
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
                _cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                if (x > 0)
                {
                    _cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                }
            }
            else
            {
                _cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                if (x < width - 1)
                {
                    _cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                }
            }
        }

        TMP_Text _label = Instantiate<TMP_Text>(cellLabelPrefab);
        _label.rectTransform.SetParent(gridCanvas.transform, false);
        _label.rectTransform.anchoredPosition = new Vector2(_position.x, _position.z);
        _label.text = _cell.coordinates.ToStringOnSeparateLines();

        _cell.UiRect = _label.rectTransform;
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        return cells[index];
    }

    public void Refresh()
    {
        hexMesh.Triangulate(cells);
    }
    #endregion
}