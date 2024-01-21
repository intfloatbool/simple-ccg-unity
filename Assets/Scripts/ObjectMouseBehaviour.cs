using System;
using UnityEngine;

public class ObjectMouseBehaviour
{
    private Func<bool> _isMouseDragEnabled;
    private Func<bool> _isMouseUpEnabled;
    private Func<Vector3> _getDefaultPosition;
    private Func<int, bool> _checkIsAlly;
    private Transform _transform;
    private int _damage;
    private IDamageable _selfDamageable;
    private ITurnable _selfTurnable;

    public ObjectMouseBehaviour(Func<bool> isMouseDragEnabled, Func<bool> isMouseUpEnabled, Transform transform, Func<Vector3> getDefaultPosition, Func<int, bool> checkIsAlly, int damage, IDamageable selfDamageable, ITurnable selfTurnable)
    {
        this._isMouseDragEnabled = isMouseDragEnabled;
        this._isMouseUpEnabled = isMouseUpEnabled;
        this._transform = transform;
        this._getDefaultPosition = getDefaultPosition;
        this._checkIsAlly = checkIsAlly;
        this._damage = damage;
        this._selfDamageable = selfDamageable;
        this._selfTurnable = selfTurnable;
    }

    public void OnMouseDrag()
    {
  
        if(this._isMouseDragEnabled() == false)
        {
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        this._transform.position = mousePos;
    }

    public void OnMouseUp()
    {

        if (this._isMouseUpEnabled() == false)
        {
            return;
        }

        Collider2D[] collidersCollection = Physics2D.OverlapBoxAll(this._transform.position, Vector2.one * 0.05f, 2f);

        foreach (Collider2D catchedCollider in collidersCollection)
        {
            if (catchedCollider.gameObject.GetInstanceID() == this._transform.gameObject.GetInstanceID())
            {
                continue;
            }

            if(this._checkIsAlly(catchedCollider.gameObject.GetInstanceID()))
            {
                continue;
            }

            IPlayableObject playableObject = catchedCollider.GetComponent<IPlayableObject>();
            if (playableObject != null && !playableObject.IsPlayable)
            {
                continue;
            }

            IDamageable target = catchedCollider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.RecieveDamage(this._damage);

                IAttackable attackable = catchedCollider.GetComponent<IAttackable>();
                if (attackable != null)
                {
                    this._selfDamageable.RecieveDamage(attackable.GetDamage());
                }

                this._selfTurnable.OnTurnEnd();
            }

            break;
        }

        this._transform.position = this._getDefaultPosition();
    }
}
