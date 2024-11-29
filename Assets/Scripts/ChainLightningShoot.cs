using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChainLightningShoot : MonoBehaviour
{
    [SerializeField] float refreshRate = 0.01f;
    [SerializeField][Range(1, 10)] int maximumEnemiesInChain = 3;
    [SerializeField] float delayBetweenEachChain = 0.5f;
    [SerializeField] Transform playerFirepoint;
    [SerializeField] EnemyDetector playerEnemyDetector;
    [SerializeField] GameObject trailRendererPrefab;
    [SerializeField] GameObject activePrefab;

    [SerializeField] InputActionReference SkillRef;

    private InputAction skillAction;

    private bool shooting;
    private List<GameObject> spawnedLineRenderers = new List<GameObject>();
    List<GameObject> activeEffects = new List<GameObject>();

    private Coroutine shootingCoroutine;

    private void Start()
    {
        if (SkillRef == null || SkillRef.action == null)
        {
            Debug.LogError("SkillRef or SkillRef.action is not assigned.");
            return;
        }
        var actionMap = SkillRef.action.actionMap;
        if (actionMap == null)
        {
            Debug.LogError("SkillRef.action.actionMap is not found.");
            return;
        }
        actionMap.Enable();
        skillAction = actionMap.FindAction("Skill1");
        if (skillAction != null) skillAction.performed += OnSkill;
    }

    void Update()
    {
    }

    private void OnSkill(InputAction.CallbackContext context)
    {
        if (!shooting)
        {
            ActivateAllEnemyColliders();

            StartShooting();
        }
    }

    void StartShooting()
    {
        shooting = true;
        shootingCoroutine = StartCoroutine(ProcessChainLightning());
    }

    void StopShooting()
    {
        shooting = false;
        if (shootingCoroutine != null)
            StopCoroutine(shootingCoroutine);

        shootingCoroutine = null;

        DeactivateAllEnemyColliders();
    }

    IEnumerator ProcessChainLightning()
    {
        yield return Lightning(gameObject, null, new HashSet<GameObject>(), maximumEnemiesInChain+1);
        StopShooting();

        DeactivateAllEnemyColliders();
    }

    IEnumerator Lightning(GameObject currentTarget, GameObject previousTarget, HashSet<GameObject> historique, int remainingChains)
    {
        if (remainingChains <= 0 || currentTarget == null || historique.Contains(currentTarget))
        {
            DeactivateAllEnemyColliders();
            yield break;
        }

        historique.Add(currentTarget);

        if (previousTarget != null)
        {
            NewTrailRenderer(previousTarget.transform, currentTarget.transform);
            var enemy = currentTarget.GetComponent<MyDummy>();
            if (enemy != null)
            {
                yield return new WaitForSeconds(delayBetweenEachChain);
                enemy.TakeDamage(25, gameObject.tag);
                SoundFXManager.Instance.PlaySoundFXClip(GetComponent<PlayerManager>().SoundEffectsClips[6], transform, 1f);
            }
        }

        yield return new WaitForSeconds(delayBetweenEachChain);
      
        var detector = currentTarget.GetComponent<EnemyDetector>();
        if (detector == null || !detector.HasTargets)
        {
            DeactivateAllEnemyColliders();
            yield break;
        }

        var nextTarget = detector.GetClosestEnemies().FirstOrDefault(i => !historique.Contains(i));
        if (nextTarget == null)
        {
            DeactivateAllEnemyColliders();
            yield break;
        }

        yield return Lightning(nextTarget, currentTarget, historique, remainingChains - 1);
    }

    void NewTrailRenderer(Transform startPos, Transform endPos)
    {
        if (endPos == null) return;

        GameObject trail = Instantiate(trailRendererPrefab, startPos.position, Quaternion.identity);
        spawnedLineRenderers.Add(trail);

        StartCoroutine(MoveTrailToTarget(trail, startPos, endPos));

        GameObject activeVFX = Instantiate(activePrefab, endPos.position, Quaternion.identity);
        activeEffects.Add(activeVFX);

    }

    IEnumerator MoveTrailToTarget(GameObject trail, Transform startPos, Transform endPos)
    {
        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration && trail != null && endPos != null)
        {
            elapsedTime += Time.deltaTime;

            trail.transform.position = Vector3.Lerp(startPos.position, endPos.position, elapsedTime / duration);

            yield return null;
        }

        if (trail != null) Destroy(trail);
    }

    void ActivateAllEnemyColliders()
    {
        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in allEnemies)
        {
            var collider = enemy.GetComponent<SphereCollider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }

    void DeactivateAllEnemyColliders()
    {
        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in allEnemies)
        {
            var collider = enemy.GetComponent<SphereCollider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
    }
}
