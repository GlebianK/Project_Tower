using System.Collections.Generic;
using UnityEngine;

public class AttackPlayerLight : AttackPlayerBase
{
    public override void DeactivateParentGameObject()
    {
        gameObject.SetActive(false);
    }
}
