using UnityEngine;
using UnityEngine.EventSystems;

public class AnimalDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameManager gameManager;

    public void OnBeginDrag(PointerEventData eventData) => gameManager.BeginDrag(gameObject, eventData);

    public void OnDrag(PointerEventData eventData) => gameManager.DuringDrag(gameObject, eventData);

    public void OnEndDrag(PointerEventData eventData) => gameManager.EndDrag(gameObject, eventData);
}