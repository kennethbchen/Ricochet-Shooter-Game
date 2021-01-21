using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreFeedBackController : MonoBehaviour
{
    public float speed;
    public float height;

    private Vector3 InitialPosition;
    private Vector3 TargetPosition;
    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = transform.position;
        TargetPosition = new Vector3(transform.position.x, transform.position.y + height, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, TargetPosition) >= 0.1)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, speed * Time.fixedDeltaTime);
        } else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetScore(int score, int multiplier)
    {
        this.GetComponent<Text>().text =  "+" + (score * multiplier).ToString() + " (x" + multiplier + ")";
    }
}
