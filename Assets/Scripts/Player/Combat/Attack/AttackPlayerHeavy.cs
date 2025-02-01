using System.Collections.Generic;
using UnityEngine;

public class AttackPlayerHeavy : AttackPlayerBase
{
    protected override void DeactivateParentGameObject() // см. класс AttackPlayerBase
    {
        gameObject.SetActive(false);
    }

    public override void OnAttackEnded()
    {
        base.OnAttackEnded();
        DeactivateParentGameObject();
    }
}
