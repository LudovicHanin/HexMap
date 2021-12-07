using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    #region F/P
    #region Serialize
    [SerializeField] private HexCell[] neighbors;
    #endregion

    #region Private
    private int elevation = int.MinValue;
    private Color color;
    private RectTransform uiRect = null;
    private HexGridChunk chunk;
    private bool hasIncomingRiver, hasOutgoingRiver;
    private HexDirection incomingRiver, outgoingRiver;
    #endregion
    
    #region Public
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
            
            if(hasOutgoingRiver && elevation < GetNeighbor(outgoingRiver).Elevation) RemoveOutgoingRiver();
            if(hasIncomingRiver && elevation > GetNeighbor(incomingRiver).Elevation) RemoveIncomingRiver();

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

    public bool HasIncomingRiver
    {
        get
        {
            return hasIncomingRiver;
        }
    }

    public bool HasOutgoingRiver
    {
        get
        {
            return hasOutgoingRiver;
        }
    }

    public HexDirection IncomingRiver
    {
        get
        {
            return incomingRiver;
        }
    }

    public HexDirection OutgoingRiver
    {
        get
        {
            return outgoingRiver;
        }
    }

    public bool HasRiver
    {
        get
        {
            return hasIncomingRiver || hasOutgoingRiver;
        }
    }

    public bool HasRiverBeginOrEnd
    {
        get
        {
            return hasIncomingRiver != hasOutgoingRiver;
        }
    }
    #endregion
    #endregion

    #region CustomMethods

    #region Private
    private void Refresh()
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

    private void RefreshSelfOnly()
    {
        chunk.Refresh();
    }
    #endregion
    
    #region Public
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

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return hasIncomingRiver && incomingRiver == direction || hasOutgoingRiver && outgoingRiver == direction;
    }

    public void RemoveOutgoingRiver()
    {
        if (!hasOutgoingRiver)
        {
            return;
        }

        hasOutgoingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(outgoingRiver);
        neighbor.hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveIncomingRiver()
    {
        if (!hasIncomingRiver) return;

        hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(incomingRiver);
        neighbor.hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void SetOutgoingriver(HexDirection direction)
    {
        if (hasOutgoingRiver && outgoingRiver == direction) return;
        HexCell neighbor = GetNeighbor(direction);
        if (!neighbor || elevation < neighbor.Elevation) return;
        //Remove the previous Outgoing River and Incoming River if overlaps with new outgoing River
        RemoveOutgoingRiver();
        if(hasIncomingRiver && incomingRiver == direction)
            RemoveIncomingRiver();
        
        //Set the Outgoing River
        hasOutgoingRiver = true;
        outgoingRiver = direction;
        RefreshSelfOnly();
        
        //Set Incoming River and remove its current Incoming River
        neighbor.RemoveIncomingRiver();
        neighbor.hasIncomingRiver = true;
        neighbor.incomingRiver = direction.Opposite();
        neighbor.RefreshSelfOnly();
    }
    #endregion
    #endregion
}