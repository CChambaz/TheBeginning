using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBall : MonoBehaviour
{
    [SerializeField] private float spell_velocity = 10.0f;

    private SpriteRenderer player_renderer;

    private void Start()
    {
        player_renderer = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void Shoot()
    {
        if (player_renderer.flipX)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-spell_velocity, 0), ForceMode2D.Impulse);
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(spell_velocity, 0), ForceMode2D.Impulse);
        }

        gameObject.GetComponent<Animator>().SetBool("is_fired", true);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" || collision.tag == "Limit")
        {
            gameObject.GetComponent<Animator>().SetBool("is_fired", false);
        }
    }
}
