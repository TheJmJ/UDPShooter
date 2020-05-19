using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BulletScript : MonoBehaviour
{
    public float speed = 1f;
    public float lifetime = 10f;

    float startTime;

    bool hasHit = false;

    private void Awake()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - startTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
        {
            Debug.Log("Hit "+ hit.collider.transform.name + "! Killing myself, bye cruel world");
            if(hit.collider.gameObject.GetComponent<Client>())
            {
                hit.collider.gameObject.GetComponent<PlayerMovement>().Respawn();
            }
            Destroy(gameObject);
        }

        transform.Translate(transform.InverseTransformDirection(transform.forward) * speed, Space.Self);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}