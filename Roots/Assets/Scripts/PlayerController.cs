using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameObject gameController;

    public float playerSpeed = 2f;

    private Camera playerCamera;

    private float leftBounds;
    private float rightBounds;

    private float leftCameraBounds;
    private float rightCameraBounds;

    private Vector2 resolution;

    // Start is called before the first frame update
    void Start()
    {
        resolution = new Vector2(Screen.width, Screen.height);
        GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        //Debug.Log(playerCamera);
        playerCamera = cameraObject.GetComponent<Camera>();
        //leftBounds = -5f;
        //rightBounds = 5f;
        recalculateBounds();

        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        processMovement();

        // Make camera follow player
        playerCamera.transform.position =
            new Vector3(Mathf.Clamp(transform.position.x, leftCameraBounds, rightCameraBounds),
            transform.position.y,
            -10f);
        
        if ((resolution.x != Screen.width || resolution.y != Screen.height))
        {
            resolution = new Vector2(Screen.width, Screen.height);
            recalculateBounds();
        }
    }

    // Temporary movement algorithm for testing
    void processMovement()
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

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, leftBounds, rightBounds),
            transform.position.y,
            0);
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
            gameController.GetComponent<LevelController>().nutrientCount++;
            Destroy(other);
        }
        else if (other.CompareTag("Water"))
        {
            gameController.GetComponent<LevelController>().waterCount++;
            Destroy(other);
        }
    }
}
