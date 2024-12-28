using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;

public class Attack : MonoBehaviour
{
    public UnityEvent AttackStarted;
    public UnityEvent AttackEnded;

    [SerializeField] private float damage;
    [SerializeField] private float rangeAttack;
    [SerializeField] private float damageDealingDelay;
    [SerializeField] private float durationOfTheAttack;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform AttackRaycastPointPosition;
    private bool canAttack = true;
    #region
    public bool CanAttack() => canAttack;
    public void TryAttack()
    {
        if(CanAttack())
        StartCoroutine(PerformAttackCoroutine());
    }
    #endregion
    private IEnumerator PerformAttackCoroutine()
    {
        canAttack = false;
        AttackStarted.Invoke();
        Health health;
        RaycastHit hit;
        Ray ray;
        yield return new WaitForSeconds(damageDealingDelay);
        ray = new Ray(AttackRaycastPointPosition.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * rangeAttack, Color.cyan);
        if (Physics.Raycast(ray, out hit, rangeAttack, layerMask))
        {
            health = hit.transform.GetComponent<Health>();
            health.TakeDamage(damage);
        }
        yield return new WaitForSeconds(durationOfTheAttack);
        AttackEnded.Invoke();
        canAttack = true;
    }

}
