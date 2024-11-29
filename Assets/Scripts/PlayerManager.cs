using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerManager : MonoBehaviour
{
    [Header("Debug Option")]
    [Tooltip("Si activée, permet d'activer les fonctionnalitées de debug.")]
    [SerializeField] bool isDebugActive = false;
    [SerializeField] private float damageAmount = 25f;
    [SerializeField] private float healingAmount = 25f;

    [Header("Health Options")]
    [SerializeField] private float mStartingHealth;
    [SerializeField] private float mCurrentHealth;
    private float mMaxHealth;
    public bool mIsLowHP;

    [SerializeField] public AudioClip[] SoundEffectsClips;

    public List<GameObject> SpellList;

    //[Header("Camera Options")]
    //[SerializeField] Volume postProcessVolume;
    //private Vignette vignette;
    //private Bloom bloom;
    //private ChromaticAberration chromaticAberration;
    //private ColorAdjustments colorAdjustments;
    //private MotionBlur motionBlur;

    //Coroutine _lowLifeRoutine;
    //[SerializeField] AnimationCurve _VignetteSin;

    private SkinnedMeshRenderer smr;
    private Material baseMaterial;
    private CameraEffects CameraEffect;
    [SerializeField] private UnityEvent onHit;

    void Start()
    {
        CameraEffect = GetComponent<CameraEffects>();
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        Material[] materials = smr.materials;

        baseMaterial = materials[0];

        isDebugActive = true;
        mMaxHealth = mStartingHealth;
        mCurrentHealth = mStartingHealth;

        


        //if (postProcessVolume.profile.TryGet(out Vignette vignetteEffect))
        //{
        //    vignette = vignetteEffect;
        //}
        //
        //if (postProcessVolume.profile.TryGet(out Bloom bloomEffect))
        //{
        //    bloom = bloomEffect;
        //}
        //
        //if (postProcessVolume.profile.TryGet(out ChromaticAberration chromaticEffect))
        //{
        //    chromaticAberration = chromaticEffect;
        //}
        //
        //if (postProcessVolume.profile.TryGet(out ColorAdjustments colorEffect))
        //{
        //    colorAdjustments = colorEffect;
        //}
    }

    public void OnHealed(InputAction.CallbackContext context)
    {
        GainHealth(healingAmount);
    }

    public void OnHitted(InputAction.CallbackContext context)
    {
        TakeDamage(damageAmount);
    }

    void Update()
    {
        if (GetPercentage() <= 0.25f) 
        {
            mIsLowHP = true;
            CameraEffect.LowHpCameraEffect();

            if (SoundFXManager.Instance != null && !SoundFXManager.Instance.IsSoundPlaying(SoundEffectsClips[2]))
            {
                SoundFXManager.Instance.PlaySoundFXClip(SoundEffectsClips[2], transform, 1);
            }
        }

        if (GetPercentage() > 0.25f)
        {
            SoundFXManager.Instance.StopSoundFXClip(SoundEffectsClips[2]);
        }

        if (mCurrentHealth <= 0.0f || mCurrentHealth == mMaxHealth)
        {
            CameraEffect.ResetCameraEffect();
            if (mCurrentHealth <= 0.0f)
            {
                SoundFXManager.Instance.PlaySoundFXClip(SoundEffectsClips[5], transform, 1);
                Destroy(this.gameObject);
            }
        }

        //if (isDebugActive) 
        //{
        //    if (Input.GetKeyDown(KeyCode.Q))
        //    {
        //        if (mCurrentHealth > 0.0f)
        //        {
        //            TakeDamage(damageAmount);
        //            CameraEffect.LowHpCameraEffect();
        //            Debug.Log("Vous venez de vous prendre " + damageAmount + " de dégâts.");
        //        }
        //        else 
        //        {
        //            Debug.Log("Vous ne pouvez pas vous infliger plus de dégâts.");
        //        }
        //    }
        //
        //    if(Input.GetKeyDown(KeyCode.E))
        //    {
        //        if (mCurrentHealth < mMaxHealth) 
        //        {
        //            GainHealth(healingAmount);
        //            CameraEffect.HealingHPCameraEffect();
        //            Debug.Log("Vous venez de vous soigner de " + healingAmount + " point de vie.");
        //        }
        //        else 
        //        {
        //            Debug.Log("Vous ne pouvez pas vous soigner d'avantage.");
        //        }
        //    }
        //}
    }

    public float GetCurrentHealth()
    {
        return mCurrentHealth;
    } 

    public float GetMaxHealth()
    {
        return mMaxHealth;
    }

    public UnityEvent GetUnityEventAttack()
    {
        return onHit;
    }

    public Material GetBaseMaterial()
    {
        return baseMaterial;
    }

    public float GetPercentage()
    {
        float percentage = GetCurrentHealth() / GetMaxHealth();
        return percentage;
    }

    public void TakeDamage(float damage)
    {
        if(mCurrentHealth > 0.0f)
        {
            SoundFXManager.Instance.PlaySoundFXClip(SoundEffectsClips[0], transform, 1);
            mCurrentHealth -= damage;
            CameraEffect.TakeDamageCameraEffect();
            CameraEffect.LowHpCameraEffect();
            Debug.Log("Vous venez de vous prendre " + damageAmount + " de dégâts.");
        }
        else
        {
            Debug.Log("Vous ne pouvez pas vous infliger plus de dégâts.");
        }
    }

    public void GainHealth(float health)
    {
        if (mCurrentHealth > 0.0f && mCurrentHealth < mMaxHealth)
        {
            SoundFXManager.Instance.PlaySoundFXClip(SoundEffectsClips[1], transform, 1);
            mCurrentHealth += health;
            CameraEffect.HealingHPCameraEffect();
            //CameraEffect.TakeHealthCameraEffect();
            Debug.Log("Vous venez de vous soigner de " + healingAmount + " point de vie.");

        }
        else
        {
            Debug.Log("Vous ne pouvez pas vous soigner d'avantage.");
        }
    }

    //private void ResetCameraEffect()
    //{
    //    if (vignette != null) 
    //    {
    //        vignette.color.value = new Color(255 / 255f, 255 / 255f, 255 / 255f);
    //        vignette.intensity.value = 0.0f;
    //        bloom.intensity.value = 0;
    //        chromaticAberration.intensity.value = 0;
    //        colorAdjustments.saturation.value = 0f;
    //    }
    //
    //    if(_lowLifeRoutine!=null) StopCoroutine(_lowLifeRoutine);
    //
    //}


    //private void LowHpCameraEffect()
    //{
    //    ResetCameraEffect();
    //    if (vignette != null)
    //    {
    //        float percentage = mCurrentHealth / mMaxHealth;
    //        if (percentage <= 0.25f) 
    //        {
    //            vignette.color.value =  new Color(200 / 255f, 48 / 255f, 48 / 255f);
    //            //vignette.intensity.value = 0.5f;
    //            chromaticAberration.intensity.value = 1;
    //            colorAdjustments.saturation.value = -30f;
    //
    //            _lowLifeRoutine = StartCoroutine(Tweening());
    //        }
    //    }
    //}

    //private void HealingHPCameraEffect()
    //{
    //    ResetCameraEffect();
    //    if (vignette !=null)
    //    {
    //        vignette.color.value = new Color(39 / 255f, 221 / 255f, 42 / 255f);
    //        vignette.intensity.value = 0.15f;
    //
    //        bloom.intensity.value = 5;
    //        chromaticAberration.intensity.value = 1;
    //        colorAdjustments.saturation.value = 10f;
    //    }
    //
    //    StartCoroutine(ResetAfterDelay(1.0f));
    //}

    //private void TakeDamageCameraEffect()
    //{
    //    ResetCameraEffect();
    //
    //    onHit.Invoke();
    //    baseMaterial.SetColor("_Color", Color.red);
    //    baseMaterial.SetFloat("_Effect", 1);
    //
    //    if (vignette != null)
    //    {
    //        vignette.color.value = new Color(39 / 255f, 221 / 255f, 42 / 255f);
    //        vignette.intensity.value = 0.75f;
    //    }
    //
    //    StartCoroutine(ResetMat());
    //}

    //private void TakeHealthCameraEffect()
    //{
    //    baseMaterial.SetColor("_Color", Color.green);
    //    baseMaterial.SetFloat("_Effect", 1);
    //    StartCoroutine(ResetMat());
    //}

    //private IEnumerator ResetAfterDelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay); // Attend le délai spécifié
    //    ResetCameraEffect(); // Réinitialise les effets de la caméra
    //}

    public IEnumerator ResetMat()
    {
        yield return new WaitForSeconds(0.5f);
        baseMaterial.SetFloat("_Effect", 0);
    }

    //private IEnumerator Tweening()
    //{
    //    var maxDuration = _VignetteSin.keys[^1].time;
    //
    //    while (true)
    //    {
    //        var time = Time.time % maxDuration;
    //        vignette.intensity.value = _VignetteSin.Evaluate(time);
    //        yield return null;
    //    }
    //}

}