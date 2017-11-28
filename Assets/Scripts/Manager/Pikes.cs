using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pikes : MonoBehaviour
{
    private float pike_high = 1.42f;
    private bool is_used = false;

    private void TrapActivated()
    {
        gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + pike_high);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !is_used)
        {
            is_used = true;
            TrapActivated();
        }
    }
}
