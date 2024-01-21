using System;
using UnityEngine;

public class TurnsManager : MonoBehaviour
{
    [SerializeField] private PlayerHero[] _heroesOnScene;
    private int _currentPlayerIndex = -1;
    private PlayerHero _currentTurnHero;

    public event Action<PlayerHero> OnNextTurn;

    private void Awake()
    {
        NextTurn();
    }

    public void NextTurn()
    {
        if(this._currentTurnHero != null)
        {
            this._currentTurnHero.OnTurnStateChanged(false);
        }

        this._currentPlayerIndex++;

        if(this._currentPlayerIndex >= this._heroesOnScene.Length)
        {
            this._currentPlayerIndex = 0;
        }

        PlayerHero hero = this._heroesOnScene[this._currentPlayerIndex];

        hero.SetDiamondCount(hero.CurrentDiamondCount + 1);

        this._currentTurnHero = hero;
        this._currentTurnHero.OnTurnStateChanged(true);

        this.OnNextTurn?.Invoke(this._currentTurnHero);
    }
}
