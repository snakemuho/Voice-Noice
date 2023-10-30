using UnityEngine;

namespace Anathema.Game
{
    public class UISwitcher : MonoBehaviour
    {
        public GameObject calibOn, calibOff;
        private static UISwitcher _instance;
        public static UISwitcher Instance
        {
            get { if (_instance is null) Debug.Log("UI manager null"); return _instance; }
        }
        private void Awake()
        {
            if (_instance == null)
            {
                DontDestroyOnLoad(this);
                _instance = this;
            }
            else Destroy(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!GameManager.Instance.GamePaused)
                    GameManager.Instance.PauseGame();
                else GameManager.Instance.ResumeGame();
                SwitchUI();
            }
        }
        public void SwitchUI()
        {
            calibOn.SetActive(!calibOn.activeSelf);
            calibOff.SetActive(!calibOff.activeSelf);
        }
    }
}
