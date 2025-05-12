using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UseItem : MonoBehaviour
{
    [SerializeField] private Health hc;
    [SerializeField] private float hpToAdd = 5f;

    private int amountOfMedKits = 0;

    public int GetMedkitsAmount() => amountOfMedKits;

    public UnityEvent CollectItem;
    public UnityEvent UseTheItem;

    private void Awake()
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
                //Debug.Log($"hp before use: {hc.HP}");
                //Debug.Log("Can use medkit");
                hc.Heal(hpToAdd);
                amountOfMedKits--;
                UseTheItem.Invoke();
                //Debug.Log($"hp after use: {hc.HP}");
            }
            
        }  
    }

    public void SetMedkitsAmountViaCheckpointData(int amountOfKits)
    {
        amountOfMedKits = amountOfKits;
        StartCoroutine(InvokeCollectItemEventForCheckpointRestart(amountOfKits));
    }
    
    private IEnumerator InvokeCollectItemEventForCheckpointRestart(int counter)
    {
        for (int i = 1; i <= counter; i++)
        {
            CollectItem.Invoke();
            yield return null;
        }
        yield return null;
    }
}
