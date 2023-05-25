using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private int selfHealth = 100;
    public bool fightEnemy = false;
    private bool isSleeping = true;
    private Transform playerTransform;
    private NavMeshAgent meshAgent;
    private Animator animator;
    private Rigidbody selfRb;
    private bool nonDamage = false;
    public AudioClip[] clipAudio;
    public bool clone;

    public float rangeAttack = 2.5f;
    public float rangeTrigger = 6;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        meshAgent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
        selfRb = gameObject.GetComponent<Rigidbody>();
        if (fightEnemy) { animator.SetBool("Fight", true); }
    }

    // Update is called once per frame
    void Update()
    {
        if (selfHealth > 0 && GameObject.Find("Player").GetComponent<PlayerController>().health > 0)
        {
            if (PlayerInRange(rangeAttack))
            {
                Attack();
            }
            else if (PlayerInRange(rangeTrigger))
            {
                FightTrigger();
            }
            else if (fightEnemy)
            {
                MoveToPlayer();
            }

            animator.SetBool("Run", true);
        }

        if(GameObject.Find("Player").GetComponent<PlayerController>().health <= 0)
        {
            animator.SetTrigger("PlayerDead");
        }
    }

    private void Attack()
    {
        RotateToPlayer();
        if (PlayerInRange(rangeAttack - 0.2f))
        {
            animator.SetBool("Run", false);
        }
        meshAgent.isStopped = true;
        animator.SetTrigger("Attack");
    }

    private void RotateToPlayer()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
        {
            Vector3 betweenVector = playerTransform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(betweenVector);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1 * Time.deltaTime);
        } else
        {
            Vector3 betweenVector = playerTransform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(betweenVector);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 4 * Time.deltaTime);
        }
    }

    private void FightTrigger()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
        {
            meshAgent.SetDestination(playerTransform.position);
            animator.SetBool("Run", true);
            meshAgent.isStopped = false;
            animator.SetBool("Fight", true);
            animator.ResetTrigger("Attack");
            fightEnemy = true;

            if (PlayerInRange(rangeAttack + 0.2f))
            {
                RotateToPlayer();
            }

            if (isSleeping)
            {
                isSleeping = false;
                gameObject.GetComponent<AudioSource>().Play();
            }
        }
    }

    private bool PlayerInRange(float distanse)
    {
        float playerPostionX = playerTransform.position.x;
        float selfPositionX = transform.position.x;
        float distanseX = Mathf.Abs(playerPostionX - selfPositionX);

        float playerPostionZ = playerTransform.position.z;
        float selfPositionZ = transform.position.z;
        float distanseZ = Mathf.Abs(playerPostionZ - selfPositionZ);

        return distanseX <= distanse
            && distanseZ <= distanse;
    }

    private void MoveToPlayer()
    {
        meshAgent.SetDestination(playerTransform.position);
    }

    public void GetHit(int damage)
    {
        if (!nonDamage)
        {
            selfHealth -= damage;
            gameObject.GetComponent<AudioSource>().PlayOneShot(clipAudio[0]);
            Vector3 betweenVector = transform.position - playerTransform.position;
            selfRb.AddForce(betweenVector.normalized * 10, ForceMode.Impulse);
            gameObject.GetComponent<Animator>().SetTrigger("Hit");
            nonDamage = true;
            StartCoroutine(NonHit());
            if (selfHealth <= 0)
            {
                gameObject.GetComponent<Animator>().SetBool("Dead", true);
                meshAgent.isStopped = true;
                animator.SetBool("Run", false);
                Destroy(this.gameObject, 2);
            }
        }
    }

    private IEnumerator NonHit()
    {
        yield return new WaitForSeconds(1f);
        nonDamage = false;
        StopCoroutine("NonHit");
    }

    public void DealDamage()
    {
        if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
        {
            GameObject.Find("Player").GetComponent<PlayerController>().GetDamage(gameObject);
        }
    }

}
