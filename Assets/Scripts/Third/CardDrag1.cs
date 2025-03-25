using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardDrag1 : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTranform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Canvas canvas;
    private CardOwner1 cardOwner;
    public GameObject playerHandObject;
    private Card1 card1;
    private int lastSiblingIndex;

    public GameObject discardPile;

    private void Awake()
    {
        rectTranform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        playerHandObject = GameObject.Find("PlayerHand");
        if (playerHandObject == null)
        {
            Debug.LogError("PlayerHand GameObject not found in the scene!");
        }

        discardPile = GameObject.Find("DiscardPile");


    }

    private void Update()
    {
        int index = 0;
        foreach (Card1 card in playerHandObject.GetComponentsInChildren<Card1>())
        {
            card.index = index;
            index++;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null &&
            eventData.pointerCurrentRaycast.gameObject.TryGetComponent<Card1>(out Card1 clickedCard))
        {
            if (clickedCard.Owner == CardOwner1.Opponent)
            {
                Debug.LogWarning("Cannot drag opponents cards!");
                ReturnToOriginalPosition(CardOwner1.Opponent);
                return;
            }
        }

        card1 = GetComponent<Card1>();
        cardOwner = card1.Owner; //???

        originalParent = transform.parent;
        transform.SetParent(canvas.transform, true);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;
        card1.GetComponent<RectTransform>().localScale = new Vector3(40, 40, 1);

        foreach (Transform child in discardPile.transform)
        {
            child.GetComponent<Image>().raycastTarget = false;
        }

        Debug.Log($"'{card1.name}' owned by '{card1.Owner}' index: '{card1.index}' OnBeginDrag()");
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTranform.position = eventData.position;
        //get the index of card going over to swap
        if (eventData.pointerCurrentRaycast.gameObject != null &&
            eventData.pointerCurrentRaycast.gameObject.TryGetComponent<Card1>(out Card1 hoveredCard))
        {
            lastSiblingIndex = hoveredCard.index;
            //Debug.Log($"Dragging over '{hoveredCard.name}', index: {hoveredCard.index}");
        }
    }

    //##################################
    //Check where to snap cards
    //if in valid place: snaps the card, else return to original parant
    //calls EndTurn() after discarting
    public void OnEndDrag(PointerEventData eventData)
    {

        Debug.Log($"{card1.name} OnEndDrag() START");

        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;

        if (dropTarget != null)
        {
            if (dropTarget.transform.parent == playerHandObject.transform)
            {
                SwapWith(dropTarget.transform);
            }
            else if (dropTarget.CompareTag("PlayerHand") && card1.Owner != CardOwner1.Opponent)
            {
                SnapToHand(playerHandObject.transform, lastSiblingIndex);
            }
            else if (dropTarget.CompareTag("DiscardPile") && card1.Owner != CardOwner1.Opponent)
            {
                SnapToDiscard(dropTarget.transform);
            }
            else
            {
                ReturnToOriginalPosition(card1.Owner);
            }
        }
        else
        {
            ReturnToOriginalPosition(card1.Owner);
        }

        foreach (Transform child in discardPile.transform)
        {
            child.GetComponent<Image>().raycastTarget = true;
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;
        card1.GetComponent<RectTransform>().localScale = new Vector3(35, 35, 0);

    }

    void SwapWith(Transform targetCard)
    {
        Debug.Log("SwapWith()");
        if (!card1.isFaceUp)
        {
            card1.FlipCard();
        }

        if (card1.index == -1) card1.index = lastSiblingIndex;

        card1.transform.SetParent(playerHandObject.transform);
        transform.SetSiblingIndex(lastSiblingIndex);
        targetCard.SetSiblingIndex(card1.index);
    }
    public void SnapToHand(Transform handTransform, int lastSiblingIndex)
    {
        Debug.Log("SnapToHand()");
        if (!card1.isFaceUp)
        {
            card1.FlipCard();
        }

        transform.SetParent(handTransform, true);
        transform.SetSiblingIndex(lastSiblingIndex);
        transform.localPosition = Vector3.zero;
        GetComponent<Card1>().Owner = CardOwner1.Player;

        LogCard(handTransform);
    }

    public void SnapToDiscard(Transform discardTransform)
    {

        transform.SetParent(discardTransform, false);
        transform.SetAsLastSibling(); //maybe not needed.
        transform.localPosition = Vector3.zero;


        if (!card1.isFaceUp)
        {
            card1.FlipCard();
        }

        GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        card1.Owner = CardOwner1.None; //needed?

        
        //GetComponent<Image>().raycastTarget = false; //check if true;
        //GetComponent<CanvasGroup>().blocksRaycasts = false;

        foreach (Card1 child in discardTransform.GetComponentsInChildren<Card1>())
        {
            child.GetComponent<Image>().CrossFadeAlpha(0.75f, 0.3f, true);
            child.Owner = CardOwner1.None;

            if (child.transform.GetSiblingIndex() == discardTransform.transform.childCount - 1)
            {
                Debug.Log($"{child.name} is the last child on discard pile.");
                child.GetComponent<Image>().raycastTarget = true;
                child.GetComponent<Image>().CrossFadeAlpha(1f, 0, true);
            }
            else
            {
                child.GetComponent<Image>().raycastTarget = false;
            }
            
        }

        LogCard(discardTransform);

        //ends the turn;
        Debug.Log($" player has {playerHandObject.transform.childCount} cards in hand!");
        

        FindFirstObjectByType<PiffGameManager>().CheckVictory(playerHandObject.transform);
        FindFirstObjectByType<PiffGameManager>().EndTurn();
    }

    public void ReturnToOriginalPosition(CardOwner1 owner)
    {
        if (originalParent.CompareTag("DrawingPile"))
        {
            transform.SetParent(originalParent, false);
            int cardCount = originalParent.childCount - 1;
            float offsetX = cardCount * 5f;
            transform.localPosition = new Vector3(offsetX, 0, 0);
        }
        else
        {
            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(lastSiblingIndex);// ???
            transform.localPosition = Vector3.zero;
        }

        LogCard();

    }

    void LogCard(Transform dropTarget = null)
    {

        Card1 card = GetComponent<Card1>();
        Debug.Log($"{card.name} value '{card.CardValue}', original owner was '{cardOwner}', original parent was '{originalParent}' into {dropTarget}, new owner is '{card.Owner}' and index in hand is '{card.index}'");
    }
}
