using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGen : MonoBehaviour
{
	
	public int waterGathered = 0;
	public int nutrientGathered = 0;
	
	
    public GameObject nutrient;
	public GameObject water;

    private int count;

    // Start is called before the first frame update
    void Start()
    {
		
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (count++ % 20 == 0)
        {
            Instantiate(nutrient, new Vector3(0, -1, 0), Quaternion.identity);
			Instantiate(water, new Vector3(0, 0, 1), Quaternion.identity);
        }
    }
}
