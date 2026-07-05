using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Movement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    private float maxForce = 4f;
    private float jumpForce = 5f;

    //GUI needs it
    public static int maxHealth = 100;
    public static int coins = 0;
    public static System.Collections.Generic.List<string> collectedCoins = new System.Collections.Generic.List<string>();
    public static int maxArmor = 50;
    public static int attempts = 0;

    //Animation needs it
    private Rigidbody2D rb;
    private float horizontalInput;

    private bool isGrounded;
    private bool isFacingRight = true;
    //private bool isSpriting = false;



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

        //Условия перезапуска (смерть/падение итд если будет)
        ReloadSceneBecauseDie();
        ReloadSceneBecauseFalling();
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


    private void ReloadSceneBecauseFalling()
    {
        if (transform.position.y < -10)
        {
            attempts++;
            Debug.Log("Try number" + attempts);
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }
    }

    private void ReloadSceneBecauseDie()
    {
        if (maxHealth <= 0)
        {
            attempts++;
            maxHealth = 100;
            Debug.Log("Try number" + attempts);
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

    public static void AddCoin()
    {
        coins++;
    }

    public static int GetCoinsValue()
    {
        return coins;
    }

    public static int GetAttemptsValue()
    {
        return attempts;
    }

    public static void RegisterCollectedCoin(string coinName)
    {
        if (!collectedCoins.Contains(coinName))
        {
            collectedCoins.Add(coinName);
        }
    }

    public static void Damage(int Damage)
    {
        if (maxArmor <= 0)
        {
            maxHealth -= Damage;
        }
        else
        {
            maxArmor -= Damage;
        }
        
    }
    public static void Heal(int Heal)
    {
        if (maxHealth < 100)
        {
            maxHealth += Heal;
        }
        else
        {
            Debug.Log("100 is a max");
        }
        
    }


}
