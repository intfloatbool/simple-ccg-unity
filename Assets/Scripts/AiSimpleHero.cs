using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AiSimpleHero : MonoBehaviour
{
    [SerializeField] private TurnsManager _turnsManager;
    [SerializeField] private PlayerHero _aiHero;
    [SerializeField] private PlayerHero _enemyHero;

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
                cardInHand.OnTurnStart();
                isCardBought = true;
            }
        }

        float delay = isCardBought ? 1.25f : 0.5f;

        yield return new WaitForSeconds(delay);

        Card cardToAttack = default;

        foreach (Card cardInDeck in this._aiHero.CurrentDeckCardList)
        {
            cardToAttack = this._enemyHero.CurrentDeckCardList.FirstOrDefault();
            if (cardToAttack && !cardToAttack.IsDead())
            {
                cardInDeck.Attack(cardToAttack);
                cardInDeck.RecieveDamage(cardToAttack.GetDamage());
            }
            else
            {
                cardInDeck.Attack(this._enemyHero);
                cardInDeck.RecieveDamage(this._enemyHero.GetDamage());
            }

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(delay);

        cardToAttack = this._enemyHero.CurrentDeckCardList.FirstOrDefault();
        if (cardToAttack && !cardToAttack.IsDead())
        {
            this._aiHero.Attack(cardToAttack);
            this._aiHero.RecieveDamage(cardToAttack.GetDamage());
        }
        else
        {
            this._aiHero.Attack(this._enemyHero);
            this._aiHero.RecieveDamage(this._enemyHero.GetDamage());
        }

        this._aiHero.OnTurnEnd();

        yield return new WaitForSeconds(delay);

        this._turnsManager.NextTurn();
    }
}
