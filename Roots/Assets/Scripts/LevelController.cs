using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public static int wateringCan;

    [Header("Upgrade Listeners")]
    //public UnityEvent<float> speedUpgradeListener;
    //public UnityEvent<float> visionUpgradeListener;
    //public UnityEvent<int> rainUpgradeListener;

    private static bool initialised = false;

    private void Awake()
    {
        if (!initialised)
        {
            // initialise Values
            speedUpgradeLevel = 1;
            visionUpgradeLevel = 10;
            rainUpgradeLevel = 0;
            waterTankLevel = 0;
            drillUpgradeLevel = 0;
            wateringCan = 0;
            initialised = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        maxWaterCount = getWaterTankValue() * maxWaterCount;
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public static float getSpeedUpgradeValue()
    {
        return speedUpgradeLevel * 0.5f + 1;
    }

    public static float getVisionUpgradeValue()
    {
        return visionUpgradeLevel * 0.5f + 1;
    }

    public static int getRainUpgradeValue()
    {
        return rainUpgradeLevel;
    }

    public static int getWaterTankValue()
    {
        return waterTankLevel + 1;
    }

    public static int getdrillUpgradeValue()
    {
        return drillUpgradeLevel;
    }
}
