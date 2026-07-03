using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class AttemtsUILogic : MonoBehaviour
{
    public TMP_Text attemptsText;

    void Update()
    {
        attemptsText.text = Movement.GetAttemptsValue().ToString();
    }

}
