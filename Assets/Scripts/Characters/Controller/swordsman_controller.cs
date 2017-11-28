using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordsman_controller : MonoBehaviour
{
    [SerializeField] Collider2D attack_collider;
    [SerializeField] private Transform left_check;
    [SerializeField] private Transform right_check;
    [SerializeField] private LayerMask layer_check;
    [SerializeField] private float radius_check = 0.2f;
    [SerializeField] private AudioClip sound_touched;

    private GameObject player;
    private Transform player_transform;
    private Vector3 player_bounds;
    private float attack_range = 2.0f;
    private Rigidbody2D rigid;
    private Swordsman swordsman;
    private Animator anim_controller;
    private float attack_time;
    private float touched_time;
    private float attack_cooldown = 2.0f;
    private float touched_cooldown = 2.0f;
    private float view_sight = 10.0f;

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
        bool is_left_ok = Physics2D.OverlapCircle(left_check.position, radius_check, layer_check);
        bool is_right_ok = Physics2D.OverlapCircle(right_check.position, radius_check, layer_check);

        if (!anim_controller.GetBool("is_touched") && !anim_controller.GetBool("is_attacking"))
        {
            if(Mathf.Abs(player_transform.position.x - transform.position.x) <= view_sight)
            {
                swordsman.Move(gameObject, rigid, player, 0, is_left_ok, is_right_ok);
            }            
        }

        WillAttack();

        anim_controller.SetFloat("speed_x", Mathf.Abs(rigid.velocity.x));

        if(Time.time - touched_time > touched_cooldown)
        {
            anim_controller.SetBool("is_touched", false);
        }

        if (swordsman.life <= 0)
        {
            Destroy(gameObject, 1);
        }
    }

    private void WillAttack()
    {
        if (!anim_controller.GetBool("is_attacking") && Time.time - attack_time > attack_cooldown && !anim_controller.GetBool("is_touched"))
        {
            if (Mathf.Abs(transform.position.x - player_transform.position.x + player_bounds.x) <= attack_range
                || Mathf.Abs(player_transform.position.x - player_bounds.x - transform.position.x) <= attack_range)
            {
                anim_controller.SetBool("is_attacking", true);
                attack_collider.enabled = true;
                attack_time = Time.time;
                rigid.velocity = new Vector2(0, 0);
            }
        }
    }

    private void Attack()
    {
        attack_collider.enabled = false;
        anim_controller.SetBool("is_attacking", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerAttack" && !anim_controller.GetBool("is_touched"))
        {
            Attack();
            anim_controller.SetBool("is_touched", true);
            touched_time = Time.time;
            SoundManager.sm_instance.PlaySounds(sound_touched);
            swordsman.HasBeenTouched(GameManager.gm_instance.cqb_damage);
            rigid.velocity = new Vector2(0, 0);
        }

        if (collision.tag == "ThunderBall")
        {
            swordsman.HasBeenTouched(GameManager.gm_instance.thunder_ball_damage);
            Attack();
            anim_controller.SetBool("is_touched", true);
            SoundManager.sm_instance.PlaySounds(sound_touched);
            touched_time = Time.time;
        }
    }
}
