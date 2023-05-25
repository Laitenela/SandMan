using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isOver;
    public float speed = 5.0f;
    public float getVertical;
    public float getHorizontal;
    private bool nonDamage = false;
    public float health = 100;
    public AudioClip[] clipAudio;

    void Update()
    {
        if (health > 0)
        {
            //Set the rotation from the direction
            SetAngle();

            //Change Animator parameters
            SetAnimatorParamters();

            if (Input.GetKey(KeyCode.Mouse0))
            {
                gameObject.GetComponent<Animator>().SetBool("Attack", true);
                StopCoroutine("Steps");
            }
            else
            {
                gameObject.GetComponent<Animator>().SetBool("Attack", false);
            }

            if (!gameObject.GetComponent<AudioSource>().isPlaying
                && (gameObject.GetComponent<Animator>()
                              .GetCurrentAnimatorStateInfo(0)
                              .IsName("Attack1") || gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack2")))
            {
                gameObject.GetComponent<AudioSource>().Play();
            }

            if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Run") && !gameObject.GetComponent<AudioSource>().isPlaying && (Mathf.Abs(getVertical) == 1 || Mathf.Abs(getHorizontal) == 1))
            {
                gameObject.GetComponent<AudioSource>().PlayOneShot(clipAudio[1]);
            }
        }
        else if (!isOver)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().GameOver(); isOver = true;
        }
    }

    private void FixedUpdate()
    {
        if (health > 0)
        {
            //Change the position by input axes
            if (!gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack1") && !gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
            {
                PositionControl();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")
            && (gameObject.GetComponent<Animator>()
                          .GetCurrentAnimatorStateInfo(0)
                          .IsName("Attack1") || gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack2")))
        {
            other.gameObject.GetComponent<EnemyController>().GetHit(20);
        }
    }


    //Set the rotation from the direction
    private void SetAngle()
    {
        //A sharp turn
        if (getHorizontal > 0 && getVertical == 0) { getHorizontal = 1; }
        else if (getHorizontal < 0 && getVertical == 0) { getHorizontal = -1; }

        //Calculate the rotation
        float angle = getVertical <= 0
            ? 180 + (Mathf.Asin(getHorizontal) / Mathf.Acos(-Mathf.Abs(getVertical)) * 90)
            : -Mathf.Asin(getHorizontal) / Mathf.Acos(-Mathf.Abs(getVertical)) * 90;

        //Change the calculated angle
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
        }
    }

    //Change the position by input axes
    private void PositionControl()
    {
        //Change variables by input axes
        getVertical = -Input.GetAxis("Vertical");
        getHorizontal = Input.GetAxis("Horizontal");

        //Set position
        transform.Translate(Vector3.left * speed * getHorizontal * Time.deltaTime, Space.World);
        transform.Translate(Vector3.forward * speed * getVertical * Time.deltaTime, Space.World);
    }

    //Change Animator parameters
    private void SetAnimatorParamters()
    {
        //Send a speed variable
        gameObject.GetComponent<Animator>().SetFloat("Speed", Mathf.Abs(getHorizontal) + Mathf.Abs(getVertical));
    }

    public void GetDamage(GameObject enemy)
    {
        if (!nonDamage)
        {
            Vector3 betweenVector = transform.position - enemy.transform.position;
            gameObject.GetComponent<Rigidbody>().AddForce(betweenVector.normalized * 100, ForceMode.Force);
            gameObject.GetComponent<Animator>().SetTrigger("Damaged");
            StartCoroutine(NonHit());
            gameObject.GetComponent<AudioSource>().PlayOneShot(clipAudio[0]);
            health -= 20;
            if (health <= 0) { gameObject.GetComponent<Animator>().SetBool("Dead", true); }
        }
    }

    private IEnumerator NonHit()
    {
        nonDamage = true;
        yield return new WaitForSeconds(0.5f);
        nonDamage = false;
        StopCoroutine("NonHit");
    }
}
