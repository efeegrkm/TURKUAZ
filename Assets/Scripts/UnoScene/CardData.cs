using System;
using UnityEngine;

public enum CardColor { Red, Green, Blue, Yellow, Wild }
public enum CardType { Number, Skip, Reverse, DrawTwo, Wild, WildDrawFour }

[Serializable]
public class CardData
{
    public CardColor color;
    public CardType type;
    public int number; // Sadece Number tipinde kullanýlacak
}
