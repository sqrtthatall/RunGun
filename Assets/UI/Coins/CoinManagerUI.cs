using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    public TMP_Text scoreText;

    void Update()
    {
        scoreText.text = Movement.GetCoinsValue().ToString();
    }
}

