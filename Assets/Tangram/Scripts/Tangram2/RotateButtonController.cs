using UnityEngine;
using UnityEngine.EventSystems;

public class RotateButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float rotationSpeed = 100f;
    void Update()
    {
        if (isPressed && ArrasteComRotacao.selectedPiece != null)
        {
            ArrasteComRotacao.selectedPiece.RotatePiece(rotationSpeed);
        }
    }

    bool isPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}