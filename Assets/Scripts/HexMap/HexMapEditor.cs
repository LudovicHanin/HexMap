using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    [SerializeField] Color[] colors;
    [SerializeField] private HexGrid hexGrid;
    
    private Color activeColor;
    private int activeElevation;

    private void Awake()
    {
        SelectColor(0);
    }
    private void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) HandleInput();
    }
    
    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) EditCell(hexGrid.GetCell(hit.point));
    }

    void EditCell(HexCell cell)
    {
        cell.ColorField = activeColor;
        cell.Elevation = activeElevation;
        hexGrid.Refresh();
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int) elevation;
    }
    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }
}
