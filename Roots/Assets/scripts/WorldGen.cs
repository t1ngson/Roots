using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class WorldGen : MonoBehaviour
{

    public GameObject[] tiles;
    public int[] tileWeights;
    private int totalWeights;

    public GameObject nutrient;
	public GameObject water;

    public int aheadDistance;
    public int tileWidth;
    public float nutrientDensity;
    public float waterDensity;

    private GameObject player = null;

    //private int count;
    private int currentDistance;

    //private Vector2 resolution;

    // Start is called before the first frame update
    void Start()
    {
        //resolution = new Vector2(Screen.width, Screen.height);
        currentDistance = 0;
        player = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(player, "Cannot Find GameObject with Player tag");

        nutrientDensity = Mathf.Clamp(nutrientDensity, 0f, 1f);
        waterDensity = Mathf.Clamp(waterDensity, 0f, 1f);

        recalculateAheadDistance();

        totalWeights = tileWeights.Sum();
    }

    void Update()
    {
        /*if ((resolution.x != Screen.width || resolution.y != Screen.height))
        {
            resolution = new Vector2(Screen.width, Screen.height);
            recalculateAheadDistance();
        }*/
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
        for (int i = -tileWidth / 2; i <= tileWidth/2; i++)
        {
            //GameObject toGenerate = (GameObject)tiles.GetValue(Random.Range(0, tiles.Length));
            Instantiate(getTileToGenerate(), new Vector3(i, currentDistance, 0), Quaternion.identity);
        }
        for (int i = 0; i < tileWidth * nutrientDensity; i++)
        {
            generateNutrient(currentDistance);
        }
        for (int i = 0; i < tileWidth * waterDensity; i++)
        {
            generateWater(currentDistance);
        }

        currentDistance--;
    }

    GameObject getTileToGenerate()
    {
        int offset = Random.Range(0, totalWeights);
        Debug.Log(offset);
        for (int i = 0; i < tileWeights.Length; i++)
        {
            if (offset < tileWeights[i])
            {
                Debug.Log(i);
                return tiles[i];
            }
            offset -= tileWeights[i];
        }
        Debug.Log(tiles.Length - 1);
        return tiles[tiles.Length - 1];
    }

    void generateNutrient(float yLevel)
    {
        float position = Random.Range(-tileWidth / 2f, tileWidth / 2f);
        Instantiate(nutrient, new Vector3(position, yLevel, 0), Quaternion.identity);
    }

    void generateWater(float yLevel)
    {
        float position = Random.Range(-tileWidth / 2f, tileWidth / 2f);
        Instantiate(water, new Vector3(position, yLevel, 0), Quaternion.identity);
    }

    void recalculateAheadDistance()
    {
        int vertExtent = (int)GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().orthographicSize;
        aheadDistance = vertExtent + 2;
    }
}
