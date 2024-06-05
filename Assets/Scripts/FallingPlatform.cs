using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelay = 1f;
    [SerializeField] private float destroyDelay = 2f;
    
    [SerializeField] private Rigidbody2D rb;
    private Vector2 startPosition;
    private IEnumerator coroutine;

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //coroutine = Fall();
            StartCoroutine(Fall());
        }
        
    }

    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        rb.bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(destroyDelay);
        gameObject.SetActive(false);
        setPosition();
    }

    public void setPosition()
    {
        StopCoroutine("Fall");

        rb.bodyType = RigidbodyType2D.Kinematic;

        transform.position = startPosition;
        gameObject.SetActive(true);

    }
}
