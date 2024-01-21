using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerHero : StateableObjectBase, IDamageable, IAttackable, ITurnable
{
    [SerializeField] private GameObject _actionState;
    [SerializeField] private GameObject _deathState;

    [Space(5f)]
    [SerializeField] private SpriteRenderer[] _iconSpriteRenderers;
    [SerializeField] private TextMeshPro _attackText;
    [SerializeField] private TextMeshPro _hpText;
    [SerializeField] private TextMeshPro _crystalText;
    [SerializeField] private SpriteRenderer _turnIndicatorSprite;

    [Space(5f)]
    [SerializeField] private HeroData _defaultHeroData;

    [Space(5f)]
    [SerializeField] private CardPositioningLogic _handCardPositioningLogic;
    [SerializeField] private CardPositioningLogic _deckCardPositioningLogic;

    [Space(5f)]
    [SerializeField] private int _startDiamondCount = 1;
    [SerializeField] private string _cardInHandState = "READY_STATE";

    private int _currentDiamondCount;
    public int CurrentDiamondCount => _currentDiamondCount;

    private HeroData _currentHeroData;

    private LinkedList<Card> _currentHandCardList = new LinkedList<Card>();
    public IReadOnlyCollection<Card> CurrentHandCardList => _currentHandCardList;

    private LinkedList<Card> _currentDeckCardList = new LinkedList<Card>();
    public IReadOnlyCollection<Card> CurrentDeckCardList => _currentDeckCardList;

    private int _currentHeroHp;
    private bool _isDead;

    private ObjectMouseBehaviour _objectMouseBehaviour;
    private Vector3 _defaultPosition;
    private bool _hasTurn;

    public event Action OnDead;

    protected override void Awake()
    {
        base.Awake();

        if (_defaultHeroData != null)
        {
            SetHeroData(_defaultHeroData);
        }

        SetDiamondCount(this._startDiamondCount);
    }

    private void Start()
    {
        this._defaultPosition = this.transform.position;
    }

    public void SetDiamondCount(int diamondCount)
    {
        this._currentDiamondCount = diamondCount;

        this._crystalText.SetText(this._currentDiamondCount.ToString());
    }

    protected override List<GameObject> GetAllPossibleStates()
    {
        return new List<GameObject>
        { _actionState, _deathState  };
    }

    public void SetHeroData(HeroData heroData)
    {
        foreach (var spriteRend in _iconSpriteRenderers)
        {
            spriteRend.sprite = heroData.Icon;
        }

        this._attackText.SetText(heroData.Damage.ToString());
        this._hpText.SetText(heroData.Health.ToString());

        this._currentHeroData = heroData;

        this._currentHeroHp = this._currentHeroData.Health;

        this._objectMouseBehaviour = new ObjectMouseBehaviour(
            () => !this._isDead && this == ObjectByTurnOwnerController.LocalPlayerHero && this._hasTurn,
            () => !this._isDead && this == ObjectByTurnOwnerController.LocalPlayerHero && this._hasTurn,
            transform,
            () => this._defaultPosition,
            (id) =>
            {
                return this._currentDeckCardList.Any(c => c.gameObject.GetInstanceID() == id) || this._currentHandCardList.Any(c => c.gameObject.GetInstanceID() == id);
            },
            this._currentHeroData.Damage,
            this,
            this
            );
    }

    #region CARDS_MANIPULATION

    private void AddCardInternal(Card card, ICollection<Card> cardCollection, CardPositioningLogic positioningLogic, string cardState)
    {
        int cardIndex = cardCollection.Count;

        Vector3 pos = positioningLogic.CalculatePosition(cardIndex);
        card.transform.position = pos;
        card.SetDefaultPosition(pos);

        card.name = $"CARD_{card.CurrentCardData.name}_{cardIndex}";

        card.SetStateByName(cardState);

        cardCollection.Add(card);
    }

    public void AddCardInHand(Card card)
    {
        AddCardInternal(card, this._currentHandCardList, this._handCardPositioningLogic, this._cardInHandState);
        card.SetOnClickMethod((clickedCard) =>
        {
            PutCardOnTable(clickedCard);
            clickedCard.OnTurnStart();
        });
    }

    public void RemoveCardFromHand(Card card)
    {
        this._currentHandCardList.Remove(card);
    }


    public void PutCardOnTable(Card card, bool isCheckCrystals = true)
    {
        if (isCheckCrystals && this._currentDiamondCount < card.CurrentCardData.Price)
        {
            return;
        }


        AddCardInternal(card, this._currentDeckCardList, this._deckCardPositioningLogic, "PLAY_STATE");
        card.SetOnClickMethod(null);

        this._currentHandCardList.Remove(card);
        ResetCardsInHandPositions();

        if (isCheckCrystals)
            SetDiamondCount(this._currentDiamondCount - card.CurrentCardData.Price);

        IEnumerator waitAndSetControllable()
        {
            yield return new WaitForSeconds(0.2f);
            card.SetIsControllable(this == ObjectByTurnOwnerController.LocalPlayerHero);
        }

        StartCoroutine(waitAndSetControllable());

        card.OnDeath -= OnCardDeath;
        card.OnDeath += OnCardDeath;

        card.IsPlayable = true;
    }

    private void OnCardDeath(Card deathCard)
    {
        this._currentDeckCardList.Remove(deathCard);
        ResetCardsInDeckPositions();
    }

    private void ResetCardsInDeckPositions()
    {
        List<Card> tempList = new List<Card>(this._currentDeckCardList);
        this._currentDeckCardList.Clear();
        this._deckCardPositioningLogic.Reset();

        foreach (Card card in tempList)
        {
            PutCardOnTable(card, false);
        }
    }

    private void ResetCardsInHandPositions()
    {
        List<Card> tempList = new List<Card>(this._currentHandCardList);
        this._currentHandCardList.Clear();
        this._handCardPositioningLogic.Reset();

        foreach (Card card in tempList)
        {
            AddCardInHand(card);
        }
    }

    public void Attack(IDamageable damageable)
    {
        damageable.RecieveDamage(this._currentHeroData.Damage);
    }

    public void Death()
    {
        //TODO:
    }

    #endregion

    public void OnTurnStateChanged(bool isMyTurn)
    {
        this._turnIndicatorSprite.gameObject.SetActive(isMyTurn);
        
        if(isMyTurn)
        {
            foreach (Card cardOnDeck in this._currentDeckCardList)
            {
                cardOnDeck.OnTurnStart();
            }

            OnTurnStart();
        }
    }

    public void RecieveDamage(int damage)
    {
        if (this._isDead)
        {
            return;
        }

        this._currentHeroHp -= damage;
        this._currentHeroHp = Mathf.Clamp(this._currentHeroHp, 0, int.MaxValue);

        this._hpText.SetText(this._currentHeroHp.ToString());

        this._isDead = this._currentHeroHp <= 0;

        if (this._isDead)
        {
            SetStateByName("DEATH_STATE");

            this.OnDead?.Invoke();
        }
    }

    public int GetDamage()
    {
        return this._currentHeroData.Damage;
    }

    private void OnMouseDrag()
    {
        this._objectMouseBehaviour.OnMouseDrag();
    }

    private void OnMouseUp()
    {
        this._objectMouseBehaviour.OnMouseUp();
    }

    public void OnTurnStart()
    {
        this._hasTurn = true;
    }

    public void OnTurnEnd()
    {
        this._hasTurn = false;
    }

    public bool IsDead()
    {
        return this._isDead;
    }
}
