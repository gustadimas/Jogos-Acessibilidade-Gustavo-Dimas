using UnityEngine;
using UnityEngine.EventSystems;

public class ArrasteComRotacao : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Vector3 originalPosition;
    public bool isDragging = false;
    public static ArrasteComRotacao selectedPiece;
    public bool isRotating = false;
    public float rotationSpeed = 100f;
    Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        originalPosition = transform.position;
    }

    void Update()
    {
        if (isRotating && !isDragging)
        {
            RotatePiece(rotationSpeed);
        }
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        if (GetComponent<Associacao>() != null && GetComponent<Associacao>().pecaNoLugar)
            return;
        isDragging = true;
        originalPosition = transform.position;
        selectedPiece = this;
    }

    public void OnDrag(PointerEventData _eventData)
    {
        if (!isDragging) return;
        Vector3 _newPos = _mainCamera.ScreenToWorldPoint(_eventData.position);
        _newPos.z = transform.position.z;
        transform.position = _newPos;
    }

    public void OnPointerUp(PointerEventData _eventData)
    {
        isDragging = false;
        if (GetComponent<Associacao>() != null && !GetComponent<Associacao>().pecaNoLugar)
            ResetPosition();
    }

    public void ResetPosition() => transform.position = originalPosition;

    public void RotatePiece(float _angle)
    {
        transform.Rotate(0, 0, _angle * Time.deltaTime);
    }

    public void OnRotateButtonDown()
    {
        isRotating = true;
    }
    public void OnRotateButtonUp()
    {
        isRotating = false;
    }
}