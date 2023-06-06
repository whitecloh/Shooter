using UnityEngine;

public class OpenPauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _menu;

    void Update()
    {
        if (_menu.activeSelf == false) OpenPauseMenue();
    }

    public void OpenPauseMenue()
    {
        var key = Input.GetKeyDown(KeyCode.Escape);

        if (key)
        {
            Debug.Log("Pause");
            _menu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
        }
    }
}
