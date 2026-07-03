using UnityEngine;
using UnityEngine.SceneManagement;


public class Movement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    public float maxForce = 4f;
    public float jumpForce = 5f;

    private Rigidbody2D rb;
    private float horizontalInput;

    private bool isGrounded;
    private bool isFacingRight = true;
    //private bool isSpriting = false;


    public static int attempts = 0;

    public Animator anim;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");


        anim.SetFloat("moveX", Mathf.Abs(horizontalInput));

        if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }

        else if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            anim.SetTrigger("isJumping");
        }
            
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            maxForce = 7f;
        }

        else
        {
            maxForce = 4f;
        }

        if (maxForce < 7)
        {
            anim.SetBool("isRunning", false);
        }
        else 
        {
            anim.SetBool("isRunning", true);
        }


        isItOnTheLevel();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * maxForce, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Floor")
        {
            Vector2 contactPoint = collision.GetContact(0).point;

            if (contactPoint.y < transform.position.y)
            {
                isGrounded = true;
            }
        }
    }

    private void isItOnTheLevel()
    {
        if (transform.position.y < -10)
        {
            attempts++;
            Debug.Log("Peyzda Try number" + attempts);
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
            
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
