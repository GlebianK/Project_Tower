using System;
using TMPro;
using UnityEngine;

public class MedKitCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text numberField;

    private void Start()
    {
        numberField.text = "0";
    }

    public void IncreaseAmount()
    {
        int temp = TextToNumber();
        temp++; 
        numberField.text = temp.ToString();
    }

    public void DecreaseAmount() 
    {
        int temp = TextToNumber();
        temp--;
        numberField.text = temp.ToString();
    }

    private int TextToNumber()
    {
        string temp_s = numberField.text;

        if (Int32.TryParse(temp_s, out int temp_i))
        {
            return temp_i;
        }
        else
        {
            Debug.LogError("Something went wrong while parsing!");
            return -100;
        }
    }
}
