using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class Card : MonoBehaviour, IPointerClickHandler
{
    private Image image;
    private Sprite frontSprite;
    private Sprite backSprite;
    private bool isFaceUp = false;

    public string CardSuit { get; private set; }
    public int CardValue { get; private set; }
    public CardOwner Owner { get; set; }

    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    private void Start()
    {
        if (GetComponent<CanvasGroup>() == null)
            gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(Sprite front, Sprite back, string suit, int value, CardOwner owner)
    {
        image = GetComponent<Image>();

        frontSprite = front;
        backSprite = back;
        image.sprite = backSprite;

        Owner = owner;
        CardSuit = suit;
        CardValue = value;
    }

    public void FlipCard()
    {
        if (image == null) return;

        isFaceUp = !isFaceUp;
        image.sprite = isFaceUp ? frontSprite : backSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        float timeSinceLastClick = Time.time - lastClickTime;


        if (timeSinceLastClick <= doubleClickThreshold)
        {
            FlipCard();
        }

        lastClickTime = Time.time;
    }
}
