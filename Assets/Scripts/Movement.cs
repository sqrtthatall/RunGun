using UnityEngine;
using UnityEngine.SceneManagement;
public class Movement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    public float maxForce = 15f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private float horizontalInput;

    private bool isGrounded;
    public static int attempts = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
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

}
