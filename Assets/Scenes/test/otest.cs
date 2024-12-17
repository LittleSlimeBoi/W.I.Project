using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class otest : MonoBehaviour
{
    Vector3 x1 = new Vector3(-4.5f, 0 , 0), x2 = new Vector3(-8f, 0, 0);
    float time = 1f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time >= 1)
        {
            time = -1f;
        }
        transform.position = Vector2.Lerp(x1, x2, time);
    }
}
