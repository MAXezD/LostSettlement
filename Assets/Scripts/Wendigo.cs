using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Wendigo : Singleton<Wendigo>
{

    NavMeshAgent agent;

    Transform player;

    Animator animator;

    public LayerMask whatIsPlayer;
    

    [Header("Patrolling")]
    public float walkSpeed;
    public float chaseSpeed;
    public float walkPointRange;
    [SerializeField] private float walkTime = 0f;
    public float maxWalkTime;

    [Header("State")]
    public float checkRange;
    public float chaseRange;
    public bool playerInCheckRange,playerInChaseRange;

    [Header("Stunned")]
    public int bulletItTakes;
    private int _bulletItTakes;
    private int bulletReceive;
    public float stunTime;
    bool stunned;
    [SerializeField] bool killable;
    bool death;

    [Header("Attack")]
    [SerializeField] float maxMashTime;
    [SerializeField] float deathTime;
    [SerializeField] Image struggleUI;
    public CinemachineVirtualCamera playerCam;
    public CinemachineVirtualCamera jumpScareCam;
    float mashTime;
    float mashDelay = 0.5f;
    public bool startedAttack;
    bool pressed;

    [Header("Sound")]
    [SerializeField] AudioSource sawScream;


    bool runCheckCalled;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        _bulletItTakes = bulletItTakes;
    }

    // Update is called once per frame
    void Update()
    {

        if (death) return;
        Animation();
        RunCheck();
        ChangeCam();

        playerInCheckRange = Physics.CheckSphere(transform.position , checkRange , whatIsPlayer);
        playerInChaseRange = Physics.CheckSphere(transform.position , chaseRange , whatIsPlayer);

        if (!playerInCheckRange && !playerInChaseRange) GetClose();
        if (playerInCheckRange && !playerInChaseRange) Patrol();
        if (playerInCheckRange && playerInChaseRange) Chase();

        walkTime += Time.deltaTime;
        //player struggle
        if (startedAttack)
        {
            struggleUI.gameObject.SetActive(true);
            struggleUI.fillAmount = mashTime/maxMashTime;

            mashTime -= Time.deltaTime;
            if (mashTime < 0) mashTime = 0;

            if (Input.GetMouseButtonDown(0) && !pressed)
            {
                pressed = true;
                mashTime += mashDelay;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                pressed = false;
            }
            if (mashTime >= maxMashTime && Gun.Instance.currentNumOfBulletInMag != 0)
            {
                _bulletItTakes = bulletItTakes;
                bulletItTakes = 1;
                Gun.Instance.Shoot();
                StopAllCoroutines();

                struggleUI.gameObject.SetActive(false);

                startedAttack = false;
                mashTime = 0;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (death) return;
        if (other.gameObject.CompareTag("Bullet"))
        {
            StartCoroutine(Stunned());
        }
        if (other.gameObject.CompareTag("Player") && !stunned)
        {
            startedAttack = true;
            StartCoroutine(Attack());
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (death) return;
        if (other.gameObject.CompareTag("Player") && !stunned)
        {
            sawScream.Play();
        }
    }
    public void GetClose()
    {
        if (stunned) return;
        agent.SetDestination(player.position);
    }
    
    public void Patrol()
    {
        if (stunned) return;
        agent.speed = walkSpeed;
        if ((agent.remainingDistance <= agent.stoppingDistance) || (walkTime >= maxWalkTime)) //done with path
        {
            walkTime = 0f;
            agent.SetDestination(transform.position); // resets walkpoint
            Vector3 point;
            if (RandomPoint(transform.position, walkPointRange, out point)) //pass in our centre point and radius of area
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                agent.SetDestination(point);
            }
        }

    }
    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    
    public void Chase()
    {
        if (stunned) return;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }
    
    private void Animation()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        animator.SetBool("IsStunned", stunned);
    }

    public void Evolve(int skulls)
    {
        Debug.Log("Evo");
        animator.SetTrigger("Evolve");  
        if (skulls == 6)
        {
            killable = true;
        }
        else
        {
            chaseSpeed += 0.5f;
            walkSpeed += 0.5f;
            stunTime -= 0.5f;
        }
    }
    public void ExtraSenses(string action,bool doing)
    {
        switch (action)
        {
            case "Flashlight":
                if (doing)
                {
                    chaseRange += 2f;
                }
                else
                {
                    chaseRange -= 2f;
                }
                break;
            case "Run":
                if (doing)
                {
                    checkRange -= 4f;
                }
                else
                {
                    checkRange += 4f;
                }
                break;
        }
        
    }
    private IEnumerator Stunned()
    {
        Debug.Log("stunned");
        bulletReceive++;
        if (bulletReceive == bulletItTakes)
        {
            if (killable)
            {
                agent.SetDestination(transform.position);
                animator.SetTrigger("Death");
                death = true;
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.isKinematic = true;
                yield return new WaitForSeconds(5f);
                GameManager.Instance.Win();
                yield break;
            }
            
            stunned = true;
            agent.SetDestination(transform.position);
            yield return new WaitForSeconds(stunTime);   
            stunned = false;
            bulletItTakes = _bulletItTakes; //use for bypassing when cancel attack
            bulletReceive = 0;
        }
        else
        {
            yield return new WaitForSeconds(5f);
            bulletReceive--;
        }
        if (bulletReceive <= 0) bulletReceive = 0;
    }
    private void RunCheck()
    {
        if (PlayerControl.Instance.State == PlayerControl.MovementState.sprint && !runCheckCalled)
        {
            runCheckCalled = true;
            ExtraSenses("Run", true);
        }
        else if (PlayerControl.Instance.State != PlayerControl.MovementState.sprint && runCheckCalled)
        {
            runCheckCalled = false;
            ExtraSenses("Run", false);
        }
    }
    private void ChangeCam()
    {
        if (startedAttack)
        {
            playerCam.Priority = 1;
            jumpScareCam.Priority = 10;
        }
        else
        {
            playerCam.Priority = 10;
            jumpScareCam.Priority = 1;
        }
    }
    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(deathTime);
        Debug.Log("dead");
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1.45f);
        player.gameObject.SetActive(false); 
        
    }

    private void OnDrawGizmosSelected()
    {
        //Shows enemy sight range in Scene window
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRange);
        //Shows enemy attack range in Scene window
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }


}
