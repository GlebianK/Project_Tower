using UnityEngine;

public interface ICombatAction
{
    public bool CanPerform(); // �������� ����������� ���������� ��������
    public void Perform(); // ���������� ��������
}
