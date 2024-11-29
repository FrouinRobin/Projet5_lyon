using RPGCharacterAnims.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyController : MonoBehaviour
{
    [Header("Debug Option")]
    [Tooltip("Si activée, permet d'activer les fonctionnalitées de debug.")]
    [SerializeField] bool isDebugActive = false;

    [SerializeField] InputActionReference MoveRef;
    [SerializeField] InputActionReference AttackRef;
    [SerializeField] InputActionReference UltimateRef;
    [SerializeField] InputActionReference DodgeRef;
    [SerializeField] InputActionReference hittedRef;
    [SerializeField] InputActionReference HealedRef;
    [SerializeField] InputActionReference SpellRef;

    [SerializeField] Animator animator;
    //[SerializeField] GameManager gameManager;

    private GameObject hitZone;

    [SerializeField] float moveSpeed = 1000f;

    [SerializeField] int ultCharge = 90;
    private bool canUseUltimate = true;

    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction ultimateAction;
    private InputAction dodgeAction;
    private InputAction hittedAction;
    private InputAction healedAction;
    private InputAction spellAction;
    private Vector2 moveInput;
    private Rigidbody rb;
    private bool isGrounded = true;

    private PlayerManager playerManager;

    [SerializeField] private CameraEffects CameraEffect;
    [SerializeField] private CutsceneController CutsceneController;

    [SerializeField] private RotateAround rotateAroundScript;
    private bool isAFK = false; // Indique si le joueur est AFK
    private Coroutine afkCoroutine; // Référence de la coroutine d'AFK
    private float afkTimeThreshold = Mathf.Infinity; // Temps en secondes avant de considérer le joueur AFK

    [SerializeField] List<Slash> slashes;

    private void Start()
    {
        playerManager = gameObject.GetComponent<PlayerManager>();
        hitZone = GameObject.FindGameObjectWithTag("HitZone");
        rb = GetComponent<Rigidbody>();
        hitZone.SetActive(false);


        MoveRef.action.actionMap.Enable();
        AttackRef.action.actionMap.Enable();
        UltimateRef.action.actionMap.Enable();
        DodgeRef.action.actionMap.Enable();
        SpellRef.action.actionMap.Enable();
        hittedRef.action.actionMap.Enable();
        HealedRef.action.actionMap.Enable();
        SpellRef.action.actionMap.Enable();

        moveAction = MoveRef.action.actionMap.FindAction("Move");
        attackAction = AttackRef.action.actionMap.FindAction("Attack");
        ultimateAction = UltimateRef.action.actionMap.FindAction("Ultimate");
        dodgeAction = DodgeRef.action.actionMap.FindAction("Dodge");
        hittedAction = hittedRef.action.actionMap.FindAction("Hitted");
        healedAction = HealedRef.action.actionMap.FindAction("Healed");
        spellAction = SpellRef.action.actionMap.FindAction("Spell");

        attackAction.performed += OnAttack;
        ultimateAction.performed += OnUltimate;
        dodgeAction.performed += OnDodge;
        hittedAction.performed += playerManager.OnHitted;
        healedAction.performed += playerManager.OnHealed;
        spellAction.performed += OnSpellCast;


        StartCoroutine(ChargeUltimate());

        DisableSlashes();
    }



    private void FixedUpdate()
    {
        Debug.Log(afkTimeThreshold);
        moveInput = moveAction.ReadValue<Vector2>();

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        rb.AddForce(moveDirection * moveSpeed);
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            if (SoundFXManager.Instance != null && !SoundFXManager.Instance.IsSoundPlaying(GetComponent<PlayerManager>().SoundEffectsClips[3]))
            {
                SoundFXManager.Instance.PlaySoundFXClip(GetComponent<PlayerManager>().SoundEffectsClips[3], transform, 1);
            }

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            if (isDebugActive || CutsceneController.IsInCutscene == true)
            {
                afkTimeThreshold = Mathf.Infinity;
            }
            else if (isDebugActive == false || CutsceneController.IsInCutscene == false)
            {
                afkTimeThreshold = 3;
            }

            if (isAFK)
            {
                StopAFK();
            }
        }
        else if(moveDirection.sqrMagnitude == 0)
        {
            if (!isAFK) 
            {
                StartAFK();
            }
        }

        UpdateAnimations(moveDirection.magnitude);
    }

    private void StartAFK()
    {
        isAFK = true;
        afkCoroutine = StartCoroutine(AFKTimer());
    }

    private void StopAFK()
    {
        isAFK = false;

        if (afkCoroutine != null)
        {
            StopCoroutine(afkCoroutine);
            afkCoroutine = null;
        }

        if (rotateAroundScript != null)
        {
            rotateAroundScript.StopRotation();
        }
    }

    private IEnumerator AFKTimer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < afkTimeThreshold)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Le joueur est officiellement AFK après le seuil
        Debug.Log("Le joueur est AFK !");
        PerformAFKAction(); // Appeler l'action AFK
    }

    private void PerformAFKAction()
    {
        // Action à effectuer lorsque le joueur est AFK
        Debug.Log("Action AFK déclenchée !");

        if (rotateAroundScript != null)
        {
            rotateAroundScript.StartRotation(50f);
        }
    }

    private void UpdateAnimations(float speed)
    {
        bool isWalking = speed > 0.1f;

        
        animator.SetBool("Walking", isWalking);

    }

    private void OnSpellCast(InputAction.CallbackContext context)
    {
        if (context.control.name == "1")
        {
            Instantiate(playerManager.SpellList[0], gameObject.transform.position + gameObject.transform.forward * 5f, gameObject.transform.rotation);
            SoundFXManager.Instance.PlaySoundFXClip(GetComponent<PlayerManager>().SoundEffectsClips[7], transform, 1f);
        }
        else if (context.control.name == "2")
        {
            Instantiate(playerManager.SpellList[1], new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1f, gameObject.transform.position.z), gameObject.transform.rotation);
            SoundFXManager.Instance.PlaySoundFXClip(GetComponent<PlayerManager>().SoundEffectsClips[8], transform, 1f);

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f) // Ensure the collision is from below
            {
                isGrounded = true;
                break;
            }
        }
    }

    private void OnDisable()
    {
        attackAction.performed -= OnAttack;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Attacking");
        StartCoroutine(EnableSlashes());
        StartCoroutine(EnableHitZone(0.5f));

        if (isAFK)
        {
            StopAFK();
        }
    }

    private void OnUltimate(InputAction.CallbackContext context)
    {
        if (canUseUltimate && ultCharge >= 100)
        {
            animator.SetTrigger("Ultimate");
            StartCoroutine (EnableHitZone(0.7f));
            ultCharge = 0;
            canUseUltimate = false;
            StartCoroutine(RechargeUltimate());

            if (isAFK)
            {
                StopAFK();
            }
        }
    }

    private void OnDodge(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Dodge");
        CameraEffect.DodgeCameraEffect();
        SoundFXManager.Instance.PlaySoundFXClip(GetComponent<PlayerManager>().SoundEffectsClips[4], transform, 1);


        // Si le joueur ne se déplace pas, appliquez une force vers l'avant
        if (moveInput.sqrMagnitude < 0.01f)
        {
            StartCoroutine(DodgeWithDelay(0.19f));
        }
    }
    
    private IEnumerator EnableHitZone(float time)
    {
        yield return new WaitForSeconds(time);
        hitZone.SetActive(true);
        yield return new WaitForSeconds(time);
        hitZone.SetActive(false);
    }

    private IEnumerator ChargeUltimate()
    {
        while (true)
        {
            if (ultCharge < 100)
            {
                ultCharge++;
                yield return new WaitForSeconds(0.1f); // 30 seconds to reach 100
            }
            else
            {
                yield return null; // Wait if ultimate is fully charged
            }
        }
    }

    private IEnumerator RechargeUltimate()
    {
        yield return new WaitForSeconds(10f); // Recharge cooldown
        canUseUltimate = true;
    }

    private IEnumerator DodgeWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.AddForce(transform.forward * moveSpeed * 0.5f, ForceMode.Impulse);
    }

    public IEnumerator EnableSlashes()
    {
        foreach (var slash in slashes)
        {
            yield return new WaitForSeconds(slash.delay);
            slash.slashObj.SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        DisableSlashes();
        //attacking = false;
    }

    public void DisableSlashes()
    {
        foreach (var slash in slashes)
        {
            slash.slashObj.SetActive(false);
        }
    }
}

[System.Serializable]
public class Slash
{
    public GameObject slashObj;
    public float delay;
}
