using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Anathema.Game
{
    public class SwitchLevel : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public GameObject LoadingScreen;
        public Image LoadBarFill;
        // Update is called once per frame


        public void LoadScene(int id)
        {
            StartCoroutine(LoadAsyncScene(id));
        }

        IEnumerator LoadAsyncScene(int id)
        {
            text.text = "LOADING PLEASE WAIT";
            AsyncOperation operation = SceneManager.LoadSceneAsync(id);
            LoadingScreen.SetActive(true);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                LoadBarFill.fillAmount = progress;
                yield return null;
            }
        }
    }
}
