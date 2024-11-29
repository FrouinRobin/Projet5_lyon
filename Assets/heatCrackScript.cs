using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heatCrackScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("trigger with " + collision.gameObject.name);
        Debug.Log("trigger with " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Slash"))
        {
            collision.gameObject.transform.parent.GetComponent<Animator>().SetBool("isEvolved", true);
            collision.gameObject.GetComponent<SlashCollideScript>().isEvolved = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger with " + other.gameObject.name);
        Debug.Log("trigger with " + other.gameObject.tag);
        if (other.gameObject.CompareTag("Slash"))
        {
            other.gameObject.transform.parent.GetComponent<Animator>().SetBool("isEvolved", true);
            other.gameObject.GetComponent<SlashCollideScript>().isEvolved = true;
        }
    }
}
