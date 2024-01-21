using System;
using UnityEngine;

[Serializable]
public class CardPositioningLogic
{
    [SerializeField] private Transform _centralCardPosition;
    [SerializeField] private float _positionOffset;

    private int _currentCardCount = 0;

    public void Reset()
    {
        this._currentCardCount = 0;
    }

    public Vector3 CalculatePosition(int cardIndex)
    {
        bool isFirstCard = cardIndex == 0;

        Vector3 startPosition = this._centralCardPosition.position;

        if (!isFirstCard)
        {
            bool isFromRight = cardIndex % 2 == 0;
            float modifier = isFromRight ? 1f : -1f;
            if (!isFromRight)
            {
                _currentCardCount++;
            }
            startPosition += Vector3.right * _currentCardCount * _positionOffset * modifier;
        }

        return startPosition;
    }
}
