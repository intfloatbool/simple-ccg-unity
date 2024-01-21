using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class Card : StateableObjectBase, IDamageable, IAttackable, IPlayableObject
{
    [Space(5f)]
    [SerializeField] private GameObject _playVisualState;
    [SerializeField] private GameObject _deathVisualState;
    [SerializeField] private GameObject _hideVisualState;
    [SerializeField] private GameObject _readyVisualState;

    [Space(5f)]
    [SerializeField] private CardData _defaultCardData;

    [Space(5f)]
    [SerializeField] private SpriteRenderer[] _sprites;
    [SerializeField] private TextMeshPro _priceText;
    [SerializeField] private TextMeshPro _damageText;
    [SerializeField] private TextMeshPro _healthText;

    [Space(5f)]
    [SerializeField] private ECardGameplayState _cardState;

    private CardData _currentCardData;
    public CardData CurrentCardData => _currentCardData;

    private Action<Card> _onClick;
    public event Action<Card> OnDeath;

    private bool _isControllable;

    private Vector3 _defaultPosition;

    private int _currentHealth;

    private bool _isDead = false;

    public bool IsPlayable { get; set; } = false;

    private ObjectMouseBehaviour _objectMouseBehaviour;

    public enum ECardGameplayState : byte
    {
        NONE, ON_HAND, ON_DECK
    }

    protected override void Awake()
    {
        base.Awake();

        if(this._defaultCardData != null)
        {
            SetCardData(this._defaultCardData);
        }
    }


    public void Attack(IDamageable damageable)
    {
        damageable.RecieveDamage(this.CurrentCardData.Damage);
    }

    public void SetDefaultPosition(Vector3 pos)
    {
        this._defaultPosition = pos;
    }

    public void SetIsControllable(bool isControllable)
    {
        this._isControllable = isControllable;
    }

    public void SetCardGameplayState(ECardGameplayState cardState)
    {
        this._cardState = cardState;
    }

    public void SetCardData(CardData cardData)
    {
        foreach(var spriteRend in this._sprites)
        {
            spriteRend.sprite = cardData.Icon;
        }

        this._healthText.text = cardData.Health.ToString();
        this._damageText.text = cardData.Damage.ToString();
        this._priceText.text = cardData.Price.ToString();

        this._currentCardData = cardData;
        this._currentHealth = cardData.Health;

        this._objectMouseBehaviour = new ObjectMouseBehaviour(
            () => !this._isDead && this._isControllable,
            () => !this._isDead && this._isControllable,    
            this.transform,
            () => this._defaultPosition,
            (id) => false,
            this.GetDamage(),
            this
            );
    }

    protected override List<GameObject> GetAllPossibleStates()
    {
        return new List<GameObject>
        {
            this._playVisualState, this._deathVisualState, this._hideVisualState, this._readyVisualState
        };
    }

    public void SetOnClickMethod(Action<Card> onClick)
    {
        this._onClick = onClick;
    }


    private void OnMouseDown()
    {
        if (this._isDead)
        {
            return;
        }

        this._onClick?.Invoke(this);
    }

    private void OnMouseDrag()
    {
        this._objectMouseBehaviour.OnMouseDrag();
    }

    private void OnMouseUp()
    {
        this._objectMouseBehaviour.OnMouseUp();
    }

    public void RecieveDamage(int damage)
    {
        if(this._isDead)
        {
            return;
        }

        this._currentHealth -= damage;
        this._currentHealth = Math.Clamp(this._currentHealth, 0, int.MaxValue);

        if(this._currentHealth == 0)
        {
            SetStateByName("DEATH_STATE");
            StartCoroutine(DeathCoroutine());
            this._isDead = true;
        }

        this._healthText.SetText(this._currentHealth.ToString());
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        this.OnDeath?.Invoke(this);
        Destroy(this.gameObject);
    }

    public int GetDamage()
    {
        return this._currentCardData.Damage;
    }
}
