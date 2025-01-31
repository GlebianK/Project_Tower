using System.Collections.Generic;
using UnityEngine;

public class AttackPlayerLight : AttackPlayerBase
{
    protected override void DeactivateParentGameObject() // ��. ����� AttackPlayerBase
    {
        gameObject.SetActive(false);
    }

    public override void OnAttackEnded()
    {
        base.OnAttackEnded();
        DeactivateParentGameObject();
    }
}
