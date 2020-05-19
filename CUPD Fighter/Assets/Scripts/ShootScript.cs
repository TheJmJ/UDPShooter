using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootScript : MonoBehaviour
{
    [SerializeField] GameObject bullet;

    // Update is called once per frame
    public void Shoot()
    {
        GameObject go = Instantiate(bullet, transform.position + transform.forward * 1.2f, transform.rotation);
    }
}
