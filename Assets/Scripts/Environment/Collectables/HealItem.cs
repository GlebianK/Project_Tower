using UnityEngine;

public class HealItem : CollectableComponent
{
    [SerializeField] private float healthToAdd;

    public float GetHealthToAdd() => healthToAdd;
    /*
    public override void Interact()
    {
        Debug.Log("Used override version");
    }
    public override void Deinteract()
    {
        Debug.Log("Used override version");
    }
    */
    public override void InteractionAction()
    {
        Debug.Log("Used override version");
        OnCollected();
    }
    
}
