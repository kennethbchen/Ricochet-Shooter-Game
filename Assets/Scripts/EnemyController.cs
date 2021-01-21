using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IKillable
{
    private Rigidbody2D RB;
    private AudioSource AS;
    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        AS = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {

        if(GameObject.Find("Player") != null)
        {
            GameObject player = GameObject.Find("Player");
            RB.MovePosition(Vector3.MoveTowards(transform.position, player.transform.position, 1f * Time.deltaTime));

            float angle = Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x) * Mathf.Rad2Deg;

            RB.rotation = angle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Kill()
    {

        Instantiate(GameObject.Find("Game Logic").GetComponent<GameController>().DeathParticles, transform.position, Quaternion.identity);
        this.gameObject.SetActive(false);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag.Equals("Player"))
        {
            IKillable player = collision.gameObject.GetComponent<PlayerController>();
            player.Kill();
        }
    }
}
