using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyDummy : MonoBehaviour
{
    private float health;

    private Animator animator;
    [SerializeField] private AudioClip[] damageSoundClips;

    private SkinnedMeshRenderer smr;


    private Material outlineMaterial; // Mat�riau Outline
    private Material baseMaterial;    // Mat�riau de base (training_dummy)

    [SerializeField] UnityEvent hit;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        health = 250;

        smr = GetComponentInChildren<SkinnedMeshRenderer>();

        Material[] materials = smr.materials;

        baseMaterial = materials[0]; // Premier mat�riau (training_dummy)
        outlineMaterial = materials[1]; // Deuxi�me mat�riau (Outline)
        this.SetOutlineToRed();
    }

    void Update()
    {
        
    }

    public void TakeDamage(float damage, string tag)
    {
        health = Mathf.Max(health - damage, 0);
        SoundFXManager.Instance.PlaySoundFXClip(damageSoundClips[0], transform, 1f);
        //SoundFXManager.Instance.PlayRandomSoundFXClip(damageSoundClips, transform, 1f);
        if (tag == "Slash")
        {
            animator = null;
            return;
        }
        else if(health <= 0 && tag != "Slash")
        {
            animator.SetBool("Dead", true);
        }
        else
        {
            animator.SetTrigger("Hit");
            baseMaterial.SetFloat("_Effect", 1);
            StartCoroutine(ResetMat());
            hit.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitZone") || other.CompareTag("CrackZone"))
        {
            TakeDamage(25, other.gameObject.tag);
        }
        else if (other.CompareTag("Slash"))
        {
            Debug.Log("here");
            TakeDamage(other.GetComponent<SlashCollideScript>().damage, other.gameObject.tag);
            Rigidbody cutRb = transform.GetChild(1).gameObject.AddComponent<Rigidbody>();
            MeshCollider cutMesh = transform.GetChild(1).gameObject.AddComponent<MeshCollider>();
            cutMesh.convex = true;
            cutRb.AddForce(1f,1f,1f, ForceMode.Impulse);
        }
    }

    IEnumerator ResetMat()
    {
        yield return new WaitForSeconds(0.5f);
        baseMaterial.SetFloat("_Effect", 0);
    }

    public void SetOutlineToRed()
    {
        if (outlineMaterial != null)
        {
            outlineMaterial.SetColor("_Outline_Color", Color.red); // Change la couleur en rouge
        }
        else
        {
            Debug.LogWarning("Outline material is missing or not assigned.");
        }
    }
}
