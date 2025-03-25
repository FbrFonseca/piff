using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardOwner { Player, Opponent, None }

public class CardGame : MonoBehaviour
{
    public Transform cardPile; // The pile of cards to draw from
    public Transform playerHand;
    public Transform opponentHand;
    public Transform discardPile;

    public Sprite[] sprites;
    public Sprite cardBackSprite;
    public GameObject cardPrefab;

    public bool isPlayerTurn = true;
    private int playerSets = 0;
    private int opponentSets = 0;
    private int setsToWin = 3;

    private Stack<GameObject> deck = new Stack<GameObject>(); // suffled deck


    void Start()
    {
        GenerateDeck();
        ShuffleDeck();
        SpawnDeck();
        StartCoroutine(DealCards());

        //foreach (Transform card in cardPile)
        //{
        //    print(card.GetComponent<Image>().sprite.name);
        //}
    }

    // Generate a deck of 52 cards 4 suits and 13 values
    void GenerateDeck()
    {
        string[] suits = new string[] { "Clubs", "Diamonds", "Hearts", "Spades" };
        string[] values = new string[] { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };

        int spriteIndex = 0;


        for (int suit = 0; suit < 4; suit++)
        {
            for (int value = 0; value < 13; value++)
            {

                string cardName = values[value] + "," + suits[suit];

                GameObject newCard = Instantiate(cardPrefab, cardPile);
                newCard.name = cardName;

                Card cardScript = newCard.AddComponent<Card>();
                cardScript.Setup(sprites[spriteIndex], cardBackSprite, suits[suit], value + 1, CardOwner.None);

                newCard.transform.SetParent(cardPile, false);
                newCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                newCard.transform.SetAsLastSibling();

                deck.Push(newCard); // maybe reverse order
                spriteIndex++;
            }
        }
    }

    // Shuffle the deck swapping each card with another random card
    void ShuffleDeck()
    {
        GameObject[] shuffleCards = deck.ToArray();

        for (int i = shuffleCards.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffleCards[i], shuffleCards[j]) = (shuffleCards[j], shuffleCards[i]); // Swap elements
        }

        deck.Clear();

        foreach (var card in shuffleCards)
        {
            deck.Push(card);
            card.transform.SetAsLastSibling();
        }
    }

    // Spawn the deck in a pile
    void SpawnDeck()
    {
        int offsetX = 0;
        foreach (Transform card in cardPile)
        {
            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(offsetX, 0);
            offsetX += 5; // Stack effect
        }
    }

    IEnumerator DealCards()
    {

        for (int i = 0; i < 9; i++)
        {
            DealCard(playerHand, CardOwner.Player);
            yield return new WaitForSeconds(0.05f);

            DealCard(opponentHand, CardOwner.Opponent);
            yield return new WaitForSeconds(0.05f);


            //GameObject dealtCard = DealCard(playerHand, CardOwner.Player);
            //if (dealtCard != null)
            //{
            //    yield return new WaitForSeconds(0.05f); //animation delay
            //    dealtCard.GetComponent<Card>().FlipCard();
            //}

            //if (deck.Count > 0)
            //{
            //    GameObject opponentCard = DealCard(opponentHand, CardOwner.Opponent);
            //    opponentCard.GetComponent<Card>().FlipCard();
            //    yield return new WaitForSeconds(0.05f); //animation delay
            //}

        }
    }

    GameObject DealCard(Transform hand, CardOwner owner)
    {
        
        if (deck.Count == 0) return null;

        GameObject card = deck.Pop();
        card.transform.SetParent(hand, false);
        card.GetComponent<Card>().Owner = owner;
        card.GetComponent<Card>().FlipCard();

        return card;
    }


    IEnumerator OpponentTurn()
    {

        yield return new WaitForSeconds(1f);
        DealCard(opponentHand, CardOwner.Opponent);
        yield return new WaitForSeconds(0.5f);
        if (opponentHand.childCount > 0)
        {
            Transform cardToDiscart = opponentHand.GetChild(Random.Range(0, opponentHand.childCount));
            DiscardCard(cardToDiscart, discardPile);
        }
        

        //if (deck.Count > 0)
        //{
        //    GameObject card = DealCard(opponentHand, CardOwner.Opponent);

        //    //card.GetComponent<Card>().FlipCard();
        //    yield return new WaitForSeconds(0.5f);
        //}

        //if (opponentHand.childCount > 0)
        //{
        //    Transform cardToDiscard = opponentHand.GetChild(Random.Range(0, opponentHand.childCount));

           
        //    DiscardCard(cardToDiscard, discardPile);

        //}

        EndTurn();
    }

    void DiscardCard(Transform cardTransform, Transform discardPile)
    {
        cardTransform.SetParent(discardPile, false);
        cardTransform.GetComponent<Card>().Owner = CardOwner.None;

        //Flip the card when discarding(if needed)
        
        
        cardTransform.GetComponent<Card>().FlipCard();
        
    }

    public void EndTurn()
    {
        
        if (CheckForVictory())
        {
            Debug.Log(isPlayerTurn ? "Player Wins!" : "Opponent Wins!");
            return;
        }

        isPlayerTurn = !isPlayerTurn;

        if (!isPlayerTurn)
        {
            StartCoroutine(OpponentTurn());
        }

    }

    private bool CheckForVictory()
    {
        int sets = CountValidateSets(isPlayerTurn ? playerHand : opponentHand);

        if (isPlayerTurn)
        {
            playerSets = sets;
        }
        else
        {
            opponentSets = sets;
        }

        return sets >= setsToWin;
    }

    private int CountValidateSets(Transform hand)
    {
        List<Card> cards = new List<Card>();

        foreach (Transform child in hand)
        {
            Card card = child.GetComponent<Card>();
            if (card != null)
            {
                cards.Add(card);
            }
        }

        int sets = 0;

        for (int i = 0; i < cards.Count - 2; i++)
        {
            for (int j = i + 1; j < cards.Count - 1; j++)
            {
                for (int k = j + 1; k < cards.Count; k++)
                {
                    if (IsSameValue(cards[i], cards[j], cards[k]) || IsSequential(cards[i], cards[j], cards[k]))
                    {
                        sets++;
                    }
                }
            }
        }

        return sets;
    }

    private bool IsSameValue(Card c1, Card c2, Card c3)
    {

        return c1.CardValue == c2.CardValue && c1.CardValue == c3.CardValue &&
            c1.CardSuit != c2.CardSuit && c1.CardSuit != c3.CardSuit && c2.CardSuit != c3.CardSuit;
    }

    private bool IsSequential(Card c1, Card c2, Card c3)
    {
        List<int> values = new List<int> { c1.CardValue, c2.CardValue, c3.CardValue };
        values.Sort();

        return c1.CardSuit == c2.CardSuit && c1.CardSuit == c3.CardSuit &&
            values[1] == values[0] + 1 && values[2] == values[1] + 1;
    }

    void LogPlayerHand()
    {
        int counter = 0;    
        string handContents = "Player hand: ";
        foreach (Transform card in playerHand)
        {
            counter++;
            handContents += card.name + ", ";
        }
        Debug.Log(handContents + "Total of: " + counter);
    }
}