using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class WorldGen : MonoBehaviour
{

    [Header("Dirt TileMaps")]
    public GameObject[] dirtTiles;
    public int[] dirtTileWeights;
    private int dirtTotalWeights;

    [Header("Partial Stone TileMaps")]
    public GameObject[] partialRockTiles;
    public int[] partialRockTileWeights;
    private int partialRockTotalWeights;

    [Header("Rock TileMaps")]
    public GameObject[] rockTiles;
    public int[] rockTileWeights;
    private int rockTotalWeights;

    [Header("Collectable prefabs")]
    public GameObject nutrient;
	public GameObject water;

    [Header("Map Settings")]
    public int aheadDistance;
    public int tileWidth;
    public int partialStoneDepth;
    public int StoneDepth;
    public int endGameDepth;
    public float nutrientDensity;
    public float waterDensity;
    public float nutrientAdditionPerTile;
    public float waterAdditionPerTile;

    private GameObject player = null;

    //private int count;
    public int currentDistance;

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

        dirtTotalWeights = dirtTileWeights.Sum();
        partialRockTotalWeights = partialRockTileWeights.Sum();
        rockTotalWeights = rockTileWeights.Sum();

        waterAdditionPerTile /= LevelController.getRainUpgradeValue();
        //GetComponent<LevelController>().visionUpgradeListener.AddListener(onVisionUpgrade);

        // pregenerate world start
        for (int i = 0; i < 5; i++)
        {
            generateTileLine();
        }

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
        if ((int)player.GetComponent<PlayerController>().posn.y - aheadDistance < currentDistance)
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
            GameObject toGenerate;
            if (currentDistance > -partialStoneDepth)
            {
                //Debug.Log("Generating Dirt");
                toGenerate = getTileToGenerate(dirtTiles, dirtTileWeights, dirtTotalWeights);
            } else if (currentDistance > -StoneDepth)
            {
                //Debug.Log("Generating Partial Rock");
                toGenerate = getTileToGenerate(partialRockTiles, partialRockTileWeights, partialRockTotalWeights);
            } else
            {
                //Debug.Log("Generating stone");
                toGenerate = getTileToGenerate(rockTiles, rockTileWeights, rockTotalWeights);
            }

            Instantiate(toGenerate, new Vector3(i, currentDistance, 0), Quaternion.identity);
        }
        for (int i = 0; i < tileWidth * nutrientDensity; i++)
        {
            generateNutrient(currentDistance);
        }
        for (int i = 0; i < tileWidth * waterDensity; i++)
        {
            generateWater(currentDistance);
        }

        incrementTileDistance();
    }

    GameObject getTileToGenerate(GameObject[] tiles, int[] tileWeights, int totalWeights)
    {
        int offset = Random.Range(0, totalWeights);
        //Debug.Log(offset);
        for (int i = 0; i < tileWeights.Length; i++)
        {
            if (offset < tileWeights[i])
            {
                //Debug.Log(i);
                return tiles[i];
            }
            offset -= tileWeights[i];
        }
        //Debug.Log(tiles.Length - 1);
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

    void incrementTileDistance()
    {
        currentDistance--;
        nutrientDensity += nutrientAdditionPerTile;
        nutrientDensity = Mathf.Clamp(nutrientDensity, 0f, 0.5f);
        waterDensity += waterAdditionPerTile;
        waterDensity = Mathf.Clamp(waterDensity, 0f, 0.5f);
    }

    public void recalculateAheadDistance()
    {
        int vertExtent = (int)LevelController.getVisionUpgradeValue();// (int)GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().orthographicSize;
        aheadDistance = vertExtent + 2;
    }
}
