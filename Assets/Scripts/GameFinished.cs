using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameFinished : MonoBehaviour
{
    [SerializeField] MonsterAI monsterAI;
    [SerializeField] AudioSource monsterAudio, playerAudio;
    [SerializeField] Image lightIMG;
    [SerializeField] AudioClip choir;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GameController")
        {
            FinishGame();
        }
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.G))
    //        FinishGame();
    //}
    public void FinishGame()
    {
        playerAudio.PlayOneShot(choir);
        GameManager.Instance.isAlive = false;
        monsterAI.StopAgent();
        monsterAI.enabled = false;
        StartCoroutine(SoundOff());
        StartCoroutine(AddLight());
        StartCoroutine(ExitGame());
    }

    IEnumerator AddLight()
    {
        while(lightIMG.color.a < 1)
        {
            lightIMG.color = Color.Lerp(lightIMG.color, new Color(1, 0.7216981f, 0.7705173f, 1), Time.fixedDeltaTime);
            yield return null;
        }
    }
    IEnumerator SoundOff()
    {
        while (monsterAudio.volume > 0)
        {
            monsterAudio.volume -= Time.fixedDeltaTime;
            yield return null;
        }
    }

    IEnumerator ExitGame()
    {
        yield return new WaitForSeconds(11);
        Application.Quit();
    }



}
