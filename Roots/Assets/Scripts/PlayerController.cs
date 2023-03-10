using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(RootRenderer))]
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

    [Header("Pathing")]
    public RootRenderer renderer;
    public Path path;

    public Vector2 posn;

    private float timeSinceLastNode = 0;
    private float timeBetweenNodes = 1;

    Quaternion rot;

    [Header("Head management")]
    public GameObject DummyHead;
    

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
        
        renderer = GetComponent<RootRenderer>();
        path = renderer.path;
        renderer.autoUpdate = true;
        renderer.spacing = 0.1f;
        path.AutoSetControlPoints = true;
        path.MovePoint(0,new Vector2(0,0));
        path.MovePoint(3,new Vector2(0,0.2f));
    }

    // Update is called once per frame
    void Update()
    {
        updatePlayerSprites();
        if (mouseControl) {
            processMouseMovement();
        } else
            processKeyboardMovement();

        posn = new Vector2(
            Mathf.Clamp(posn.x, leftBounds, rightBounds),
            Mathf.Clamp(posn.y, maxDepth, 0.0f)
        );

        
        
        if ((resolution.x != Screen.width || resolution.y != Screen.height))
        {
            resolution = new Vector2(Screen.width, Screen.height);
            recalculateBounds();
        }
        timeSinceLastNode += Time.deltaTime;
        if (timeSinceLastNode >= timeBetweenNodes) {
            path.AddSegment(posn + new Vector2(0, 0.01f));
            timeSinceLastNode -= timeBetweenNodes;
        } else {
            path.MovePoint(path.NumPoints-1,posn);
        }
        renderer.UpdateNarrowMesh();
        //Instantiate(trail, transform.position, transform.rotation);
        GetComponent<CircleCollider2D>().offset = posn;
        DummyHead.transform.position = new Vector3(posn.x, posn.y, 10);
        DummyHead.transform.rotation = rot;
    }

    // Temporary movement algorithm for testing
    void processKeyboardMovement()
    {
        Vector2 movement = new Vector2();

        if (Input.GetKey(KeyCode.DownArrow))
        {
            movement += new Vector2(0, -1);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            movement += new Vector2(0, 1);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement += new Vector2(-1, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            movement += new Vector2(1, 0);
        }

        posn += movement.normalized * playerSpeed * Time.deltaTime;
    }

    void processMouseMovement()
    {
        Vector2 mousePos = playerCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 movement = mousePos - posn;

        posn += movement.normalized * playerSpeed * Time.deltaTime;


        float AngleRad = Mathf.Atan2(movement.y, movement.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        rot = Quaternion.Euler(0,0,AngleDeg);
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
        GameObject other = collision.gameObject;
        if (other.CompareTag("Nutrient"))
        {
            LevelController.nutrientCount++;
            Destroy(other);
            gameController.GetComponent<Sound>().playNomNom();
            LevelController.upgradeSpeed();
            setSpeedUpgrade(LevelController.getSpeedUpgradeValue());
            gameController.GetComponent<WorldGen>().recalculateAheadDistance();
        }
        else if (other.CompareTag("Water"))
        {
            gameController.GetComponent<LevelController>().collectWater();
            Destroy(other);
            gameController.GetComponent<Sound>().playSplash();
            LevelController.upgradeVision();
            setVisionUpgrade(LevelController.getVisionUpgradeValue());
        }
    }

    void LateUpdate()
    {
        // Make camera follow player
        Vector3 newCameraPosition =
            new Vector3(Mathf.Clamp(posn.x, leftCameraBounds, rightCameraBounds),
            posn.y,
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
        /*
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
        */
    }
}
