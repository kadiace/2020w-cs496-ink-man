using System.Collections;
using UnityEngine;
using Platformer.Core;
using Platformer.Gameplay;
using Platformer.Mechanics;

public class Boss : MonoBehaviour
{

    public int life = 5;
    public float rockspeed = 3f;

    public float speed = 4f;

    //private bool isDead = false;

    bool isLeft = true;
    bool spinned = false;

    protected Rigidbody2D body;

    public GameObject rockPrefab;
    public GameObject currentRock;

    private Transform playerTransform;
    private Transform _transform;

    private bool rockThrown = false;

    private Vector3 rockOffset_left = new Vector3(-1.414307f, 1.727929f, 0f);
    private Vector3 rockOffset_right = new Vector3(-1.45953f, 1.925298f, 0f);

    private Vector2 defaultleftVector;
    private Vector2 defaultrightVector;
    private Vector3 punchTargetPosition;
    private bool hasPunchTarget = false;
    private bool punchTargetLocked = false;

    //public bool hit = false;



    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        _transform = GetComponent<Transform>();

        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        StartCoroutine("Run");

        defaultleftVector = new Vector2(_transform.right.x, _transform.right.y);
        defaultrightVector = new Vector2(-_transform.right.x, _transform.right.y);
    }

    private void Update()
    {



    }
    void setspinned()
    {
        spinned = !spinned;
    }



    IEnumerator Run()
    {
        while (true)
        {
            AnimatorStateInfo animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            Vector3 translation = (playerTransform.position - _transform.position).normalized * 1f * Time.deltaTime;

            if (!animatorStateInfo.IsName("Punch") && !animatorStateInfo.IsName("fistrotateleft") && !animatorStateInfo.IsName("fistrotateright"))
                punchTargetLocked = false;

            isLeft = translation.x < 0;
            anim.SetBool("isLeft", isLeft);

            if (animatorStateInfo.IsName("idle"))
            {


            }
            else if (animatorStateInfo.IsName("trace_left"))
            {
                float randomValue = Random.Range(0.0f, 1.0f);

                if (randomValue >= 0.98)
                {
                    if (!rockThrown)
                    {
                        Debug.Log("1234");
                        anim.SetTrigger("drawRock_left");
                        Invoke("CreateRock", 0.5f);
                        rockThrown = true;
                    }
                }
                else if (randomValue >= 0.97)
                {
                    anim.SetTrigger("fistRotation");

                }
                else
                {
                    //_transform.Translate(translation);
                    _transform.position = Vector2.MoveTowards(_transform.position, playerTransform.position, speed * Time.deltaTime);
                }



            }
            else if (animatorStateInfo.IsName("trace_right"))
            {
                float randomValue = Random.Range(0.0f, 1.0f);
                translation.x = -translation.x;
                _transform.Translate(translation);

                if (randomValue >= 0.98)
                {
                    if (!rockThrown)
                    {
                        Debug.Log("1234");
                        anim.SetTrigger("drawRock_right");
                        Invoke("CreateRock", 0.5f);
                        rockThrown = true;
                    }
                }
                else if (randomValue >= 0.97)
                {
                    anim.SetTrigger("fistRotation");
                }
                else
                {
                    //_transform.Movet
                    _transform.position = Vector2.MoveTowards(_transform.position, playerTransform.position, speed*Time.deltaTime);
                }


            }
            else if (animatorStateInfo.IsName("drawRock_left") || animatorStateInfo.IsName("drawRock_right"))
            {
                rockThrown = false;
            }
            else if (animatorStateInfo.IsName("Punch"))
            {

                RaycastHit2D[] hits;

                hits = Physics2D.RaycastAll(_transform.position, -_transform.right, 4f);

                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject.CompareTag("tile"))
                    {
                        if (spinned ^ isLeft)
                        {
                            anim.SetTrigger("PunchCollided");
                        }
                        else
                        {
                            anim.SetTrigger("needSpin");
                        }
                    }

                }

                if (!punchTargetLocked)
                    SetPunchTarget();

                if (hasPunchTarget)
                    MoveToPunchTarget();

            }
            else if (animatorStateInfo.IsName("fistrotateleft"))
            {

                if (Mathf.Abs(Vector2.SignedAngle(new Vector2(-_transform.right.x, -_transform.right.y), new Vector2(translation.x, translation.y))) < 5f)
                {
                    if (!punchTargetLocked)
                        SetPunchTarget();
                    anim.SetTrigger("PunchStart");
                }
                else
                {
                    //Debug.Log("rotating");
                    _transform.Rotate(Vector3.forward, 4f);
                }
            }
            else if (animatorStateInfo.IsName("fistrotateright"))
            {

                if (Mathf.Abs(Vector2.SignedAngle(new Vector2(-_transform.right.x, -_transform.right.y), new Vector2(translation.x, translation.y))) < 5f)
                {
                    if (!punchTargetLocked)
                        SetPunchTarget();
                    anim.SetTrigger("PunchStart");
                }
                else
                {
                    //Debug.Log("rotating");
                    _transform.Rotate(Vector3.forward, 4f);
                }
            }
            else if (animatorStateInfo.IsName("rotatetoleft"))
            {
                //(0,0,20)
                //if (!(spinned ^ isLeft)){
                //    anim.SetTrigger("needSpin");
                //}

                if (Mathf.Abs(Vector2.SignedAngle(defaultleftVector, _transform.right))<5f)
                {
                    anim.SetTrigger("rotationBeforeTrace");
                }
                else
                {
                    _transform.Rotate(Vector3.forward, 4f);
                }
            }
            else if (animatorStateInfo.IsName("rotatetoright"))
            {

                //(0,180,20)
                //if (!(spinned ^ isLeft))
                //{
                //    anim.SetTrigger("needSpin");
                //}

                if (Mathf.Abs(Vector2.SignedAngle(defaultrightVector, _transform.right)) < 5f)
                {
                    anim.SetTrigger("rotationBeforeTrace");
                }
                else
                {
                    _transform.Rotate(Vector3.forward, 4f);
                }
            }
            else if (animatorStateInfo.IsName("Dead"))
            {
                _transform.position = _transform.position - new Vector3(0, 0.1f,0);
            }
            yield return new WaitForSeconds(0.03f);
        }
    }

    void CreateRock()
    {
        currentRock = Instantiate(rockPrefab);
        Vector3 offset = isLeft ? rockOffset_left : rockOffset_right;
        currentRock.transform.position = transform.position + offset;
        currentRock.GetComponent<Rigidbody2D>().linearVelocity = rockspeed * (playerTransform.position - transform.position).normalized;
    }

    void SetPunchTarget()
    {
        punchTargetPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, _transform.position.z);
        hasPunchTarget = true;
        punchTargetLocked = true;
    }

    void MoveToPunchTarget()
    {
        _transform.position = Vector3.MoveTowards(_transform.position, punchTargetPosition, 0.3f);
        if (_transform.position == punchTargetPosition)
            hasPunchTarget = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        TryKillPlayer(collision.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryKillPlayer(collision.gameObject);
    }

    static void TryKillPlayer(GameObject hitObject)
    {
        if (hitObject.GetComponent<PlayerController>() != null)
            Simulation.Schedule<PlayerDeath>(0);
    }








}
