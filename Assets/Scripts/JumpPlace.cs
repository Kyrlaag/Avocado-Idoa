using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlace : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject obj;

    [SerializeField] private float force = 5f;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            //playerigit = GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(obj.transform.position.x, obj.transform.position.y * force);
            Debug.Log("trigger");
        }



    }

    




}
