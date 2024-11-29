using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    public bool IsInCutscene = false;
    // Start is called before the first frame update
    void Start()
    {
        //director = GetComponent<PlayableDirector>();
        if (director == null)
        {
            Debug.LogError($"PlayableDirector non trouv� sur l'objet {gameObject.name}");
        }
        else
        {
            director.stopped += OnCutsceneEnd; // Abonnez-vous � l'�v�nement 'stopped'
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(IsInCutscene);
    }

    public void PlayCutscene() 
    {
        IsInCutscene = true;
        director.Play();
    }

    private void OnCutsceneEnd(PlayableDirector obj)
    {
        IsInCutscene = false;
        Debug.Log("Cutscene termin�e !");
    }

    private void OnDestroy()
    {
        // Se d�sabonner pour �viter les r�f�rences pendantes
        if (director != null)
        {
            director.stopped -= OnCutsceneEnd;
        }
    }
}
