using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameObject gameController;

    public bool mouseControl;

    public float playerSpeed = 2f;

    public GameObject trail;

    private Camera playerCamera;

    private float leftBounds;
    private float rightBounds;

    private float leftCameraBounds;
    private float rightCameraBounds;

    private float maxDepth;

    private Vector2 resolution;

    [Header("Smooth Camera Following")]
    public float smoothSpeed = 0.1f;

    [Header("Upgrade Sprites")]
    public GameObject Drill;
    public GameObject DDrill;
    public GameObject[] WaterBottles;
    public GameObject Glasses;
    public GameObject[] Skates;
    public GameObject WateringCan;
    

    // Start is called before the first frame update
    void Start()
    {
        resolution = new Vector2(Screen.width, Screen.height);
        GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        //Debug.Log(playerCamera);
        playerCamera = cameraObject.GetComponent<Camera>();
        //leftBounds = -5f;
        //rightBounds = 5f;
        //recalculateBounds();

        gameController = GameObject.FindGameObjectWithTag("GameController");
        //LevelController controller = gameController.GetComponent<LevelController>();
        //controller.speedUpgradeListener.AddListener(onSpeedUpgrade);
        setSpeedUpgrade(LevelController.getSpeedUpgradeValue());
        //controller.visionUpgradeListener.AddListener(onVisionUpgrade);
        setVisionUpgrade(LevelController.getVisionUpgradeValue());

        switch (LevelController.getdrillUpgradeValue())
        {
            case 0:
                maxDepth = gameController.GetComponent<WorldGen>().partialStoneDepth - 1;
                break;
            case 1:
                maxDepth = gameController.GetComponent<WorldGen>().StoneDepth - 1;
                break;
            case 2:
                maxDepth = gameController.GetComponent<WorldGen>().endGameDepth + 5;
                break;
            default:
                Debug.LogError("drill upgrade value not in range");
                maxDepth = 5;
                break;
        }
        maxDepth = -maxDepth;

        smoothSpeed = ( (11 *0.5f)+1 - LevelController.getVisionUpgradeValue()) / 100;
    }

    // Update is called once per frame
    void Update()
    {
        updatePlayerSprites();
        if (mouseControl)
            processMouseMovement();
        else
            processKeyboardMovement();

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, leftBounds, rightBounds),
            Mathf.Clamp(transform.position.y, maxDepth, 0.0f),
            transform.position.z
            );

        
        
        if ((resolution.x != Screen.width || resolution.y != Screen.height))
        {
            resolution = new Vector2(Screen.width, Screen.height);
            recalculateBounds();
        }

        Instantiate(trail, transform.position, transform.rotation);
    }

    // Temporary movement algorithm for testing
    void processKeyboardMovement()
    {
        Vector3 movement = new Vector3();

        if (Input.GetKey(KeyCode.DownArrow))
        {
            movement += new Vector3(0, -1);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            movement += new Vector3(0, 1);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement += new Vector3(-1, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            movement += new Vector3(1, 0);
        }

        transform.position += movement.normalized * playerSpeed * Time.deltaTime;
    }

    void processMouseMovement()
    {
        Vector3 mousePos = playerCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector3 movement = mousePos - transform.position;

        movement.z = 0f;

        transform.position += movement.normalized * playerSpeed * Time.deltaTime;

        float AngleRad = Mathf.Atan2(movement.y, movement.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;

        transform.rotation = Quaternion.Euler(0, 0, AngleDeg);
    }

    void recalculateBounds()
    {
        float vertExtent = playerCamera.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        int tileWidth = GameObject.FindGameObjectWithTag("GameController").GetComponent<WorldGen>().tileWidth;
        float bound = Mathf.Max(tileWidth / 2 - horzExtent, 0f);
        leftCameraBounds = -bound;
        rightCameraBounds = bound;
        leftBounds = -tileWidth / 2;
        rightBounds = tileWidth / 2;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        GameObject other = collision.gameObject;
        if (other.CompareTag("Nutrient"))
        {
            LevelController.nutrientCount++;
            Destroy(other);
            gameController.GetComponent<Sound>().playNomNom();
            LevelController.upgradeSpeed();
        }
        else if (other.CompareTag("Water"))
        {
            LevelController.waterCount++;
            Destroy(other);
            gameController.GetComponent<Sound>().playSplash();
            LevelController.upgradeVision();
        }
    }

    private void LateUpdate()
    {
        // Make camera follow player
        Vector3 newCameraPosition =
            new Vector3(Mathf.Clamp(transform.position.x, leftCameraBounds, rightCameraBounds),
            transform.position.y,
            -10f);
        SmoothFollow(newCameraPosition);
    }

    public void setSpeedUpgrade(float newSpeed)
    {
        playerSpeed = newSpeed;
    }

    public void setVisionUpgrade(float newVision)
    {
        playerCamera.orthographicSize = newVision;
        recalculateBounds();
    }

    private void SmoothFollow(Vector3 targetPos)
    {
        Vector3 smoothFollow = Vector3.Lerp(playerCamera.transform.position, targetPos, smoothSpeed);
        playerCamera.transform.position = smoothFollow;
    }

    private void updatePlayerSprites()
    {
        // Drills
        switch (LevelController.getdrillUpgradeValue())
        {
            case 0:
                Drill.SetActive(false);
                DDrill.SetActive(false);
                break;
            case 1:
                Drill.SetActive(true);
                DDrill.SetActive(false);
                break;
            case 2:
                Drill.SetActive(false);
                DDrill.SetActive(true);
                break;
            default:
                Debug.LogError("Drill upgrade invalid");
                break;
        }
        // WaterBottles
        int numberOfBottles = LevelController.getWaterTankValue() - 1;
        for (int i = 0; i < WaterBottles.Length; i++)
        {
            if (i < numberOfBottles)
            {
                WaterBottles[i].SetActive(true);
            }
            else
            {
                WaterBottles[i].SetActive(false);
            }
        }
        // Glasses
        if (LevelController.visionUpgradeLevel > 2)
        {
            Glasses.SetActive(true);
        }
        else
        {
            Glasses.SetActive(false);
        }
        // Skates
        int numberOfSkates = LevelController.speedUpgradeLevel;
        for (int i = 0; i < Skates.Length; i++)
        {
            if (i < numberOfSkates)
            {
                Skates[i].SetActive(true);
            }
            else
            {
                Skates[i].SetActive(false);
            }
        }
        // WateringCan
        WateringCan.SetActive(LevelController.getWateringCanFull());
    }
}
