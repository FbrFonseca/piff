using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public enum CardOwner1 { Player, Opponent, None  };
public class PiffGameManager : MonoBehaviour
{
    public Transform playerHand;
    public Transform opponentHand;
    public Transform discardPile;
    public Transform drawingPile;

    public GameObject victoryCardUI;
    public GameObject defeatCardUI;

    public Sprite[] sprites;
    public Sprite cardBack;
    public GameObject cardPrefab;

    public bool isPlayerTurn = true;
    public bool isCardDrawn = false;

    public Stack<GameObject> deck = new Stack<GameObject>();

    void Start()
    {
        GenerateDeck();
        ShuffleDeck();
        SpawnDeck();
        
        StartCoroutine(DealInitialHands());
    }

    private void Update()
    {
        
    }

    void GenerateDeck()
    {
        string[] suits = new string[] { "Clubs", "Diamonds", "Hearts", "Spades" };
        string[] values = new string[] { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };

        int spriteIndex = 0;

        for (int suit = 0; suit < suits.Length; suit++)
        {
            for (int value = 0; value < values.Length; value++)
            {
                string cardName = values[value] + " " + suits[suit];

                GameObject newCard = Instantiate(cardPrefab, drawingPile);
                newCard.name = cardName;

                Card1 cardScript = newCard.AddComponent<Card1>();
                cardScript.Setup(sprites[spriteIndex], cardBack, suits[suit], value + 1, CardOwner1.None);

                newCard.transform.SetParent(drawingPile, false);
                newCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                newCard.transform.SetAsFirstSibling();

                deck.Push(newCard);
                spriteIndex++;
            }
        }
    }

    void ShuffleDeck()
    {
        GameObject[] shuffleCards = deck.ToArray();

        for (int i = shuffleCards.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffleCards[i], shuffleCards[j]) = (shuffleCards[j], shuffleCards[i]);
        }

        deck.Clear();

        foreach (var card in shuffleCards)
        {
            deck.Push(card);
            card.transform.SetAsLastSibling();
        }
    }

    void SpawnDeck()
    {
        int offSetX = 0;
        int offSetZ = 0;


        foreach (Transform card in drawingPile)
        {
            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector3(offSetX, 0, offSetZ);
            offSetX += 5;
            offSetZ += 1;
        }
    }

    IEnumerator DealInitialHands()
    {
        for (int i = 0; i < 9; i++)
        {
            yield return new WaitForSeconds(0.1f);
            DealCard(playerHand, CardOwner1.Player).GetComponent<Card1>().FlipCard();
            
            DealCard(opponentHand, CardOwner1.Opponent).GetComponent<Card1>().FlipCard();
        }
    }

    Transform DealCard(Transform hand, CardOwner1 owner)
    {
        if (deck.Count == 0)
        {
            Debug.LogWarning("No cards left on deck!");
            return null;

            //maybe restart the deck or end the game as draw.
        }


        Transform card = drawingPile.GetChild(drawingPile.childCount - 1);
        card.transform.SetParent(hand, false);
        card.GetComponent<Card1>().Owner = owner;

        return card;
    }

    public void EndTurn()
    {
        
        isPlayerTurn = !isPlayerTurn;
        Debug.Log($"EndTurn(isPlayerTurn = {isPlayerTurn})");
        

        if (!isPlayerTurn)
        {
            Debug.Log("Calling OpponentTurn()");
            StartCoroutine(OpponentTurn());
        }
        else
        {
            Debug.Log($"PlayerTurn = {isPlayerTurn}");
            isCardDrawn = !isCardDrawn;
        }
    }

    IEnumerator OpponentTurn()
    {

        if (isPlayerTurn == false)
        {
            yield return new WaitForSeconds(1f);
            //random chance to get card from discardPile istead of drawing pile
            int rand = Random.Range(0, 4);
            if (rand > 0)
            {
                DealCard(opponentHand, CardOwner1.Opponent).GetComponent<Card1>().FlipCard();
            } 
            else
            {
                Transform cardFromDiscard = discardPile.GetChild(discardPile.childCount - 1);
                cardFromDiscard.SetParent(opponentHand);
                cardFromDiscard.GetComponent<Card1>().Owner = CardOwner1.Opponent;
            }

            
           
            yield return new WaitForSeconds(0.5f);

            if (opponentHand.childCount > 0)
            {
                Transform cardToDiscard = opponentHand.GetChild(Random.Range(0, opponentHand.childCount));//gets one random card of opponents hand
                cardToDiscard.SetParent(discardPile);
                cardToDiscard.SetAsLastSibling();

                foreach (Card1 child in discardPile.GetComponentsInChildren<Card1>())
                {
                    child.GetComponent<Image>().CrossFadeAlpha(0.75f, 0.3f, true);
                    child.Owner = CardOwner1.None;  
                    if (child.transform.GetSiblingIndex() == discardPile.transform.childCount - 1)
                    {
                        child.GetComponent<Image>().raycastTarget = true;
                        child.GetComponent<Image>().CrossFadeAlpha(1f, 0, true);

                    }
                    else
                    {
                        child.GetComponent<Image>().raycastTarget = false;
                    }

                }
            }

            CheckVictory(opponentHand); //on opponents turn check for victory before discarding because opponent discards randomly;
            EndTurn();
        }
    }

    public void CheckVictory(Transform hand)
    {
        
        
        int sets = CountValidSets(hand);

        Debug.Log($"CheckVictory({hand}) it has {sets} sets.");
        if (sets == 3)
        {
            Debug.Log($"{hand} won!");
            if(hand == playerHand)
            {
                DisplayVictory();
            }
            else
            {
                DisplayDefeat();
            }
        }
        else
        {
            Debug.Log("No Winner");
        }
    }

    int CountValidSets(Transform hand)
    {
        List<Card1> cards = new List<Card1>();

        foreach (Transform child in hand)
        {
            Card1 card = child.GetComponent<Card1>();
            if (card != null)
            {
                cards.Add(card);
            }
        }

        //foreach (Card1 card in cards)
        //{
        //    Debug.Log($"{card.name}");
        //}

        Debug.Log($"CountValidSets({hand}) has {cards.Count} cards.)");
        int sets = 0;
        for (int i = 0; i < cards.Count - 2; i++)
        {
            for (int j = 0; j < cards.Count - 1; j++)
            {
                for(int k = 0; k < cards.Count; k++)
                {
                    if (IsTriad(cards[i], cards[j], cards[k]) || IsSequence(cards[i], cards[j], cards[k]))
                    {
                        cards[i] = null;
                        cards[j] = null;
                        cards[k] = null;
                        sets++;
                    }
                }
            }
        }

        return sets;    

    }

    bool IsTriad(Card1 c1, Card1 c2, Card1 c3)
    {
        //cards that are part of a set need to be removed from cards list
        if (c1 == null || c2 == null || c3 == null) return false;

        if (c1.CardValue == c2.CardValue && c1.CardValue == c3.CardValue &&
            c1.CardSuit != c2.CardSuit && c1.CardSuit != c3.CardSuit && c2.CardSuit != c3.CardSuit)
        {
            Debug.Log($"Triad {c1},{c2},{c3}");
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsSequence(Card1 c1, Card1 c2, Card1 c3)
    {
        if (c1 == null || c2 == null || c3 == null) return false;

        List<int> values = new List<int> { c1.CardValue, c2.CardValue, c3.CardValue };
        values.Sort();

        if (c1.CardSuit == c2.CardSuit && c1.CardSuit == c3.CardSuit &&
            values[1] == values[0] + 1 && values[2] == values[1] + 1)
        {
            Debug.Log($"Sequence {c1},{c2},{c3}");
            return true;
        }
        else
        {
            return false; 
        }
        
    }

    void DisplayVictory()
    {
        Debug.Log("Calling victory card");
        victoryCardUI.SetActive(true);
        StartCoroutine(FadeInResultUICard(victoryCardUI));

    }

    void DisplayDefeat()
    {
        Debug.Log("Calling defeat card");
        defeatCardUI.SetActive(true);
        StartCoroutine(FadeInResultUICard(defeatCardUI));
    }

    private IEnumerator FadeInResultUICard(GameObject cardUI)
    {
        CanvasGroup canvasGroup = cardUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null) yield break;

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;  
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1;
    }
}
