using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class ControladorVideo : MonoBehaviour
{
    [Header("Componentes de Video")]
    public VideoPlayer videoPlayer;
    public UIDocument uiDocument;

    VisualElement root;
    VisualElement interfazAviso;

    public string nombreEscenaJuego = "EscenaJuego";
    public bool botonEstaPresionado = false;
    public float tiempoPresionado = 0f;
    private const float TIEMPO_REQUERIDO_SALTAR = 5f;
    private bool videoHaTerminado = false;


    private void Awake()
    {
        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
        root = uiDocument.rootVisualElement;
        interfazAviso = root.Q<VisualElement>("Interfaz");
    }

    void OnEnable()
    {
        videoPlayer.loopPointReached += AlLlegarAlFinalDelVideo;
    }

    void OnDisable()
    {
        videoPlayer.loopPointReached -= AlLlegarAlFinalDelVideo;
    }

    void Update()
    {
        if (botonEstaPresionado)
        {
            tiempoPresionado += Time.deltaTime;

            if (tiempoPresionado >= TIEMPO_REQUERIDO_SALTAR)
            {
                tiempoPresionado = 0f;
                botonEstaPresionado = false;
                CargarSiguienteEscena();
            }
        }
    }

    /// <summary>
    /// Función que llamará tu botón físico desde el Arduino
    /// </summary>
    public void RecibirInputBotonFisico(bool estaPresionado)
    {
        if (GameManager.instance.estadoActual == GameManager.EstadoJuego.Video)
        {
            botonEstaPresionado = estaPresionado;

            if (estaPresionado)
            {
                if (videoHaTerminado)
                {
                    Debug.Log("Contando 5 segundos para continuar tras el final...");
                    return;
                }

                if (videoPlayer.isPlaying)
                {
                    videoPlayer.Pause();
                    Debug.Log("Video Pausado.");
                }
                else
                {
                    videoPlayer.Play();
                    Debug.Log("Video Reanudado.");
                }
            }
            else
            {
                tiempoPresionado = 0f;
            }
        }
        
    }

    private void AlLlegarAlFinalDelVideo(VideoPlayer vp)
    {
        videoHaTerminado = true;
        videoPlayer.Pause();

        if (interfazAviso != null)
        {
            if (interfazAviso.ClassListContains("interfaz"))
            {
                interfazAviso.RemoveFromClassList("interfaz");
            }

            interfazAviso.AddToClassList("interfazOn");
        }
    }

    private void CargarSiguienteEscena()
    {
        SceneManager.LoadScene(nombreEscenaJuego);
        GameManager.instance.estadoActual = GameManager.EstadoJuego.Juego;

    }
}

