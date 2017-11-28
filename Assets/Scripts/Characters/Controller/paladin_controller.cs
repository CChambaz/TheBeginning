using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class paladin_controller : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private Transform position_raycast_jump;
    [SerializeField] private float radius_raycast_jump;
    [SerializeField] private LayerMask layer_mask_jump;

    [Header("UI")]
    [SerializeField] private Text text_life;
    [SerializeField] private Text text_health_point;
    [SerializeField] private Text text_mana;

    [Header("Object Info")]
    [SerializeField] private SpriteRenderer hero_renderer;
    [SerializeField] private CircleCollider2D attack_collider;

    [Header("Spell")]
    [SerializeField] private Transform cast_left;
    [SerializeField] private Transform cast_right;
    [SerializeField] private GameObject spell_prefab;

    private Transform spawn_transform;
    private Rigidbody2D rigid;
    private Paladin paladin_player;
    private Animator anim_controller;
    private float touched_time;
    private float touched_cooldown = 0.5f;

    // Use this for initialization
    void Start ()
    {
        paladin_player = new Paladin();
        hero_renderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        anim_controller = GetComponent<Animator>();
        spawn_transform = GameObject.Find("Spawn").transform;
        rigid.mass = paladin_player.mass;
    }
	
	// Update is called once per frame
	void Update ()
    {
        text_life.text = "Life : " + paladin_player.life.ToString();
        text_health_point.text = "HP : " + paladin_player.health_point.ToString();
        text_mana.text = "Mana : " + paladin_player.ammo.ToString();

        float horizontal_input = Input.GetAxis("Horizontal");

        anim_controller.SetFloat("speed_x", Mathf.Abs(horizontal_input));

        if ((Input.GetKey(KeyCode.A) || horizontal_input < 0) && !hero_renderer.flipX)
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x, rigid.velocity.y);
            hero_renderer.flipX = true;
        }

        if ((Input.GetKey(KeyCode.D) || horizontal_input > 0) && hero_renderer.flipX)
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x, rigid.velocity.y);
            hero_renderer.flipX = false;
        }

        Vector2 forceDirection = new Vector2(horizontal_input, 0);
        forceDirection *= paladin_player.force_x;

        rigid.AddForce(forceDirection);

        if (Mathf.Abs(rigid.velocity.magnitude) > paladin_player.max_speed)
        {
            rigid.velocity = rigid.velocity.normalized * paladin_player.max_speed;
        }
        
        bool touch_floor = Physics2D.OverlapCircle(position_raycast_jump.position, radius_raycast_jump, layer_mask_jump);

        if (touch_floor)
        {
            anim_controller.SetBool("is_jumping", false);
        }

        if (Input.GetButton("Jump") && touch_floor)
        {
            if (rigid.velocity.y <= paladin_player.force_y)
            {
                rigid.AddForce(Vector2.up * paladin_player.force_y, ForceMode2D.Impulse);
            }            
            
            anim_controller.SetBool("is_jumping", true);
        }        

        if (!touch_floor)
        {
            anim_controller.SetBool("is_jumping", true);
        }

        if (Input.GetAxis("Fire1") > 0 && !anim_controller.GetBool("is_attacking") && !anim_controller.GetBool("is_spelling"))
        {
            attack_collider.enabled = true;

            anim_controller.SetBool("is_attacking", true);            
        }

        if(Input.GetAxis("Fire2") > 0 && !anim_controller.GetBool("is_spelling") && !anim_controller.GetBool("is_attacking") && paladin_player.ammo > 0)
        {
            anim_controller.SetBool("is_spelling", true);
        }

        if (Time.time - touched_time > touched_cooldown)
        {
            Touched();
        }
    }

    private void Attack()
    {
        anim_controller.SetBool("is_attacking", false);

        attack_collider.enabled = false;
    }

    private void Touched()
    {
        anim_controller.SetBool("is_touched", false);
    }

    private void ChargeSpell()
    {
        if (!anim_controller.GetBool("is_spelling"))
        {
            if (gameObject.GetComponent<SpriteRenderer>().flipX)
            {
                GameObject spell = Instantiate(spell_prefab, cast_left.transform.position, cast_left.rotation);
            }
            else
            {
                GameObject spell = Instantiate(spell_prefab, cast_right.transform.position, cast_right.rotation);
            }

            paladin_player.ammo -= 1;
        }
    }

    private void ShootSpell()
    {        
        anim_controller.SetBool("is_spelling", false);        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Win")
        {
            SceneManager.LoadScene("WinMenu");
        }

        if (collision.tag == "EnemyAttack")
        {
            paladin_player.HasBeenTouched(transform, spawn_transform);

            if (!anim_controller.GetBool("is_attacking"))
            {
                anim_controller.SetBool("is_touched", true);
                touched_time = Time.time;
            }
        }

        if(collision.tag == "HostileEnvironnement")
        {
            paladin_player.HasBeenTouched(transform, spawn_transform);
            anim_controller.SetBool("is_touched", true);
            touched_time = Time.time;
        }

        if(collision.tag == "HPItem")
        {
            paladin_player.health_point += GameManager.gm_instance.item_hp_gained;

            Destroy(collision.gameObject);
        }

        if (collision.tag == "ManaItem")
        {
            paladin_player.ammo += GameManager.gm_instance.item_mana_gained;

            Destroy(collision.gameObject);
        }

        if (collision.tag == "LifeItem")
        {
            paladin_player.life += GameManager.gm_instance.item_life_gained;

            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "EnemyAmo")
        {
            paladin_player.HasBeenTouched(transform, spawn_transform);

            if (!anim_controller.GetBool("is_attacking"))
            {
                anim_controller.SetBool("is_touched", true);
                touched_time = Time.time;
            }
            
            Destroy(collision.collider.gameObject);
        }
    }
}
