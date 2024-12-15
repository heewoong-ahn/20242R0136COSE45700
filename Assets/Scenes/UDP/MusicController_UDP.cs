using UnityEngine;

public class MusicController_UDP : MonoBehaviour
{
    public GameData gameData;
    public AudioSource backgroundMusic; // 배경음악 AudioSource
    public AudioSource winner; // 승리 음악 AudioSource
    public AudioSource startBell;
    public AudioSource win;
    public AudioSource ready;
    public AudioSource three;
    public AudioSource two;
    public AudioSource one;

    private bool backgroundMusicPlayed = false; // 배경음악 재생 여부 플래그
    private bool startBellPlayed = false; // 스타트 벨 재생 여부 플래그
    private bool readyPlayed = false;
    private bool threePlayed = false;
    private bool twoPlayed = false;
    private bool onePlayed = false;

    void Start()
    {
       /* // 배경음악 시작
        backgroundMusic.Play();
        Invoke("playStartBell", 4f);
        Invoke("playReady", 0.2f);
        Invoke("playThree", 1.1f);
        Invoke("playTwo", 2.1f);
        Invoke("playOne", 3.1f);*/
    }

    private void Update()
    {
        if (!backgroundMusicPlayed && gameData.gameTime <= 94.5f)
        {
            backgroundMusic.Play();
            backgroundMusicPlayed = true; // 한 번만 실행되도록 설정
        }
        if (!readyPlayed && gameData.gameTime <= 94f)
        {
            ready.Play();
            readyPlayed = true;
        }
        if (!threePlayed && gameData.gameTime <= 93.1f)
        {
            three.Play();
            threePlayed = true;
        }
        if (!twoPlayed && gameData.gameTime <= 92.1f)
        {
            two.Play();
            twoPlayed = true;
        }
        if (!onePlayed && gameData.gameTime <= 91.1f)
        {
            one.Play();
            onePlayed = true;
        }
        if (!startBellPlayed && gameData.gameTime <= 90.2f)
        {
            startBell.Play();
            startBellPlayed = true;
        }
    }

    public void PlayWinnerMusic()
    {
        // 배경음악 정지
        backgroundMusic.Stop();

        // 승리 음악 재생
        win.Play();
        winner.Play();
       
    }

    void playStartBell()
    {
        startBell.Play();
    }

    void playReady()
    {
        ready.Play();
    }
    void playThree()
    {
        three.Play();
    }
    void playTwo()
    {
        two.Play();
    }
    void playOne()
    {
        one.Play();
    }
}