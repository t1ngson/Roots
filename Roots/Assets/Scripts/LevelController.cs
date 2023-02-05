using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{

    [Header("Levels")]
    public static int nutrientCount;
    public static int waterCount;

    public int maxWaterCount;

    [Header("Upgrades")]
    public static int speedUpgradeLevel;
    public static int visionUpgradeLevel;
    public static int rainUpgradeLevel;
    public static int waterTankLevel;
    public static int drillUpgradeLevel;
    public static int wateringCanLevel;

    [Header("Upgrade Listeners")]
    //public UnityEvent<float> speedUpgradeListener;
    //public UnityEvent<float> visionUpgradeListener;
    //public UnityEvent<int> rainUpgradeListener;

    private static bool initialised = false;
    [SerializeField] GameObject nutrientText;
    [SerializeField] GameObject waterText;

    [Header("Water usage")]
    [SerializeField]private float timePerWaterUse;
    private float lastWaterUseTime;


    private void Awake()
    {
        if (!initialised)
        {
            // initialise Values
            speedUpgradeLevel = 0;
            visionUpgradeLevel = 0;
            rainUpgradeLevel = 0;
            waterTankLevel = 0;
            drillUpgradeLevel = 0;
            wateringCanLevel = 0;
            initialised = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        maxWaterCount = getWaterTankValue() * maxWaterCount;
        waterCount = 10;

        lastWaterUseTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        nutrientText.GetComponent<TextMeshProUGUI>().text = nutrientCount.ToString();
        waterText.GetComponent<TextMeshProUGUI>().text = waterCount.ToString() + "/" + maxWaterCount.ToString();

        // check water usage
        if (lastWaterUseTime + timePerWaterUse > Time.realtimeSinceStartup)
        {
            useWater();
            lastWaterUseTime += timePerWaterUse;
        }
    }

    public void collectWater()
    {
        waterCount++;
        waterCount = Mathf.Min(waterCount, maxWaterCount);
    }

    public static bool upgradeSpeed()
    {
        if (speedUpgradeLevel == 5)
            return false;
        speedUpgradeLevel += 1;
        //speedUpgradeLevel = Mathf.Min(speedUpgradeLevel, 5);
        //speedUpgradeListener.Invoke(speedUpgradeLevel);
        return true;
    }

    public static bool upgradeVision()
    {
        if (visionUpgradeLevel == 5)
            return false;
        visionUpgradeLevel += 1;
        //visionUpgradeLevel = Mathf.Min(visionUpgradeLevel, 5);
        //visionUpgradeListener.Invoke(visionUpgradeLevel);
        return true;
    }

    public static bool upgradeRain()
    {
        if (rainUpgradeLevel == 5)
            return false;
        rainUpgradeLevel += 1;
        //rainUpgradeLevel = Mathf.Min(visionUpgradeLevel, 5);
        //rainUpgradeListener.Invoke(rainUpgradeLevel);
        return true;
    }

    public static bool upgradeWaterTank()
    {
        if (waterTankLevel == 5)
            return false;
        waterTankLevel += 1;
        //waterTankLevel = Mathf.Min(waterTankLevel, 5);
        //rainUpgradeListener.Invoke(rainUpgradeLevel);
        return true;
    }

    public static bool upgradeDrill()
    {
        if (drillUpgradeLevel == 2)
            return false;
        drillUpgradeLevel += 1;
        //drillUpgradeLevel = Mathf.Min(drillUpgradeLevel, 2);
        return true;
    }

    public static bool upgradeWateringCan()
    {
        if (wateringCanLevel == 10)
            return false;
        wateringCanLevel += 1;
        //drillUpgradeLevel = Mathf.Min(drillUpgradeLevel, 2);
        return true;
    }

    // returns a scale value to represent the speed of the root
    public static float getSpeedUpgradeValue()
    {
        return speedUpgradeLevel * 0.5f + 1;
    }

    // returns a scaled value to represent the vision ahead of the root head
    public static float getVisionUpgradeValue()
    {
        return visionUpgradeLevel * 0.5f + 1;
    }

    // returns the direct rain level
    public static int getRainUpgradeValue()
    {
        return rainUpgradeLevel + 1;
    }

    // returns the water tank level + 1
    public static int getWaterTankValue()
    {
        return waterTankLevel + 1;
    }

    // returns direct drill level
    public static int getdrillUpgradeValue()
    {
        return drillUpgradeLevel;
    }

    // Checks if the watering can level is equal to 10
    public static bool getWateringCanFull()
    {
        return wateringCanLevel == 10;
    }

    private void useWater()
    {
        waterCount--;
        if (waterCount <= 0)
        {
            //TODO: Exit Game
            Debug.Log("Game Over");
        }
    }
}
