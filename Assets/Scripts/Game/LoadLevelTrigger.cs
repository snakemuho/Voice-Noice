using UnityEngine;
using UnityEngine.SceneManagement;

namespace Anathema.Game
{
    public class LoadLevelTrigger : MonoBehaviour
    {
        // Update is called once per frame
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "GameController")
                SceneManager.LoadScene("Level");
        }


    }
}
