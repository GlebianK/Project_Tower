using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UseItem : MonoBehaviour
{
    [SerializeField] private Health hc;
    [SerializeField] private float hpToAdd = 5f;

    private int amountOfMedKits = 0;

    public UnityEvent CollectItem;
    public UnityEvent UseTheItem;

    private void Start()
    {
        amountOfMedKits = 0;
    }

    public void AddMedKit()
    {
        amountOfMedKits++;
        CollectItem.Invoke();
    }

    public void OnUseMedKit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (amountOfMedKits > 0 && hc.HP < hc.MaxHP)
            {
                Debug.Log($"hp before use: {hc.HP}");
                Debug.Log("Can use medkit");
                hc.Heal(hpToAdd);
                amountOfMedKits--;
                UseTheItem.Invoke();
                Debug.Log($"hp after use: {hc.HP}");
            }
            
        }  
    }
}
