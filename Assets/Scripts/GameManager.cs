using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [SerializeField] CutsceneController CutsceneController;
    [SerializeField] SoundMixerManager SoundMixerManager;

    // Start is called before the first frame update
    void Start()
    {
        if (CutsceneController != null && CutsceneController.GetComponent<PlayableDirector>() != null)
        {
            CutsceneController.PlayCutscene();
        }
        else
        {
            Debug.LogError("CutsceneController ou PlayableDirector manquant !");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
