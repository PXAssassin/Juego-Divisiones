using UnityEngine;
using System.IO.Ports;

public class comunicacionArduino : MonoBehaviour
{
    public SerialPort puerto = new SerialPort("COM5", 9600);
    public UiJuegoScript juego;
    public string ultimoDatoRecibido = "";

    void Start()
    {
        if (juego == null)
        {
            juego = FindFirstObjectByType<UiJuegoScript>();
        }

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
            if (puerto.IsOpen)
            {
                ultimoDatoRecibido = puerto.ReadLine();
                Debug.Log(ultimoDatoRecibido);

                if (int.TryParse(ultimoDatoRecibido, out int switchesActivos) && juego != null)
                {
                    juego.ValidaInputArduino(switchesActivos);
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
