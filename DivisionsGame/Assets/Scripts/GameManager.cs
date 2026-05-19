using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public comunicacionArduino comunicacion;
    public enum EstadoJuego { Menu,Video,Juego}
    public EstadoJuego estadoActual;
    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        estadoActual = EstadoJuego.Menu;
    }

    public void Iniciar()
    {
        SceneManager.LoadScene("EscenaVideo");
        estadoActual = EstadoJuego.Video;
    }
}
