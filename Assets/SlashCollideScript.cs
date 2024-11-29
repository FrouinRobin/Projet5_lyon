using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SlashCollideScript : MonoBehaviour
{
    public GameObject explosion;
    public bool isEvolved = false;
    public float damage = 15;

    public void Update()
    {
        if (isEvolved)
        {
            damage = 50;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger slash with" + other.name);
        if (other.gameObject.tag != "CrackZone")
        {
            if (isEvolved)
            {
                Instantiate(explosion, transform.position, transform.rotation);
                Destroy(transform.parent.gameObject);
            }
            
        }
        
    }
}
