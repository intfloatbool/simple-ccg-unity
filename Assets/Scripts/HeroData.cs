using UnityEngine;

[CreateAssetMenu(fileName = "HeroData", menuName = "Heroes/HeroData")]
public class HeroData : ScriptableObject
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
}
