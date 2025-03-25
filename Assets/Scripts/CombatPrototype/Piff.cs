using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piff : MonoBehaviour
{
    public Image[] cardFaces;
    public GameObject cardPrefab;

    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

    public List<string> deck;

    void Start()
    {
        PlayPiff();
    }

    
    void Update()
    {
        
    }

    public void PlayPiff()
    {

        deck = GenerateDeck();
        ShuffleDeck();
        foreach (string card in deck)
        {
            Debug.Log(card);
        }


        DealCards();
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            string temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string s in suits)
        {
            foreach (string v in values)
            {
                newDeck.Add(s + v);
            }
        }
        return newDeck;
    }

    public void DealCards()
    {
        for (int i = 0; i < cardFaces.Length; i++)
        {
            GameObject card = Instantiate(cardPrefab, cardFaces[i].transform);
            card.transform.localPosition = Vector3.zero;
            card.transform.localRotation = Quaternion.identity;
            card.transform.localScale = Vector3.one;
            card.GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/" + deck[i]);
        }
    }
}
