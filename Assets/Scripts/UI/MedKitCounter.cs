using System;
using TMPro;
using UnityEngine;

public class MedKitCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text numberField;

    private void Start()
    {
        numberField.text = "x0";
    }

    public void IncreaseAmount()
    {
        int temp = TextToNumber();
        temp++; 
        numberField.text = "x" + temp.ToString();
    }

    public void DecreaseAmount() 
    {
        int temp = TextToNumber();
        temp--;
        numberField.text = "x" + temp.ToString();
    }

    private int TextToNumber()
    {
        string temp_s = numberField.text.Remove(0, 1);
        int temp_i = 0;

        if (Int32.TryParse(temp_s, out temp_i))
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
