using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    [SerializeField] private HexCell[] neighbors;
    private int elevation;
    private Color color;
    private RectTransform uiRect = null;

    public RectTransform UiRect
    {
        get => uiRect;
        set => uiRect = value;
    }

    public int Elevation
    {
        get => elevation;
        set
        {
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;
        }
    }
    public HexCoordinates coordinates;

    public Color ColorField
    {
        get => color;
        set => color = value;
    }

    public HexCell GetNeighbor(HexDirection direction) => neighbors[(int) direction];
    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int) direction] = cell;
        cell.neighbors[(int) direction.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(elevation, neighbors[(int) direction].elevation);
    }
}
