using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLogic : MonoBehaviour
{
    [SerializeField] GameObject camera;

    void Awake(){

    }
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
         camera.transform.position = camera.transform.position + new Vector3(0,-1, 0);
    }
}
