using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleGate : MonoBehaviour
{
    [SerializeField] private GameObject gate_open;
    [SerializeField] private GameObject gate_closed;
    [SerializeField] private Transform player_transform;

    private float open_range = 5;
    private bool is_open = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!is_open && Mathf.Abs(transform.position.x - player_transform.position.x) < open_range)
        {
            gate_closed.SetActive(false);
            gate_open.SetActive(true);

            is_open = true;
        }
        else if(is_open && (transform.position.x - player_transform.position.x > open_range || player_transform.position.x > transform.position.x + open_range))
        {
            gate_closed.SetActive(true);
            gate_open.SetActive(false);

            is_open = false;
        }
    }
}
