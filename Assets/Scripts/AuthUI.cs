using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AuthUI : MonoBehaviour
{
    [Header("Поля ввода")]
    [SerializeField] private InputField usernameInput;
    [SerializeField] private InputField passwordInput;

    [Header("Кнопки")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;

    [Header("Статус / Ошибки")]
    [SerializeField] private Text statusText;

    [Header("Настройки сцены")]
    [SerializeField] private string gameSceneName = "example";

    private void Start()
    {
        if (passwordInput != null)
            passwordInput.contentType = InputField.ContentType.Password;

        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);

        SetStatus("");
    }

    private void OnLoginClicked()
    {
        string user = usernameInput.text.Trim();
        string pass = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            SetStatus("Заполните все поля!");
            return;
        }

        SetStatus("Вход...");
        SetButtonsInteractable(false);

        NetworkManager.Instance.Login(user, pass,
            response =>
            {
                SetStatus("Успешный вход!");
                // Переходим на игровой уровень
                SceneManager.LoadScene(gameSceneName);
            },
            error =>
            {
                SetStatus("Ошибка: неверный логин или пароль");
                SetButtonsInteractable(true);
            }
        );
    }

    private void OnRegisterClicked()
    {
        string user = usernameInput.text.Trim();
        string pass = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            SetStatus("Заполните все поля!");
            return;
        }

        SetStatus("Регистрация...");
        SetButtonsInteractable(false);

        NetworkManager.Instance.Register(user, pass,
            () =>
            {
                SetStatus("Регистрация успешна! Выполняется вход...");
                // После успешной регистрации сразу автоматически входим
                NetworkManager.Instance.Login(user, pass,
                    response =>
                    {
                        SceneManager.LoadScene(gameSceneName);
                    },
                    err =>
                    {
                        SetStatus("Зарегистрирован, но не удалось войти");
                        SetButtonsInteractable(true);
                    }
                );
            },
            error =>
            {
                SetStatus("Ошибка: пользователь уже существует");
                SetButtonsInteractable(true);
            }
        );
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    private void SetButtonsInteractable(bool state)
    {
        loginButton.interactable = state;
        registerButton.interactable = state;
    }
}