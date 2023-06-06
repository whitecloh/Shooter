
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Button _unpauseButton;
    [SerializeField]
    private Button _startMenuButton;
    [SerializeField]
    private GameObject _results;
    [SerializeField]
    private GameObject _pauseMenu;

    private void Awake()
    {
        _unpauseButton.onClick.AddListener(ClosePauseMenu);
        _startMenuButton.onClick.AddListener(QuitGame);
        _pauseMenu.SetActive(false);
        _results.SetActive(false);
    }

    private void QuitGame()
    {
        Debug.Log("Open Start Menu");
        Time.timeScale = 1f;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadSceneAsync(0);
    }

    private void ClosePauseMenu()
    {
        Debug.Log("Unpaused");
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
