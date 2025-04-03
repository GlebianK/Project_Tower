using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ÏÎÊÀ ÍÅ ĞÀÁÎÒÀÅÒ ÏĞÎÒÈÂ ÂĞÀÃÎÂ, ÂÈÄÈÌÎ, ×ÒÎ-ÒÎ Ñ ÒĞÈÃÃÅĞÀÌÈ, ÍÀÄÎ ÒÅÑÒÈĞÎÂÀÒÜ ÄÀËÜØÅ
public class Mud : MonoBehaviour
{
    [SerializeField] private float frequencyOfAttacks;
    [SerializeField] private float damage;

    private bool canAttack = true;
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
