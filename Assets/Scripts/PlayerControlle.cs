using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControlle : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private enum State {Idle,Running,Jumping,Falling,Hurt}
    private State state = State.Idle;
    private Collider2D coll;
    [SerializeField]private LayerMask Ground;
    [SerializeField]private float speed = 5f;
    [SerializeField]private float jumpHeight = 10f;
    [SerializeField]private int Diamonds = 0;
    [SerializeField]private Text diaText;
    [SerializeField]private float hurtforce = 10f;

    private void Start(){
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }
    
    private void Update(){ 

        if(state != State.Hurt){
            Movement();
        }
        VelocityState();
        anim.SetInteger("State",(int)state);
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "Collectable"){
            Destroy(collision.gameObject);
            Diamonds++;
            diaText.text = Diamonds.ToString();
        }
    }

    private void Movement(){
        float hdirection = Input.GetAxis("Horizontal");
            if(hdirection < 0)
            {
                rb.velocity = new Vector2(-speed,rb.velocity.y);
                transform.localScale = new Vector2(-1,1);
                
            } 
            else if(hdirection > 0){
                rb.velocity = new Vector2(speed,rb.velocity.y);
                transform.localScale = new Vector2(1,1);
                
            }
            if(Input.GetButtonDown("Jump") && coll.IsTouchingLayers(Ground))
            {
                rb.velocity = new Vector2(rb.velocity.x,jumpHeight);
                state = State.Jumping;

            }
    }

    private void OnCollisionEnter2D(Collision2D other){
        if (other.gameObject.tag == "Enemy"){
            if(state == State.Falling){
                Destroy(other.gameObject);
            }
            else{
                state = State.Hurt;
                if(other.gameObject.transform.position.x > transform.position.x){
                    rb.velocity = new Vector2(-hurtforce,rb.velocity.y);
                } else {
                    rb.velocity = new Vector2(hurtforce,rb.velocity.y);
                }
            }
        }
    }

    private void VelocityState(){
        if(state == State.Jumping){
            if(rb.velocity.y <  .1f){
                state = State.Falling;
            }
        }
        else if(state == State.Falling){
            if(coll.IsTouchingLayers(Ground)){
                state = State.Idle;
            }
        }
        else if(state == State.Hurt){
            if(Mathf.Abs(rb.velocity.x) < 0.1f){
                state = State.Idle;
            }
        }

        else if(Mathf.Abs(rb.velocity.x) > 2f){
            //Moving
            state = State.Running;    
        }
        else{
            state = State.Idle;
        }
    }
}