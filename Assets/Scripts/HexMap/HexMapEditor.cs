using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    #region F/P

    #region Serialized

    [SerializeField] Color[] colors;
    [SerializeField] private HexGrid hexGrid;

    #endregion

    #region Private
    private Color activeColor;
    private int activeElevation;
    private int brushSize;
    private bool applyColor = true;
    private bool applyElevation = true;

    #endregion

    #region Public

    #endregion

    #endregion

    #region UnityMethods
    private void Awake()
    {
        SelectColor(0);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) HandleInput();
    }
    #endregion
    
    void HandleInput()
    {
        if (!hexGrid) return;
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            HexCell cell = hexGrid.GetCell(hit.point);
            if (cell == null) return;
            EditCell(cell);
        }
    }

    void EditCell(HexCell cell)
    {
        if (applyColor)
        {
            cell.ColorField = activeColor;
        }

        if (applyElevation)
        {
            cell.Elevation = activeElevation;
        }
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int) elevation;
    }

    public void SelectColor(int index)
    {
        applyColor = index >= 0;
        if (applyColor)
        {
            activeColor = colors[index];
        }
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetBrushSize(float size)
    {
        brushSize = (int) size;
    }
}