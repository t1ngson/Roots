using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeController : MonoBehaviour
{
    [SerializeField] GameObject drillUpgrade;
    [SerializeField] GameObject glassesUpgrade;
    [SerializeField] GameObject rainUpgrade;
    [SerializeField] GameObject skatesUpgrade;
    [SerializeField] GameObject waterBottleUpgrade;
    [SerializeField] GameObject waterCanUpgrade;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void increaseSlider(GameObject gameObject){
        Slider slider = gameObject.transform.GetChild(0).GetComponent<Slider>();
        slider.value = slider.value +1;
        TextMeshProUGUI tmp = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        tmp.text = (slider.value).ToString();
    }
}
