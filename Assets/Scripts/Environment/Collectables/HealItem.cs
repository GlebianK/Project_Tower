using UnityEngine;

public class HealItem : CollectableComponent
{
    [SerializeField] private float healthToAdd;

    public float GetHealthToAdd() => healthToAdd;

    public override void InteractionAction()
    {
        Debug.Log("Used override version of InteractionAction");
        OnCollected();
    }

    public override void OnCollected()
    {
        //добавить добавление этого айтема в коллекцию игрока
        Debug.Log("Adding med kit...");
        Destroy(gameObject);
        Debug.Log("Med kit added!");
    }
}
