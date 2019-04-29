using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   
    private float speed = 5f;


    void Start()
    {
        
    }

    void Update()
    {

        float _xMov = Input.GetAxisRaw("Horizontal");
        float _zMov = Input.GetAxisRaw("Vertical");


        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        //final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical).normalized * speed;


        //

    }

}
