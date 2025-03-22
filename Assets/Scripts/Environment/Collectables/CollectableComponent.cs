using UnityEngine;

public class CollectableComponent : InteractiveComponent
{
    public virtual void OnCollected()
    {
        Debug.LogWarning("Generic version of OnCollected is used! Be sure to use the overriden one!");
    }

}
