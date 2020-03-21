using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed;
    private Rigidbody2D rb;
    public GameObject bulletEffect;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
        rb.velocity = new Vector2(bulletSpeed * transform.localScale.x, 0);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player1")
        {
            FindObjectOfType<GameManager>().DamageP1(damage);
        }
        if (collision.gameObject.tag == "Player2")
        {
            FindObjectOfType<GameManager>().DamageP2(damage);
        }
        Instantiate(bulletEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
