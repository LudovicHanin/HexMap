using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
    #region F/P
    private HexCell[] cells;

    private HexMesh hexMesh;
    private Canvas gridCanvas;
    #endregion

    #region UnityMethods
    private void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
        ShowUI(false);
    }
    private void LateUpdate()
    {
        hexMesh.Triangulate(cells);
        enabled = false;
    }

    #endregion

    #region CustomMethods

    #region Private
    

    #endregion
    
    #region Public
    public void AddCell(int index, HexCell cell)
    {
        cells[index] = cell;
        cell.Chunk = this;
        cell.transform.SetParent(transform, false);
        cell.UiRect.SetParent(gridCanvas.transform, false);
    }

    public void Refresh()
    {
        // hexMesh.Triangulate(cells);
        enabled = true;
    }

    public void ShowUI(bool visible)
    {
        gridCanvas.gameObject.SetActive(visible);
    }
    #endregion

    #endregion

}
