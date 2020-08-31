using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    public float speed = 5;
    private Material mat;


    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material ;
    }

    // Update is called once per frame
    void Update()
    {
        if (mat.mainTextureOffset.y >= 4)
        {
            mat.mainTextureOffset = Vector2.zero;
        }
        else
        {
            mat.mainTextureOffset += new Vector2(0, Time.deltaTime * speed);
        }
    }
}
