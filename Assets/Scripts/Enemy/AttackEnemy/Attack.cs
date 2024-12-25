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
    private Transform AttackRaycastPointPosition;
    private void Start()
    {
        AttackRaycastPointPosition = transform.Find("AttackRaycastPointPosition");
    }
    #region
    public void CanAttack(bool canAttack)
    {
        if (canAttack)
        {
            TryAttack();
        }
    }
    public void TryAttack()
    {
        StartCoroutine(PerformAttackCoroutine());
    }
    #endregion
    private IEnumerator PerformAttackCoroutine()
    {
        Health health;
        RaycastHit hit;
        Ray ray;
        yield return new WaitForSeconds(damageDealingDelay);
        ray = new Ray(AttackRaycastPointPosition.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * rangeAttack, Color.cyan);

        if (Physics.Raycast(ray, out hit, rangeAttack, layerMask))
        {
            AttackStarted.Invoke();
            AttackEnded.Invoke();
            health = hit.transform.GetComponent<Health>();
            health.TakeDamage(damage);
            AttackStarted.Invoke();
            AttackEnded.Invoke();
        }
        yield return new WaitForSeconds(durationOfTheAttack);
    }

}
