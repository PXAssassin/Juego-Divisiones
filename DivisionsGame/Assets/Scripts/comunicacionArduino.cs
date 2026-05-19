using UnityEngine;
using System.IO.Ports;

public class comunicacionArduino : MonoBehaviour
{
    public SerialPort puerto = new SerialPort("COM5", 9600);
    public string ultimoDatoRecibido = "";

    void Start()
    {
        
        puerto.ReadTimeout = 30;

        try
        {
            if (!puerto.IsOpen)
            {
                puerto.Open();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"No se pudo abrir el puerto serial: {ex.Message}");
        }
    }


    void Update()
    {
        try
        {
            if (puerto.IsOpen && puerto.BytesToRead > 0)
            {
                ultimoDatoRecibido = puerto.ReadLine().Trim();
                Debug.Log(ultimoDatoRecibido);

                if (int.TryParse(ultimoDatoRecibido, out int switchesActivos))
                {
                    RedirigirInputPorEstado(switchesActivos); 
                }
            }
        }
        catch (System.TimeoutException)
        {
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Error leyendo desde Arduino: {ex.Message}");
        }

    }

    private void RedirigirInputPorEstado(int switchesActivos)
    {
        // 0. Si estamos en la escena del MENU (¡Añadido para poder arrancar!)
        if (GameManager.instance.estadoActual == GameManager.EstadoJuego.Menu)
        {
            // Como tu Arduino manda datos solo cuando se presiona el botón, 
            // cualquier dato recibido aquí significa que quieren iniciar el juego.
            Debug.Log("Botón presionado en el Menú. Iniciando juego...");
            GameManager.instance.Iniciar();
        }
        // 1. Si estamos en la escena del VIDEO
        else if (GameManager.instance.estadoActual == GameManager.EstadoJuego.Video)
        {
            ControladorVideo scriptVideo = FindFirstObjectByType<ControladorVideo>();
            if (scriptVideo != null)
            {
                scriptVideo.RecibirInputBotonFisico(true);
            }
        }
        // 2. Si estamos en la escena del JUEGO MATEMÁTICO
        else if (GameManager.instance.estadoActual == GameManager.EstadoJuego.Juego)
        {
            UiJuegoScript scriptJuego = FindFirstObjectByType<UiJuegoScript>();
            if (scriptJuego != null)
            {
                scriptJuego.ValidaInputArduino(switchesActivos);
            }
        }
    }

    public void EnviarRespuesta(int respuesta)
    {
        if (respuesta != 0 && respuesta != 1)
        {
            Debug.LogWarning("La respuesta para Arduino debe ser 0 o 1.");
            return;
        }

        try
        {
            if (puerto.IsOpen)
            {
                puerto.WriteLine(respuesta.ToString());
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Error enviando respuesta a Arduino: {ex.Message}");
        }
    }

    public void EnviarRespuesta(bool esCorrecta)
    {
        EnviarRespuesta(esCorrecta ? 1 : 0);
    }

    void OnDestroy()
    {
        if (puerto != null && puerto.IsOpen)
        {
            puerto.Close();
        }
    }
}
