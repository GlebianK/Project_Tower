using UnityEngine;
using UnityEngine.Events;

public class HealItem : CollectableComponent
{
    public UnityEvent AddMedKit;

    [SerializeField] private float healthToAdd; //�������� 

    public float GetHealthToAdd() => healthToAdd;

    public override void InteractionAction()
    {
        Debug.Log("Used override version of InteractionAction");
        OnCollected();
    }

    public override void OnCollected()
    {
        //�������� ���������� ����� ������ � ��������� ������
        Debug.Log("Adding med kit...");
        AddMedKit.Invoke();
        Destroy(gameObject);
        Debug.Log("Med kit added!");
    }
}
