using System.Collections.Generic;
using UnityEngine;

public abstract class StateableObjectBase : MonoBehaviour
{
    [SerializeField] protected GameObject _defaultState;

    protected List<GameObject> _allStates;
        
    protected virtual void Awake()
    {
        this._allStates = GetAllPossibleStates(); 

        SetStateByName(this._defaultState.name);
    }

    protected abstract List<GameObject> GetAllPossibleStates();

    public void SetStateByName(string stateName)
    {
        foreach (var state in this._allStates)
        {
            state.SetActive(state.name.Equals(stateName));
        }
    }
}
