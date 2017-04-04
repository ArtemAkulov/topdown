using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UnityEngine.AI.NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State {Idle, Chasing, Attacking};
    State currentState;

    public ParticleSystem deathEffect;
    public ParticleSystem hitEffect;

    public static event System.Action OnDeathStatic;

    public AudioClip spawnSound;
    public AudioClip damageSound;
    public AudioClip deathSound;

    UnityEngine.AI.NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;

    Material enemyMaterial;
    Material attackMaterial;
    Color originalColor;
    Color attackColor = Color.black;

    float attackDistanceThreshold = .5f;
    float timeBetweenAttacks = 1;
    float nextAttackTime;
    float damage = 1;

    float enemyCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    private void Awake() {
        pathfinder = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            enemyCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }

    protected override void Start () {
        base.Start();
        if (hasTarget) {
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTargetDeath;
            StartCoroutine(UpdatePath());
            AudioManager.audioManager.PlaySound(spawnSound, transform.position);
        }
	}

    public void SetParameters(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor) {
        pathfinder.speed = moveSpeed;
        if (hasTarget) {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;

        ParticleSystem.MainModule deathEffectSettings = deathEffect.main;
        ParticleSystem.MainModule hitEffectSettings = hitEffect.main;
        deathEffectSettings.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1);
        hitEffectSettings.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1);
        enemyMaterial = GetComponent<Renderer>().material;
        attackMaterial = GetComponent<Renderer>().material;
        enemyMaterial.color = skinColor;
        originalColor = enemyMaterial.color;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= health && !dead) {
            if (OnDeathStatic != null) {
                OnDeathStatic();
            }
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);
            AudioManager.audioManager.PlaySound(deathSound, transform.position);
        }
        else {
            Destroy(Instantiate(hitEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);
            AudioManager.audioManager.PlaySound(damageSound, transform.position);
        }
        
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    void OnTargetDeath() {
        hasTarget = false;
        currentState = State.Idle;
    }

	void Update () {
        if (hasTarget) {
            if (Time.time > nextAttackTime) {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + enemyCollisionRadius + targetCollisionRadius, 2)) {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
	}

    IEnumerator Attack() {

        pathfinder.enabled = false;
        currentState = State.Attacking;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * enemyCollisionRadius;
        
        float attackSpeed = 3;
        float percent = 0;

        bool hasAppliedDamage = false;

        attackMaterial.color = attackColor;

        while (percent <= 1) {
            if (percent >= .5f && !hasAppliedDamage) {
                hasAppliedDamage = true;
                AudioManager.audioManager.PlaySound("Player Damage", transform.position);
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        attackMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath() {
        float refreshRate = .25f;
        while (hasTarget) {
            if (currentState == State.Chasing) {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (enemyCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate); 
        }
    }
}
