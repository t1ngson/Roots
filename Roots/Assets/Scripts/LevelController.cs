using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelController : MonoBehaviour
{

    [Header("Levels")]
    public int nutrientCount;
    public int waterCount;

    [Header("Upgrades")]
    public float speedUpgradeLevel;
    public float visionUpgradeLevel;
    public int RainUpgradeLevel;
    public int WaterTankLevel;
    public int DrillUpgradeLevel;
    public bool WateringCan;

    [Header("Upgrade Listeners")]
    public UnityEvent<float> speedUpgradeListener;
    public UnityEvent<float> visionUpgradeListener;

    private void Awake()
    {
        // initialise Values
        speedUpgradeLevel = 1;
        visionUpgradeLevel = 1;
        RainUpgradeLevel = 1;
        WaterTankLevel = 1;
        DrillUpgradeLevel = 1;
        WateringCan = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void upgradeSpeed()
    {
        speedUpgradeLevel += 0.5f;
        speedUpgradeListener.Invoke(speedUpgradeLevel);
    }

    public void upgradeVision()
    {
        visionUpgradeLevel += 0.5f;
        visionUpgradeListener.Invoke(visionUpgradeLevel);
    }
}
