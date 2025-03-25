using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //public static GameManager Instance;

    //public Transform drawPile;
    //public Transform discardPile;
    //public Transform playerHand;
    //public Transform opponentHand;

    //private Stack<GameObject> deck = new Stack<GameObject>();
    //public bool isPlayerTurn = true;

    //void Awake()
    //{
    //    if (Instance == null) Instance = this;
    //    else Destroy(gameObject);
    //}

    //void Start()
    //{
    //    StartGame();
    //}

    //void StartGame()
    //{
    //    DealInitalHands();
    //    StartTurn();
    //}

    //void DealInitalHands()
    //{
    //    for (int i = 0; i < 9; i++)
    //    {
    //        DrawCard(playerHand);
    //        DrawCard(opponentHand);
    //    }
    //}

    //void StartTurn()
    //{
    //    if (isPlayerTurn)
    //    {
    //        Debug.Log("Player's Turn - Draw a card from the pile");
    //    }
    //    else
    //    {
    //        StartCoroutine(AI_Turn());
    //    }
    //}

    //public void DrawCard(Transform hand)
    //{
    //    if (deck.Count == 0) return;

    //    GameObject drawCard = deck.Pop();
    //    drawCard.transform.SetParent(hand, false);
    //    drawCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    //}

    //public void DiscardCard(GameObject card)
    //{
    //    card.transform.SetParent(discardPile, false);
    //    card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    //    card.GetComponent<Image>().raycastTarget = false;

    //    EndTurn();
    //}

    //void EndTurn()
    //{
    //    if (CheckForWin(playerHand))
    //    {
    //        Debug.Log("Player Wins!");
    //        return;
    //    }
    //    else if (CheckForWin(opponentHand))
    //    {
    //        Debug.Log("Opponent Wins!");
    //        return;
    //    }


    //    isPlayerTurn = !isPlayerTurn;
    //    StartTurn();
    //}

    //IEnumerator AI_Turn()
    //{
    //    yield return new WaitForSeconds(1f);
    //    DrawCard(opponentHand);

    //    yield return new WaitForSeconds(0.5f);
    //    DiscardCard(opponentHand.GetChild(0).gameObject);
    //}

    //bool CheckForWin(Transform hand)
    //{
    //    List<Card> cards = new List<Card>();
    //    foreach (Transform child in hand)
    //    {
    //        cards.Add(child.GetComponent<Card>());
    //    }

    //    return ValidateSets(cards);
    //}

    //bool ValidateSets(List<Card> cards)
    //{
    //    if (cards.Count != 9) return false;

    //    List<List<Card>> possibleSets = new List<List<Card>>();

    //    while (cards.Count > 0)
    //    {
    //        List<Card> potencialSet = new List<Card>();
    //        Card firstCard = cards[0];
    //        potencialSet.Add(firstCard);

    //        foreach (Card c in cards)
    //        {
    //            if (c != firstCard && (IsSameValue(firstCard, c) || IsSequential(firstCard, c)))
    //            {
    //                potencialSet.Add(c);
    //            }
    //        }

    //        if (potencialSet.Count == 3)
    //        {
    //            possibleSets.Add(potencialSet);
    //            foreach (var c in potencialSet)
    //            {
    //                cards.Remove(c);
    //            }
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }

    //    return possibleSets.Count == 3;
    //}

    //bool IsSameValue(Card c1, Card c2)
    //{
    //    return c1.CardValue == c2.CardValue && c1.CardSuit != c2.CardSuit;
    //}

    //bool IsSequential(Card c1, Card c2)
    //{
    //    return c1.CardSuit == c2.CardSuit && Mathf.Abs(c1.CardValue - c2.CardValue) == 1;
    //}

}