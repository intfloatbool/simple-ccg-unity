using UnityEngine;

public class CardDealer : MonoBehaviour
{
    [SerializeField] private Card _cardPrefab;

    [Space(5f)]
    [SerializeField] private CardData[] _possibleCardDataCollection;

    [Space(5f)]
    [SerializeField] private int _initialCardsCount = 11;

    [Space(5f)]
    [SerializeField] private PlayerHero[] _playerHeroesCollection;

    private void Start()
    {
        DealCards();
    }

    public void DealCards()
    {
        foreach(PlayerHero hero in this._playerHeroesCollection)
        {
            for(int i = 0; i < this._initialCardsCount; i++)
            {
                Card cardInstance = Instantiate(this._cardPrefab);

                CardData randomCardData = this._possibleCardDataCollection[Random.Range(0, this._possibleCardDataCollection.Length)];

                cardInstance.SetCardData(randomCardData);

                hero.AddCardInHand(cardInstance);
            }
        }
    }
}
