using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class MusicaBG : MonoBehaviour
{
    [SerializeField] private EventReference musicaMenu;
    [SerializeField] private EventReference musicaEscapeRoom;

    private FMOD.Studio.EventInstance musicaAtual;
    private bool musicaCriada;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += AoCarregarCena;
        TocarMusica(SceneManager.GetActiveScene().name);
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode _)
    {
        TocarMusica(cena.name);
    }

    private void TocarMusica(string nomeCena)
    {
        EventReference novaMusica;

        if (nomeCena == "Menu")
        {
            novaMusica = musicaMenu;
        }
        else if (nomeCena == "EscapeRoom")
        {
            novaMusica = musicaEscapeRoom;
        }
        else
        {
            return;
        }

        if (musicaCriada)
        {
            musicaAtual.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicaAtual.release();
        }

        musicaAtual = RuntimeManager.CreateInstance(novaMusica);
        musicaAtual.start();
        musicaCriada = true;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= AoCarregarCena;

        if (musicaCriada)
        {
            musicaAtual.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicaAtual.release();
        }
    }
}
