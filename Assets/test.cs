using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Use this for initialization
    public Texture fdrfe;//Render Texture
    void Start()
    {
        WebCamTexture web = new WebCamTexture(1280, 720, 60);
        GetComponent<MeshRenderer>().material.mainTexture  = web;
        web.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

