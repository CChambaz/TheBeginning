using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class archer_controller : MonoBehaviour
{
    [Header("Some cool stuff")]
    [SerializeField] private Transform left_check;
    [SerializeField] private Transform right_check;
    [SerializeField] private LayerMask layer_check;
    [SerializeField] private float radius_check = 0.2f;
    [SerializeField] private BoxCollider2D attack_collider;

    [Header("Fire")]
    [SerializeField] private GameObject arrow_prefab;
    [SerializeField] private Transform arrow_transform;
    [SerializeField] private float arrow_velocity = 15;
    [SerializeField] private float time_to_fire = 5;
    [SerializeField] private float last_time_fire = 0;

    [Header("Audio")]
    [SerializeField] private AudioClip sound_touched;

    private GameManager game_manager;
    private AudioSource audio_source;
    private GameObject player;
    private Rigidbody2D rigid;
    private Archer archer;
    private Animator anim_controller;
    private Transform player_transform;
    private float view_sight = 20;
    private float shoot_range = 15;
    private float attack_range = 2;
    private float attack_time;
    private float attack_cooldown = 2.0f;
    private Vector3 rotate_around = new Vector3(0, 180, 0);
    private bool rotate_used = false;
    private Vector3 archer_bounds;
    private Vector3 player_bounds;
    private float rnd_aim_angle;
    private float rnd_aim_min = 0;
    private float rnd_aim_max = 45;    
    private float touched_time;    
    private float touched_cooldown = 1.0f;
    private float audio_delay = 0.2f;

    // Use this for initialization
    void Start()
    {
        archer = new Archer();
        rigid = GetComponent<Rigidbody2D>();
        anim_controller = GetComponent<Animator>();
        audio_source = GetComponent<AudioSource>();
        archer_bounds = GetComponent<Renderer>().bounds.size;
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        player_transform = GameObject.FindGameObjectWithTag("Player").transform;
        player_bounds = GameObject.FindGameObjectWithTag("Player").GetComponent<Renderer>().bounds.size;
        rigid.mass = archer.mass;
    }

    // Update is called once per frame
    void Update()
    {
        anim_controller.SetFloat("speed_x", Mathf.Abs(rigid.velocity.x));

        if (anim_controller.GetBool("is_touched") && Time.time - touched_time > touched_cooldown)
        {
            anim_controller.SetBool("is_touched", false);
        }

        if(!anim_controller.GetBool("is_touched") && !anim_controller.GetBool("is_attacking") && !anim_controller.GetBool("is_shooting"))
        {
            Rotate();

            bool is_left_ok = Physics2D.OverlapCircle(left_check.position, radius_check, layer_check);
            bool is_right_ok = Physics2D.OverlapCircle(right_check.position, radius_check, layer_check);           

            WillShoot();

            WillAttack();
        }

        if (archer.life <= 0)
        {
            Destroy(gameObject, 1);
        }
        
    }

    private void Shoot()
    {
        if (!rotate_used)
        { 
            arrow_velocity = -arrow_velocity;
            arrow_transform.Rotate(rotate_around);
            rotate_used = true;
        }

        GetRndAngle();
        GameObject arrow;

        if(player_transform.position.x < gameObject.transform.position.x)
        {
            arrow = Instantiate(arrow_prefab, arrow_transform.position, new Quaternion(0, 0, arrow_transform.rotation.z - rnd_aim_angle, 0));
        }
        else
        {
            arrow = Instantiate(arrow_prefab, arrow_transform.position, new Quaternion(0, 0, arrow_transform.rotation.z + rnd_aim_angle, 0));
        }

        arrow.GetComponent<Rigidbody2D>().AddForce(new Vector2(arrow_velocity, 0), ForceMode2D.Impulse);

        last_time_fire = Time.time;

        anim_controller.SetBool("is_shooting", false);
        
        Destroy(arrow, 5);
    }

    private void Rotate()
    {
        if (player_transform.position.x > transform.position.x && GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            arrow_transform.position = new Vector2(transform.position.x + archer_bounds.x, arrow_transform.position.y);
            rotate_used = false;
        }
        else if (player_transform.position.x < transform.position.x && !GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            arrow_transform.position = new Vector2(transform.position.x - archer_bounds.x, arrow_transform.position.y);
            rotate_used = false;
        }
    }

    private void WillShoot()
    {
        if (Time.time - last_time_fire > time_to_fire && !anim_controller.GetBool("is_shooting"))
        {
            if ((Mathf.Abs(transform.position.x - player_transform.position.x) <= shoot_range || Mathf.Abs(player_transform.position.x - transform.position.x) <= shoot_range)
                && (Mathf.Abs(transform.position.x - player_transform.position.x) > attack_range || Mathf.Abs(player_transform.position.x - transform.position.x) > attack_range))
            {
                anim_controller.SetBool("is_shooting", true);
                audio_source.PlayDelayed(audio_delay);
            }
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
            }
        }
    }

    private void Attack()
    {
        attack_collider.enabled = false;
        anim_controller.SetBool("is_attacking", false);
        attack_time = Time.time;
    }

    private void GetRndAngle()
    {
        rnd_aim_angle = Random.Range(rnd_aim_min, rnd_aim_max);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "PlayerAttack" && !anim_controller.GetBool("is_touched"))
        {
            archer.HasBeenTouched(GameManager.gm_instance.cqb_damage);
            Attack();
            anim_controller.SetBool("is_shooting", false);
            anim_controller.SetBool("is_touched", true);
            SoundManager.sm_instance.PlaySounds(sound_touched);
            touched_time = Time.time;
        }

        if (collision.tag == "ThunderBall")
        {
            archer.HasBeenTouched(GameManager.gm_instance.thunder_ball_damage);
            Attack();
            anim_controller.SetBool("is_shooting", false);
            anim_controller.SetBool("is_touched", true);
            SoundManager.sm_instance.PlaySounds(sound_touched);
            touched_time = Time.time;
        }
    }
}
