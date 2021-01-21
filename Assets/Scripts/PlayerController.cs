using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IKillable
{
    public float Speed = 10f;

    private Rigidbody2D RigidBody;
    private GameObject LookPivot;
    private GameObject GameLogic;
    private Animator anim;

    private AudioSource AudioSrc;
   
    public CameraShake ShakeScript;

    private LineRenderer AimLaser;
    

    // Start is called before the first frame update
    void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        LookPivot = transform.GetChild(0).gameObject;
        GameLogic = GameObject.Find("Game Logic");
        anim = GetComponent<Animator>();
        AudioSrc = GetComponent<AudioSource>();


        AimLaser = GetComponent<LineRenderer>();

    }

    void FixedUpdate()
    {
        
    }

    void Update()
    {
        Move();
        Look();
        if(Input.GetMouseButtonDown(0) == true)
        {
            
            Shoot(LookPivot.transform.GetChild(0).transform.position, LookPivot.transform.right);
        }
        DrawLaser(LookPivot.transform.GetChild(0).transform.position, LookPivot.transform.right, 5, 1);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Kill();
        }
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");


        Vector2 netVelocity = new Vector2(horizontal, vertical);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            netVelocity = netVelocity.normalized * Speed * 0.30f;
        } else
        {
            netVelocity = netVelocity.normalized * Speed;
        }
        
        RigidBody.velocity = netVelocity;
    }


    private void Look()
    {
        // Current mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Current mouse position centered around current position
        Vector3 lookPos = mousePos - transform.position;

        // Get angle of mouse position centered around current position
        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
        
        if(angle > 90 || angle < -90)
        {
            LookPivot.GetComponent<Transform>().localScale = new Vector3(1, -1);
        } else
        {
            LookPivot.GetComponent<Transform>().localScale = new Vector3(1, 1);
        }

        LookPivot.GetComponent<Transform>().rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Shoot(Vector3 startPos, Vector3 rotation)
    {


        RaycastHit2D hit = Physics2D.Raycast(startPos, rotation);
        if(Vector3.Distance(startPos, hit.point) < .1f)
        {
            return;
        }
        AudioSrc.Play();
        anim.SetTrigger("Shoot");
        StartCoroutine(ShakeScript.Shake(.1f, .3f, .8f));
        List<Vector3> points = new List<Vector3>();
        points.Add(startPos);
        Shoot(startPos, rotation, points, 0);

    }

    private void Shoot(Vector3 startPos, Vector3 rotation, List<Vector3> points, int reflections)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, rotation);
        if (hit)
        {
            points.Add(hit.point);
            if (hit.collider.tag.Equals("Reflector"))
            {
                IDamageable obj = hit.collider.GetComponent<WallController>();
                obj.Damage(1);
                Shoot(hit.point + hit.normal * 0.01f, Vector3.Reflect(rotation, hit.normal), points, reflections + 1);
            } else if (hit.collider.tag.Equals("Enemy") && reflections > 0)
            {
                DrawShootEffect(points);

                IKillable obj = hit.collider.GetComponent<EnemyController>();
                obj.Kill();
                GameController.SharedInstance.ScorePoints(GameController.SharedInstance.ScorePerKill, reflections);
            } else
            {
                DrawShootEffect(points);
            }



        }

    }

    public void DrawLaser(Vector3 startPos, Vector3 rotation, int reflections, int index)
    {

        AimLaser.SetPosition(index - 1, startPos);
        
        RaycastHit2D hit = Physics2D.Raycast(startPos, rotation);
        if (hit)
        {
            if (AimLaser.positionCount <= index)
            {
                AimLaser.positionCount += 1;

            }

            if(AimLaser.positionCount - index > 1)
            {
                AimLaser.positionCount -= 1;
            }
            AimLaser.SetPosition(index - 1, startPos);
            AimLaser.SetPosition(index, hit.point);

            if (hit.collider.tag.Equals("Reflector") && reflections > 0)
            {
                DrawLaser(hit.point + hit.normal * 0.01f, Vector3.Reflect(rotation, hit.normal), reflections - 1, index + 1);
            }
        }


    }

    public void DrawShootEffect(List<Vector3> points)
    {
        GameObject emitter = Instantiate(GameLogic.GetComponent<GameController>().ShootParticles);
        if (emitter != null)
        {
            emitter.SetActive(true);
            emitter.GetComponent<BulletParticleController>().FollowPoints(points);
        }
    }

    public void Kill()
    {
        
        Instantiate(GameLogic.GetComponent<GameController>().DeathParticles, transform.position, Quaternion.identity);
        

        this.gameObject.SetActive(false);
        GameLogic.GetComponent<GameController>().EndGame();
    }
}
