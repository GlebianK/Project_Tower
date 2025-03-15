using UnityEngine;

public class CollectableComponent : InteractiveComponent
{
    /*
    public override void Interact()
    {
        Debug.Log("Used override version");
    }
    public override void Deinteract()
    {
        Debug.Log("Used override version");
    }
    
    public override void InteractionAction()
    {
        Debug.Log("Used override version");
        base.InteractionAction();
        OnCollected();
    }
    */
    public void OnCollected()
    {
        Destroy(gameObject);
    }
}
