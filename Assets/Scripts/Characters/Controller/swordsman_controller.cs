using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordsman_controller : MonoBehaviour
{
    [SerializeField] Collider2D attack_collider;

    private GameObject player;
    private Transform player_transform;
    private Vector3 player_bounds;
    private float attack_range = 2.0f;
    private Rigidbody2D rigid;
    private Swordsman swordsman;
    private Animator anim_controller;
    private float touched_time = 0.0f;

	// Use this for initialization
	void Start ()
    {
        swordsman = new Swordsman();
        rigid = GetComponent<Rigidbody2D>();
        anim_controller = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        player_transform = player.transform;
        player_bounds = player.GetComponent<Renderer>().bounds.size;
        rigid.mass = swordsman.mass;
	}
	
	// Update is called once per frame
	void Update ()
    {
        swordsman.Move(gameObject, rigid, player);

        WillAttack();

        anim_controller.SetFloat("speed_x", Mathf.Abs(rigid.velocity.x));
        anim_controller.SetFloat("is_touched", touched_time);
    }

    private void WillAttack()
    {
        if (!anim_controller.GetBool("is_attacking"))
        {
            if (Mathf.Abs(transform.position.x - player_transform.position.x + player_bounds.x) <= attack_range
                || Mathf.Abs(player_transform.position.x - player_bounds.x - transform.position.x) <= attack_range)
            {
                anim_controller.SetBool("is_attacking", true);
                attack_collider.enabled = true;
            }
        }
    }

    private void Attack()
    {
        attack_collider.enabled = false;
        anim_controller.SetBool("is_attacking", false);
    }

    private void Touched()
    {
        if (swordsman.life <= 0)
        {
            Destroy(gameObject, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerAttack") //&& !anim_controller.GetBool("is_touched"))
        {
            touched_time = 2.0f;
            swordsman.HasBeenTouched(1);
        }
    }
}
