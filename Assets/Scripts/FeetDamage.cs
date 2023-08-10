using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetDamage : MonoBehaviour
{
    [SerializeField] AudioClip killHitSound;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("EnemiesHead"))
        {
            // disable the head hurtbox
            Collider2D coli = other.GetComponent<Collider2D>();
            coli.enabled = false;
            Animator animator = other.GetComponentInParent<Animator>();
            Collider2D[] colliders = other.GetComponentsInParent<Collider2D>();
            Rigidbody2D rigidbody = other.GetComponentInParent<Rigidbody2D>();
            if (animator != null && rigidbody != null)
            {
                // disable character stuff
                foreach (Collider2D collider in colliders)
                {
                    collider.enabled = false;
                }
                rigidbody.simulated = false;
                // play animation
                AudioSource.PlayClipAtPoint(killHitSound, Camera.main.transform.position);
                animator.SetBool("isDying", true);
            }
        }
    }
}
