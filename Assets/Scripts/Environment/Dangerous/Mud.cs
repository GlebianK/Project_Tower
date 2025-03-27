using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// œŒ ¿ Õ≈ –¿¡Œ“¿≈“, ¬»ƒ»ÃŒ, ◊“Œ-“Œ — “–»√√≈–¿Ã», Õ¿ƒŒ “≈—“»–Œ¬¿“‹ ƒ¿À‹ÿ≈
public class Mud : MonoBehaviour
{
    [SerializeField] private float frequencyOfAttacks;
    [SerializeField] private float damage;

    private bool canAttack = true;
    //private List<GameObject> objectsToDamage = new List<GameObject>();
    private List<Health> objectsToDamage;

    private void Awake()
    {
        objectsToDamage = new List<Health>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hi enemy!");
        }

        if (other.TryGetComponent<Health>(out Health healthComponent))
        {
            if (objectsToDamage.Contains(healthComponent))
            {
                return;
            }
            objectsToDamage.Add(healthComponent);

            Debug.Log($"{other.gameObject.name} is in mud zone!");
        }
        /*
        if (other.CompareTag("Player"))
        {
            ActiveDangerIcon.Invoke();
        }
        */

        /*
        else if (other.GetComponent<Health>() == null)
            Debug.LogWarning("Component Health not found");
        */
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hi enemy!");
        }
        if (objectsToDamage.Count > 0 && canAttack)
        {
            for (int i = 0; i < objectsToDamage.Count; i++)
            {
                /*
                if (objectsToDamage[i].GetComponent<Health>() != null)
                {
                    objectsToDamage[i].GetComponent<Health>().TakeDamage(damage);
                }
                else if (objectsToDamage[i].GetComponent<Health>() == null)
                    Debug.LogWarning("Component Health not found");
                */
                if (objectsToDamage[i] != null)
                    objectsToDamage[i].TakeDamage(damage);
            }
            StartCoroutine(PerformAttackCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("bye enemy!");
        }
        /*
        if (other.CompareTag("Player"))
        {
            DeactiveDangerIcon.Invoke();
        }
        */
        if (other.TryGetComponent<Health>(out Health healthComponent))
        {
            if (objectsToDamage.Contains(healthComponent))
            {
                objectsToDamage.Remove(healthComponent);
            }

            Debug.Log($"{other.gameObject.name} is out of the dirt");
        }
    }

    private IEnumerator PerformAttackCoroutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(frequencyOfAttacks);
        canAttack = true;
    }
}
