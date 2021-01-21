using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour, IKillable, IDamageable
{
    public int BaseHealth;

    private int Health;
    private SpriteRenderer SR;

    public List<Sprite> Sprites;
    // Start is called before the first frame update
    void Start()
    {
        Health = BaseHealth;
        SR = GetComponent<SpriteRenderer>();
    }
    
    void OnEnable()
    {
        Health = BaseHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(Health <= 0)
        {
            Kill();

        }
    }

    public void Kill()
    {
        this.gameObject.SetActive(false);
        Health = BaseHealth;
        SR.sprite = Sprites[Health];
    }

    public void Damage(int damage)
    {
        Health -= damage;
        if (Health >= 0)
        {
            SR.sprite = Sprites[Health];
        }

    }
}
