using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerHand : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedCard = eventData.pointerDrag;

        if (droppedCard != null)
        {
            Card card = droppedCard.GetComponent<Card>();
            if (card != null && card.Owner != CardOwner.Opponent)
            { 
                droppedCard.transform.SetParent(transform, true);
                droppedCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            }
            else
            {
                Debug.LogWarning($"Rejected drop for {droppedCard.name} due to ownership mismatch!");
            }
        }
    }
}
