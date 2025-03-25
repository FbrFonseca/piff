using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengerBehaviour : MonoBehaviour, Interactable
{
    public void Interact()
    {
        Debug.Log("You will start a battle!");
        SceneManager.LoadScene("Third");
    }
}
