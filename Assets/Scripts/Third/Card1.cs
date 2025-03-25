using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card1 : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    private Sprite frontSprite;
    private Sprite backSprite;
    public bool isFaceUp;

    public string CardSuit {  get; private set; }
    public int CardValue { get; private set; }

    public CardOwner1 Owner { get; set; }
    public int index;
    public int lastSiblingIndex;

    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    private void Start()
    {
        if (GetComponent<CanvasGroup>() == null) gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(Sprite front, Sprite back, string suit, int value, CardOwner1 owner)
    {
        image = GetComponent<Image>();

        frontSprite = front;
        backSprite = back;
        image.sprite = backSprite;
        
        CardSuit = suit;
        CardValue = value;
        Owner = owner;
        index = -1;
    }

    public void FlipCard()
    {
        if (image == null)
        {
            Debug.LogWarning("FlipCard(): Image is null!");
            return;
        }

        isFaceUp = !isFaceUp;
        image.sprite = isFaceUp ? frontSprite : backSprite;

       // Debug.Log("card flipped!");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <=  doubleClickThreshold)
        {
            FlipCard();
        }

        lastClickTime = Time.time;
    }
}
