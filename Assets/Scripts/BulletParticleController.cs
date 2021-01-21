using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticleController : MonoBehaviour
{

    public float speed = 10f;

    private List<Vector3> Points;

    private ParticleSystem PC;

    private float LifeTime;
    private float StartTime;
    // Start is called before the first frame update
    void Start()
    {
        PC = this.gameObject.GetComponent<ParticleSystem>();
        LifeTime = PC.main.startLifetime.Evaluate(0);

    }

    private void OnEnable()
    {
        StartTime = Time.time;
    }
    private void FixedUpdate()
    {
        if (Points.Count > 0)
        {
            var emission = PC.emission;
            emission.enabled = true;
            transform.position = Vector3.MoveTowards(transform.position, Points[0], 500f * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, Points[0]) < 1)
            {
                Points.RemoveAt(0);
            }
        } else
        {
            var emission = PC.emission;
            emission.enabled = false;
        }

        
        if(Time.time - StartTime > LifeTime)
        {
            Destroy(this.gameObject);
        }
        

    }
    // Update is called once per frame
    void Update()
    {
        
    }


    public void FollowPoints(List<Vector3> points)
    {
        Points = points;
        transform.position = points[0];
    }
}
