using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BalanceBar : MonoBehaviour
{
    private Slider Slider;

    private void Awake()
    {
        Slider = GetComponent<Slider>();
    }


    public void setValue(int MaxHealth)
    {
        Slider.maxValue = MaxHealth;
    }

    public void UpdateValue()
    {
        Slider.value = Slider.maxValue;
    }

    public void UpdateValue(int ValueChange)
    {
        if (ValueChange < Slider.maxValue && ValueChange >= 0)
        {
            Slider.value = ValueChange;
        }
        else if (ValueChange < 0)
        {
            Slider.value = 0;
        }
        else
        {
            UpdateValue();
        }
    }

    public void UpdateMaxValue(int MaxValueIncrease)
    {
        Slider.maxValue += MaxValueIncrease;
    }

    public void HideBar()
    {
        transform.parent.gameObject.SetActive(false);
    }
    
    public void HideBar(bool status)
    {
        transform.parent.gameObject.SetActive(status);
    }
}
