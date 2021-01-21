using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController SharedInstance;

    public GameObject DeathScreen;
    public GameObject DeathParticles;
    public GameObject ShootParticles;
    public GameObject ScoreFeedback;
    public CameraShake ShakeScript;

    public float SpawnRadius;
    public int ScorePerKill;

    private int Score;
    private float RunTime;
    
    private GameObject Player;
    private GameObject ScoreText;
    private GameObject FinalTimeText;
    private GameObject FinalScoreText;

    // Start is called before the first frame update
    void Start()
    { 
        Score = 0;
        RunTime = 0;

        SharedInstance = this;
        Player = GameObject.Find("Player");
        ScoreText = GameObject.Find("Score");
        FinalTimeText = GameObject.Find("Time");
        FinalScoreText = GameObject.Find("FinalScore");

        // Randomize all wall positions and rotation
        while (ObjectPooler.SharedInstance.GetPooledObject("Reflector") != null)
        {

            GameObject obj = ObjectPooler.SharedInstance.GetPooledObject("Reflector");
            obj.transform.rotation = Quaternion.Euler(0, 0, GetRandomAngle());
            obj.transform.position = GetRandomPosition();
            obj.SetActive(true);
        }

    }

    private void FixedUpdate()
    {
        RunTime += Time.deltaTime;

        ScoreText.GetComponent<Text>().text = Score.ToString();
        if (Random.value <= 0.01)
        {
            if (ObjectPooler.SharedInstance.GetPooledObject("Reflector") != null)
            {
                GameObject obj = ObjectPooler.SharedInstance.GetPooledObject("Reflector");
                obj.transform.rotation = Quaternion.Euler(0, 0, GetRandomAngle());

                
                obj.transform.position = GetRandomPosition();
                obj.SetActive(true);
            }
        }

        if (Random.value <= 0.01)
        {
            if (ObjectPooler.SharedInstance.GetPooledObject("Enemy") != null)
            {
                GameObject obj = ObjectPooler.SharedInstance.GetPooledObject("Enemy");
                obj.transform.position = GetRandomPosition();
                obj.SetActive(true);
            }
        }
    }

    float GetRandomAngle()
    {
        float output = ( (int)(Random.value * 360) / 45) * 45;
        return output;
    }

    Vector3 GetRandomPosition()
    {
        
        Vector3 output = new Vector3(Random.value * 20 - 10, Random.value * 16 - 8, 0);

        while (true)
        {
            bool ValidPosition = true;
            output = new Vector3(Random.value * 20 - 10, Random.value * 16 - 8, 0);

            if (!DistanceCheck(output, Player.gameObject.transform.position))
            {
                ValidPosition = false;
            }

            for(int i = 0; i < ObjectPooler.SharedInstance.GetTotalPoolSize(); i++)
            {
                if (!DistanceCheck(output, ObjectPooler.SharedInstance.GetPooledObject(i).transform.position))
                {
                    ValidPosition = false;
                }
            }

            if (ValidPosition)
            {
                break;
            }
        }
        return output;
    }

    bool DistanceCheck(Vector3 currentPos, Vector3 targetPos)
    {
        if(Vector3.Distance(currentPos, targetPos) < SpawnRadius)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public void ScorePoints(int score, int multiplier)
    {
        Score += score * multiplier;
        GiveScoreFeedback(score, multiplier);
    }

    public void GiveScoreFeedback(int score, int multiplier)
    {
        GameObject FeedBack = Instantiate(ScoreFeedback, ScoreText.transform.position, Quaternion.identity, ScoreText.transform);
        FeedBack.GetComponent<ScoreFeedBackController>().SetScore(score, multiplier);
    }

    private string GetTimeString()
    {
        string output = "{0:00}:{1:00}";
        return string.Format(output, (int) RunTime / 60, RunTime % 60);
    }

    public void EndGame()
    {
        FinalTimeText.GetComponent<Text>().text = GetTimeString();
        FinalScoreText.GetComponent<Text>().text = Score.ToString();

        StartCoroutine(ShakeScript.Shake(1f, .8f, .8f));
        ScoreText.SetActive(false);
        DeathScreen.GetComponent<Animator>().SetTrigger("EndGame");
        
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
