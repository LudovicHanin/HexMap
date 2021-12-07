using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
    #region F/P

    #region Serialized

    [SerializeField] private float stickMinZoom = -250f;
    [SerializeField] private float stickMaxZoom = -45f;
    [SerializeField] private float swivelMinZoom = 90f;
    [SerializeField] private float swivelMaxZoom = 45f;

    [SerializeField] private float moveSpeedMinZoom = 400f;
    [SerializeField] private float moveSpeedMaxZoom = 100f;

    [SerializeField] private float rotationSpeed = 180f;

    [SerializeField] private HexGrid grid = null;

    #endregion

    #region Private

    private Transform swivel = null, stick = null;
    private float zoom = 1f;
    private float rotationAngle = 0f;

    #endregion

    #region Public

    #endregion

    #endregion

    #region UnityMethods

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }

    private void Update()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
        }

        float rotationDelta = Input.GetAxis("Rotation");
        if (rotationDelta != 0f)
        {
            AdjustRotation(rotationDelta);
        }

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if (xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
        }
    }

    #endregion

    #region CustomMethods

    #region Private

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    void AdjustPosition(float xDelta, float zDelta)
    {
        Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float speed = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom);
        float distance = speed * damping * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    Vector3 ClampPosition(Vector3 position)
    {
        float xMax = (grid.ChunkCountX * HexMetrics.chunkSizeX - 0.5f) * (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax = (grid.ChunkCountZ * HexMetrics.chunkSizeZ - 1f) * (1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);
        return position;
    }

    void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        
        if (rotationAngle < 0f) rotationAngle += 360f;
        else if (rotationAngle >= 360f) rotationAngle -= 360f;
        
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }

    #endregion

    #region Public

    #endregion

    #endregion
}