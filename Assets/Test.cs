using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(Time.time);
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            Debug.Log(Time.time);
        }
    }
}
