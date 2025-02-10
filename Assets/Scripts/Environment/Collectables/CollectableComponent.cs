using UnityEngine;

public class CollectableComponent : InteractiveComponent
{
    public void OnCollected()
    {
        Destroy(gameObject);
    }
}
