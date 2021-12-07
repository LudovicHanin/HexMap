using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    #region F/P

    [SerializeField] private HexCell[] neighbors;
    private int elevation = int.MinValue;
    private Color color;
    private RectTransform uiRect = null;

    private HexGridChunk chunk;

    //
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
            if (elevation == value) return;

            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;

            Refresh();
        }
    }

    public Vector3 Position
    {
        get { return transform.localPosition; }
    }

    public HexCoordinates coordinates;

    public Color ColorField
    {
        get => color;
        set
        {
            if (color == value) return;
            color = value;
            Refresh();
        }
    }

    public HexGridChunk Chunk
    {
        get => chunk;
        set => chunk = value;
    }

    #endregion

    #region CustomMethods

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

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }

    void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = this.neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }

    #endregion
}