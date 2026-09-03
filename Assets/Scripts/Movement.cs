using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    private float maxForce = 4f;
    private float jumpForce = 5f;

    // GUI needs it
    public static int Health = 100;
    public static int coins = 0;

    public static System.Collections.Generic.List<string> collectedCoins = new System.Collections.Generic.List<string>();
    public static System.Collections.Generic.List<string> collectedBandages = new System.Collections.Generic.List<string>();

    public static int Armor = 50;
    public static int attempts = 0;

    // Animation needs it
    private Rigidbody2D rb;
    private float horizontalInput;

    private bool isGrounded;
    private bool isFacingRight = true;

    public Animator anim;

    public AudioSource coinTakeAudio;
    public AudioSource bandageTakeAudio;

    public static Movement Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    // ВРЕМЕННЫЙ ТЕСТ: автовход игроком tester
        if (NetworkManager.Instance != null && string.IsNullOrEmpty(NetworkManager.Instance.CurrentUsername))
        {
            NetworkManager.Instance.Register("tester", "12345", () => {
                NetworkManager.Instance.Login("tester", "12345", res => {
                    Debug.Log($"Тест сети: Успешный вход! Монеты в базе: {res.coins}, Смерти: {res.deaths}");
                });
            }, err => {
                // Если уже зарегистрирован — сразу входим
                NetworkManager.Instance.Login("tester", "12345", res => {
                    Debug.Log($"Тест сети: Успешный вход! Монеты в базе: {res.coins}, Смерти: {res.deaths}");
                });
            });
        }
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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            maxForce = 7f;
            anim.SetBool("isRunning", true);
        }
        else
        {
            maxForce = 4f;
            anim.SetBool("isRunning", false);
        }

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

            // Отправляем смерть на сервер (0 монет, 1 смерть)
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.SendProgress(0, 1);
            }

            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }
    }

    private void ReloadSceneBecauseDie()
    {
        if (Health <= 0)
        {
            attempts++;
            Health = 100;
            Debug.Log("Try number" + attempts);

            // Отправляем смерть на сервер (0 монет, 1 смерть)
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.SendProgress(0, 1);
            }

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

    public void CoinTakeAudio()
    {
        if (coinTakeAudio != null)
            coinTakeAudio.Play();
    }

    public static void AddCoin()
    {
        Movement.Instance.CoinTakeAudio();
        coins++;

        // Сразу синхронизируем +1 монетку с БД (1 монета, 0 смертей)
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.SendProgress(1, 0);
        }
    }

    public static int GetCoinsValue() => coins;
    public static int GetAttemptsValue() => attempts;

    public static void RegisterCollectedCoin(string coinName)
    {
        if (!collectedCoins.Contains(coinName))
        {
            collectedCoins.Add(coinName);
        }
    }

    public static void RegisterCollectedBandage(string bandageName)
    {
        if (!collectedBandages.Contains(bandageName))
        {
            collectedBandages.Add(bandageName);
        }
    }

    public static void Damage(int damageValue)
    {
        if (Armor <= 0)
        {
            Health -= damageValue;
        }
        else
        {
            Armor -= damageValue;
        }
    }

    public static void Heal(int healValue)
    {
        if (Movement.Instance.bandageTakeAudio != null)
            Movement.Instance.bandageTakeAudio.Play();

        if (Health < 100)
        {
            Health += healValue;
            if (Health > 100) Health = 100;
        }
    }
}
