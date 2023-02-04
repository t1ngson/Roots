using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGen : MonoBehaviour
{
	
	public int waterGathered = 0;
	public int nutrientGathered = 0;

    public GameObject player;
    public int playerSpeed = 1;

    public GameObject[] tiles;
    public GameObject nutrient;
	public GameObject water;

    public int aheadDistance;
    public int tileWidth;

    //private int count;
    private int currentDistance;

    // Start is called before the first frame update
    void Start()
    {
        currentDistance = 0;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            player.transform.position -= new Vector3(0, playerSpeed * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.UpArrow))
        {
            player.transform.position += new Vector3(0, playerSpeed * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log((int)player.transform.position.y - aheadDistance);
        if ((int)player.transform.position.y - aheadDistance < currentDistance)
        {
            Debug.Log("Generating new Line");
            generateTileLine();
        }

        // Temporary tests
        /*if (count++ % 20 == 0)
        {
            Instantiate(nutrient, new Vector3(0, -1, 0), Quaternion.identity);
			Instantiate(water, new Vector3(0, 0, 1), Quaternion.identity);
        }*/
    }

    void generateTileLine()
    {
        for (int i = -tileWidth / 2; i < tileWidth/2; i++)
        {
            GameObject toGenerate = (GameObject)tiles.GetValue(Random.Range(0, tiles.Length));
            Instantiate(toGenerate, new Vector3(i, currentDistance), Quaternion.identity);
        }
        currentDistance--;
    }
}
