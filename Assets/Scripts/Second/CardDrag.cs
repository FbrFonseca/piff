using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Card card = GetComponent<Card>();

        if ((FindFirstObjectByType<CardGame>().isPlayerTurn && card.Owner != CardOwner.Player) ||
            (!FindFirstObjectByType<CardGame>().isPlayerTurn && card.Owner != CardOwner.Opponent))
        {
            return;
        }

        originalParent = transform.parent;
        transform.SetParent(canvas.transform, true);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
        Card card = GetComponent<Card>();

        if (dropTarget != null)
        {
            if (dropTarget.CompareTag("PlayerHand") && card.Owner != CardOwner.Opponent)
            {
                SnapToHand(dropTarget.transform);
            }
            else if (dropTarget.CompareTag("DiscardPile") && card.Owner != CardOwner.Opponent)
            {

                SnapToDiscard(dropTarget.transform);
                FindFirstObjectByType<CardGame>().EndTurn();
            }
            else
            {
                ReturnToOriginalPosition();
            }

        }
        else
        {
            ReturnToOriginalPosition();
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    private void SnapToHand(Transform handTransform)
    {
        transform.SetParent(handTransform, false);
        transform.localPosition = Vector2.zero; // Align with hand position
    }

    private void SnapToDiscard(Transform discardTransform)
    {
        // Set this card's parent to the discard pile
        transform.SetParent(discardTransform, false);
        transform.SetAsLastSibling(); // Ensures it's always on top
        transform.localPosition = Vector2.zero; // Align with hand position

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        GetComponent<Image>().raycastTarget = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;

    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent, false);
        transform.localPosition = Vector2.zero;
    }
}


