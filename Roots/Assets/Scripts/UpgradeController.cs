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
    [SerializeField] GameObject wateringCanUpgrade;
    [SerializeField] GameObject nutrientText;
    
    // Start is called before the first frame update
    void Start()
    {
        setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void increaseSlider(GameObject gameObject){
        Slider slider = gameObject.transform.GetChild(0).GetComponent<Slider>();
        slider.value = slider.value + 1;
        TextMeshProUGUI tmp = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        tmp.text = (slider.value).ToString();
    }

    public void setValues(GameObject gameObject, int num){
        //Set the Slider Value
        Slider slider = gameObject.transform.GetChild(0).GetComponent<Slider>();
        slider.value = num;

        //Set the text below the slider value
        TextMeshProUGUI tmp = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        tmp.text = (slider.value).ToString();

        //Set the cost
        calculateCost(gameObject, num);
    }


    public void calculateCost(GameObject gameObject, int num){
        TextMeshProUGUI tmp = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        if(num ==0){
            tmp.text = "10";
        }else{
            tmp.text = (num*10).ToString();
        }
    
    }

    private void setup(){
        int drillLevel = LevelController.drillUpgradeLevel;
        int glassesLevel = LevelController.visionUpgradeLevel;
        int rainLevel = LevelController.rainUpgradeLevel;
        int skatesLevel = LevelController.speedUpgradeLevel;
        int waterBottleLevel = LevelController.waterTankLevel;
        int wateringCanLevel = LevelController.wateringCanLevel;


        setValues(drillUpgrade, drillLevel);
        setValues(glassesUpgrade, glassesLevel);
        setValues(rainUpgrade, rainLevel);
        setValues(skatesUpgrade, skatesLevel);
        setValues(waterBottleUpgrade, waterBottleLevel);
        setValues(wateringCanUpgrade, wateringCanLevel);

        setNutrientCount();
    }

    public void checkNutrients(GameObject gameObject){
        TextMeshProUGUI tmp = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        int cost = int.Parse(tmp.text);
        int nutrientCount = LevelController.nutrientCount;

        if(nutrientCount >= cost){
            //call functions, setvalues and change nutrient count
            if(gameObject.name == "Drill Upgrade"){
                if(LevelController.upgradeDrill()){
                    setValues(drillUpgrade, LevelController.drillUpgradeLevel);
                    LevelController.nutrientCount = LevelController.nutrientCount - cost;
                    setNutrientCount();
                };
            }else if(gameObject.name == "Glasses Upgrade"){
                if(LevelController.upgradeVision()){
                    setValues(glassesUpgrade, LevelController.visionUpgradeLevel);
                    LevelController.nutrientCount = LevelController.nutrientCount - cost;
                    setNutrientCount();
                }               
            }else if(gameObject.name == "Rain Upgrade"){
                if(LevelController.upgradeRain()){
                    setValues(rainUpgrade, LevelController.rainUpgradeLevel);
                    LevelController.nutrientCount = LevelController.nutrientCount - cost;
                    setNutrientCount();
                }
            }else if(gameObject.name == "Skates Upgrade"){
                if(LevelController.upgradeSpeed()){
                    setValues(skatesUpgrade, LevelController.speedUpgradeLevel);
                    LevelController.nutrientCount = LevelController.nutrientCount - cost;
                    setNutrientCount();
                }
            }else if(gameObject.name == "Water Bottle Upgrade"){
                if(LevelController.upgradeWaterTank()){
                    setValues(glassesUpgrade, LevelController.waterTankLevel);
                    LevelController.nutrientCount = LevelController.nutrientCount - cost;
                    setNutrientCount();
                }
            }else if(gameObject.name == "Watering Can Upgrade"){
                if(LevelController.upgradeWateringCan()){
                    setValues(wateringCanUpgrade, LevelController.wateringCanLevel);
                    LevelController.nutrientCount = LevelController.nutrientCount - cost;
                    setNutrientCount();
                }
            }else{
                Debug.Log("Error");
            }
        }else{
            Debug.Log("Not Enough Money");
        }
    }

    public void setNutrientCount(){
        int nutrientCount = LevelController.nutrientCount;
        TextMeshProUGUI tmp = nutrientText.GetComponent<TextMeshProUGUI>();
        tmp.text = nutrientCount.ToString();
    }
}
