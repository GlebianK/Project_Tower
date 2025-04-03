using UnityEngine;

public class HealItem : CollectableComponent
{
    [SerializeField] private float healthToAdd;

    public float GetHealthToAdd() => healthToAdd;

    public override void InteractionAction(GameObject player)
    {
        Debug.Log("Used override version of InteractionAction");
        OnCollected(player);
    }

    public override void OnCollected(GameObject player)
    {
        //добавить добавление этого айтема в коллекцию игрока
        Debug.Log("Retrieving UseItem component...");

        if(player.TryGetComponent<UseItem>(out UseItem useItemComponent))
        {
            Debug.Log("Adding med kit...");
            useItemComponent.AddMedKit();
        }

        Debug.Log("Med kit added!");
        
        Destroy(gameObject);
    }
}
