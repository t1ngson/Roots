using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 2f;

    private GameObject playerCamera;

    private float leftBounds;
    private float rightBounds;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        //Debug.Log(playerCamera);
        leftBounds  = -5f;
        rightBounds =  5f;
    }

    // Update is called once per frame
    void Update()
    {
        processMovement();

        // Make camera follow player
        playerCamera.transform.position =
            new Vector3(Mathf.Clamp(transform.position.x, leftBounds, rightBounds),
            transform.position.y,
            -10f);
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
    }
}
