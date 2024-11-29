using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class CameraEffects : MonoBehaviour
{
    PlayerManager myPlayerManager;

    [Header("Camera Options")]
    [SerializeField] Volume postProcessVolume;
    private Vignette vignette;
    private Bloom bloom;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;
    private MotionBlur motionBlur;

    Coroutine _lowLifeRoutine;
    [SerializeField] AnimationCurve _VignetteSin;

    // Start is called before the first frame update
    void Start()
    {
        myPlayerManager = GetComponent<PlayerManager>();

        if (postProcessVolume.profile.TryGet(out Vignette vignetteEffect))
        {
            vignette = vignetteEffect;
        }

        if (postProcessVolume.profile.TryGet(out Bloom bloomEffect))
        {
            bloom = bloomEffect;
        }

        if (postProcessVolume.profile.TryGet(out ChromaticAberration chromaticEffect))
        {
            chromaticAberration = chromaticEffect;
        }

        if (postProcessVolume.profile.TryGet(out ColorAdjustments colorEffect))
        {
            colorAdjustments = colorEffect;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetCameraEffect()
    {
        if (vignette != null)
        {
            vignette.color.value = new Color(255 / 255f, 255 / 255f, 255 / 255f);
            vignette.intensity.value = 0.0f;
            bloom.intensity.value = 0;
            chromaticAberration.intensity.value = 0;
            colorAdjustments.saturation.value = 0f;
        }

        if (_lowLifeRoutine != null) StopCoroutine(_lowLifeRoutine);

    }


    public void LowHpCameraEffect()
    {
        ResetCameraEffect();
        if (vignette != null)
        {
            
            float percentage = myPlayerManager.GetCurrentHealth() / myPlayerManager.GetMaxHealth();
            if (percentage <= 0.25f)
            {
                //myPlayerManager.mIsLowHP = true;
                vignette.color.value = new Color(200 / 255f, 48 / 255f, 48 / 255f);
                //vignette.intensity.value = 0.5f;
                chromaticAberration.intensity.value = 1;
                colorAdjustments.saturation.value = -30f;

                _lowLifeRoutine = StartCoroutine(Tweening());
            }
        }
    }

    public void HealingHPCameraEffect()
    {
        ResetCameraEffect();
        TakeHealthCameraEffect();
        if (vignette != null)
        {
            vignette.color.value = new Color(39 / 255f, 221 / 255f, 42 / 255f);
            vignette.intensity.value = 0.15f;
            bloom.intensity.value = 0.5f;
            chromaticAberration.intensity.value = 1;
            colorAdjustments.saturation.value = 10f;
        }

        StartCoroutine(ResetAfterDelay(1.0f));
    }

    public void TakeDamageCameraEffect()
    {
        ResetCameraEffect(); // Réinitialise les effets avant d'ajouter le nouveau
        myPlayerManager.GetUnityEventAttack().Invoke();

        // Effet visuel sur le matériau du joueur
        myPlayerManager.GetBaseMaterial().SetColor("_Color", Color.red);
        myPlayerManager.GetBaseMaterial().SetFloat("_Effect", 1);

        // Effet d'assombrissement temporaire
        if (vignette != null)
        {
            vignette.intensity.value = 0.75f; 
            vignette.smoothness.value = 0.7f; 
            vignette.color.value = new Color(1f, 0f, 0f);
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = -1f; // Réduit l'exposition pour assombrir l'écran
        }

        // Retour progressif à la normale
        StartCoroutine(ResetVisualEffects(0.5f)); // Durée de 0.5 secondes pour le retour à la normale

        StartCoroutine(myPlayerManager.ResetMat()); // Réinitialise le matériau du joueur
    }

    public void TakeHealthCameraEffect()
    {
        myPlayerManager.GetBaseMaterial().SetColor("_Color", Color.green);
        myPlayerManager.GetBaseMaterial().SetFloat("_Effect", 1);
        StartCoroutine(myPlayerManager.ResetMat());
    }

    public void DodgeCameraEffect()
    {
        ResetCameraEffect(); // Réinitialise les effets actuels
        if (colorAdjustments != null)
        {
            StartCoroutine(DodgeEffect(1f));
        }
    }

    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Attend le délai spécifié
        ResetCameraEffect(); // Réinitialise les effets de la caméra
    }

    private IEnumerator Tweening()
    {
        var maxDuration = _VignetteSin.keys[^1].time;

        while (true)
        {
            var time = Time.time % maxDuration;
            vignette.intensity.value = _VignetteSin.Evaluate(time);
            yield return null;
        }
    }

    private IEnumerator DodgeEffect(float dodgeDuration)
    {
        float elapsedTime = 0f;
        bool increasing = true;

        while (elapsedTime < dodgeDuration)
        {
            float normalizedTime = elapsedTime / dodgeDuration; // Temps normalisé (0 à 1)
            if (increasing)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(0, -50, normalizedTime * 2); // Descends jusqu'à -50
                chromaticAberration.intensity.value = Mathf.Lerp(0, 1, normalizedTime * 2);
                if (normalizedTime >= 0.5f) increasing = false; // Change de direction à mi-chemin
            }
            else
            {
                colorAdjustments.saturation.value = Mathf.Lerp(-50, 0, (normalizedTime - 0.5f) * 2); // Remonte à 0
                chromaticAberration.intensity.value = Mathf.Lerp(1, 0, (normalizedTime - 0.5f) * 2);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        colorAdjustments.saturation.value = 0; 
        chromaticAberration.intensity.value = 0;
    }

    private IEnumerator ResetVisualEffects(float duration)
    {
        float elapsedTime = 0f;

        // Capture des valeurs initiales
        float initialVignetteIntensity = vignette != null ? vignette.intensity.value : 0f;
        float initialExposure = colorAdjustments != null ? colorAdjustments.postExposure.value : 0f;
        Color initialVignetteColor = vignette != null ? vignette.color.value : Color.black;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // Normalisation entre 0 et 1

            // Réduit progressivement l'effet de la vignette
            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(initialVignetteIntensity, 0f, t); // Retour à 0 pour l'intensité
                vignette.color.value = Color.Lerp(initialVignetteColor, Color.black, t); // Retour à une couleur neutre
            }

            // Réduit progressivement l'effet d'exposition
            if (colorAdjustments != null)
            {
                colorAdjustments.postExposure.value = Mathf.Lerp(initialExposure, 0f, t); // Retour à l'exposition normale
            }

            yield return null;
        }

        // Réinitialisation finale des valeurs
        if (vignette != null)
        {
            vignette.intensity.value = 0f;
            vignette.color.value = Color.white; // Couleur neutre
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = 0f;
        }
    }

}
