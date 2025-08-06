using UnityEngine;
using UnityEngine.SceneManagement;

public class Changescenes : MonoBehaviour
{
    // purely for buttons
    public void LevelOneScene()
    {
        SceneManager.LoadScene("CheckPoinrDialogue");
    }

    public void LevelTwoScene()
    {
        SceneManager.LoadScene("B-DIALOGUE");
    }

    public void LevelThreeScene()
    {
        SceneManager.LoadScene("AD-DIALOGUE");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
