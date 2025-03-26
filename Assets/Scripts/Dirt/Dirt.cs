using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Dirt: MonoBehaviour
{
    public UnityEvent ActiveDangerIcon;
    public UnityEvent DeactiveDangerIcon;

    private bool canAttack = true;
    private List<GameObject> inTriggerList = new List<GameObject>();
    [SerializeField] private float frequencyOfAttacks;
    [SerializeField] private float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (inTriggerList.Contains(other.gameObject))
        {
            return;
        }
        inTriggerList.Add(other.gameObject);

        if (other.GetComponent<Health>() != null)
        {
            Debug.Log("Enter of the dirt");
        }
        if (other.CompareTag("Player"))
        {
            ActiveDangerIcon.Invoke();
        }
        else if (other.GetComponent<Health>() == null)
            Debug.LogWarning("Component Health not found");
    }
    private void OnTriggerStay()
    {
        if (inTriggerList.Count != 0 && canAttack)
        {
            for (int i = 0; i < inTriggerList.Count; i++)
            {
                if (inTriggerList[i].GetComponent<Health>() != null)
                {
                    inTriggerList[i].GetComponent<Health>().TakeDamage(damage);   
                }
                else if (inTriggerList[i].GetComponent<Health>() == null)
                    Debug.LogWarning("Component Health not found");
            }
            StartCoroutine(PerformAttackCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Out of the dirt");
        inTriggerList.Remove(other.gameObject);
        if (other.CompareTag("Player"))
        {
            DeactiveDangerIcon.Invoke();
        }
    }

    private IEnumerator PerformAttackCoroutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(frequencyOfAttacks);
        canAttack = true;
    }
}
