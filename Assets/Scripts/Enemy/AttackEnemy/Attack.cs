using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;

public class Attack : MonoBehaviour
{
    [SerializeField] private float _damageGive;
    [SerializeField] private float _rangeAttack;
    [SerializeField] private float _taimAttack;
    [SerializeField] private float _durationOfTheAttack;
    private bool _isAttacking = true;
   
    private RaycastHit hit;
    private Ray ray;
    [SerializeField] private LayerMask layerMask;

    private Health health;
    [SerializeField] private Transform AttackRaycastPointPosition;
    public UnityEvent AttackStarted;
    public UnityEvent AttackEnded;
    private void Start()
    {
        AttackRaycastPointPosition = transform.Find("AttackRaycastPointPosition");
    }
    private IEnumerator StartRaycast()
    {
        yield return new WaitForSeconds(_taimAttack);
        ray = new Ray(AttackRaycastPointPosition.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * _rangeAttack, Color.cyan);
       
        if (Physics.Raycast(ray, out hit, _rangeAttack, layerMask))
        {
            {
               
                health = GameObject.Find(hit.transform.name).GetComponent<Health>();
                health.TakeDamage(_damageGive);
                yield return new WaitForSeconds(_durationOfTheAttack);
                //_isAttacking = true;
            }
        }
        _isAttacking = true;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && _isAttacking)
        {
            _isAttacking = false;
            StartCoroutine(StartRaycast());
        }
    }
    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && _isAttacking)
        {
            _isAttacking = false;
            StartCoroutine(StartRaycast());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        _isAttacking = true;
    }




}
