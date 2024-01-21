using UnityEngine;

public class ObjectByTurnOwnerController : MonoBehaviour
{
    [SerializeField] private TurnsManager _turnsManager;
    [SerializeField] private PlayerHero _ownerHero;
    [SerializeField] private GameObject[] _gameObjectsToControlCollection;

    public static PlayerHero LocalPlayerHero { get; private set; }

    private void Awake()
    {
        LocalPlayerHero = this._ownerHero;
        this._turnsManager.OnNextTurn += HandleOnNextTurn;
    }

    private void OnDestroy()
    {
        this._turnsManager.OnNextTurn -= HandleOnNextTurn;
    }

    private void HandleOnNextTurn(PlayerHero playerHero)
    {
        foreach(GameObject go in this._gameObjectsToControlCollection)
        {
            go.SetActive(playerHero == _ownerHero);
        }
    }
}
