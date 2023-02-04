using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGen : MonoBehaviour
{

    public GameObject nutrient;

    private int count;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (count++ == 20)
        {
            Instantiate(nutrient, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
