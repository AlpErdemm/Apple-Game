using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeButton : MonoBehaviour
{
    public GameMode gameMode; 
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Controller")
        {
            GameModeManager.Instance.gameModeUI.SetActive(false);
            switch (gameMode)
            {
                case GameMode.Measurement:
                    Debug.Log("Measurement Mode Started");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    break;
                case GameMode.WrongBasket:
                    GameModeManager.Instance.StartWrongBasket();
                    break;
                case GameMode.DropOnly:
                    GameModeManager.Instance.StartDropOnly();
                    break;
                case GameMode.Unreachable:
                    GameModeManager.Instance.StartUnreachable();
                    break;
                case GameMode.Static:
                    GameModeManager.Instance.StartStatic();
                    break;
                case GameMode.PerceptionEvaluation:
                    GameModeManager.Instance.StartPerceptionEvaluation();
                    break;
                case GameMode.None:
                    GameModeManager.Instance.EndModeByButton();
                    GameModeManager.Instance.gameModeUI.SetActive(true);
                    break;
            }
        }
    }
}
