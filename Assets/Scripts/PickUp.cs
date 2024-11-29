using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mine : MonoBehaviour
{
    [SerializeField] UnityEvent onWalkedIn;
    [SerializeField] AudioClip[] explosionSoundClips;
    private void OnTriggerEnter(Collider other)
    {
        var my = other.GetComponent<MyController>();
        if (my != null)
        {
            onWalkedIn.Invoke();
            SoundFXManager.Instance.PlaySoundFXClip(explosionSoundClips[0], transform, 1f);
            other.GetComponent<PlayerManager>().TakeDamage(25f);
            Debug.LogWarning(other.name + " walked in mine");
            Debug.LogWarning("Player health: " + other.GetComponent<PlayerManager>().GetCurrentHealth());
            Destroy(gameObject, 0.5f);
        }
    }
} 