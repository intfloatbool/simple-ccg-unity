using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSimpleHero : MonoBehaviour
{
    [SerializeField] private TurnsManager _turnsManager;
    [SerializeField] private PlayerHero _aiHero;

    private void Awake()
    {
        this._turnsManager.OnNextTurn += HandleOnTurnOwnerChanged;
    }

    private void OnDestroy()
    {
        this._turnsManager.OnNextTurn -= HandleOnTurnOwnerChanged;
    }

    private void HandleOnTurnOwnerChanged(PlayerHero playerHero)
    {
        if(playerHero != this._aiHero)
        {
            return;
        }

        StartCoroutine(DoTurnCoroutine());
    }

    private IEnumerator DoTurnCoroutine()
    {
        bool isCardBought = false;
        foreach(Card cardInHand in new List<Card>(this._aiHero.CurrentHandCardList))
        {
            if(cardInHand.CurrentCardData.Price <= this._aiHero.CurrentDiamondCount)
            {
                this._aiHero.PutCardOnTable(cardInHand);
                isCardBought = true;
            }
        }

        float delay = isCardBought ? 1.25f : 0.5f;

        yield return new WaitForSeconds(delay);

        this._turnsManager.NextTurn();
    }
}
