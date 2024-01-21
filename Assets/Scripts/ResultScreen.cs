using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private PlayerHero _localPlayerHero;
    [SerializeField] private PlayerHero _enemyPlayerHero;

    [Space(5f)]
    [SerializeField] private GameObject _resultContainer;
    [SerializeField] private TextMeshProUGUI _resultScreenText;
    [SerializeField] private Button _startAgainButton;
    [SerializeField] private string _sceneToLoad;

    private void Awake()
    {
        this._localPlayerHero.OnDead += OnLocalPlayerHeroDead;
        this._enemyPlayerHero.OnDead += OnEnemyHeroDead;

        this._startAgainButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(this._sceneToLoad);
        });
    }

    private void OnDestroy()
    {
        this._localPlayerHero.OnDead -= OnLocalPlayerHeroDead;
        this._enemyPlayerHero.OnDead -= OnEnemyHeroDead;
    }

    private void OnLocalPlayerHeroDead()
    {
        this._resultContainer.SetActive(true);
        this._resultScreenText.SetText("You died");
        this._resultScreenText.color = Color.red;
    }

    private void OnEnemyHeroDead()
    {
        this._resultContainer.SetActive(true);
        this._resultScreenText.SetText("You won");
        this._resultScreenText.color = Color.green;
    }
}
