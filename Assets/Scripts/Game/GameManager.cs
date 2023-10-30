using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Anathema.Game
{
    public class GameManager : MonoBehaviour
    {
        public bool isAlive = true;
        public bool GamePaused { get; private set; }

        private static GameManager _instance;
        public static GameManager Instance
        {
            get { if (_instance is null) Debug.Log("game manager null"); return _instance; }
        }
        private void Awake()
        {
            _instance = this;
        }
        public void PauseGame()
        {
            GamePaused = true;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void ResumeGame()
        {
            GamePaused = false;
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ReloadLevel()
        {
            isAlive = false;
            StartCoroutine(StartReload());
        }

        IEnumerator StartReload()
        {
            Debug.Log(isAlive);
            yield return new WaitForSeconds(2);
            isAlive = true;
            Debug.Log(isAlive);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


    }
}
