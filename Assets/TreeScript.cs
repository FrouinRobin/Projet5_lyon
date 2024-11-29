using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    [SerializeField] public AudioClip[] SoundEffectsClips;
    public bool trigger = false;
    private void OnCollisionEnter(Collision other)
    {
        trigger = true;
        Debug.Log(other.gameObject.name + "triger");
        Debug.Log("triger");
        Debug.Log(other.gameObject.tag + "triger");
        if (other.gameObject.tag == "Slash")
        {
            Rigidbody rb = transform.parent.GetChild(0).gameObject.AddComponent<Rigidbody>();
            MeshCollider mc = transform.parent.GetChild(0).gameObject.AddComponent<MeshCollider>();
            mc.convex = true;
            rb.AddForce(new Vector3(0.1f, 0.1f, 0.1f));
            Destroy(transform.parent.GetChild(0).gameObject, 5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        trigger = true;
        Debug.Log(other.gameObject.name + "triger");
        Debug.Log("triger here");
        Debug.Log(other.gameObject.tag + "triger");
        if (other.gameObject.tag == "Slash")
        {
            
            Rigidbody rb = transform.parent.GetChild(0).gameObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(new Vector3(3f, 3f, 3f), ForceMode.Impulse);
            gameObject.GetComponent<MeshCollider>().isTrigger = true;
            Destroy(transform.parent.GetChild(0).gameObject, 5f);
        }
    }
}