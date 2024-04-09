using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWheelAnim : MonoBehaviour
{
    private double speed;

    [SerializeField] private double OptionalSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (OptionalSpeed == 0)
        {
            speed = Random.Range(15f, 20f);
        }
        else
        {
            speed = OptionalSpeed;
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate((float) (speed * Time.deltaTime), 0, 0);
    }
}
