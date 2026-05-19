using System;
using System.IO.Ports;
using UnityEngine;

public class comunicacionArduino : MonoBehaviour
{
    public SerialPort puerto = new SerialPort("COM4", 9600);
    public string ultimoDatoRecibido = "";

    private bool botonPresionadoAnterior = false;

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
            while (puerto.IsOpen && puerto.BytesToRead > 0)
            {
                ultimoDatoRecibido = puerto.ReadLine().Trim();
                Debug.Log(ultimoDatoRecibido);

                if (TryParseEstadoArduino(ultimoDatoRecibido, out int switchesActivos, out bool botonPresionado))
                {
                    RedirigirInputPorEstado(switchesActivos, botonPresionado, false);
                    botonPresionadoAnterior = botonPresionado;
                }
                else if (int.TryParse(ultimoDatoRecibido, out switchesActivos))
                {
                    // Compatibilidad con el formato anterior: una linea numerica significaba boton presionado.
                    RedirigirInputPorEstado(switchesActivos, true, true);
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

    private bool TryParseEstadoArduino(string dato, out int switchesActivos, out bool botonPresionado)
    {
        switchesActivos = 0;
        botonPresionado = false;

        if (string.IsNullOrWhiteSpace(dato))
        {
            return false;
        }

        string[] partes = dato.Trim().Split(new[] { ';', ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
        bool encontroSwitches = false;
        bool encontroBoton = false;

        if (partes.Length == 2 && int.TryParse(partes[0].Trim(), out int switchesSinClave) && TryParseBoolSerial(partes[1].Trim(), out bool botonSinClave))
        {
            switchesActivos = switchesSinClave;
            botonPresionado = botonSinClave;
            return SwitchesValidos(switchesActivos);
        }

        foreach (string parte in partes)
        {
            string[] claveValor = parte.Split(new[] { '=', ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (claveValor.Length != 2)
            {
                continue;
            }

            string clave = claveValor[0].Trim().ToUpperInvariant();
            string valor = claveValor[1].Trim();

            if ((clave == "S" || clave == "SW" || clave == "SWITCHES") && int.TryParse(valor, out int switchesConClave))
            {
                switchesActivos = switchesConClave;
                encontroSwitches = true;
            }
            else if ((clave == "B" || clave == "BTN" || clave == "BOTON" || clave == "BUTTON") && TryParseBoolSerial(valor, out bool botonConClave))
            {
                botonPresionado = botonConClave;
                encontroBoton = true;
            }
        }

        return encontroSwitches && encontroBoton && SwitchesValidos(switchesActivos);
    }

    private bool TryParseBoolSerial(string valor, out bool resultado)
    {
        string normalizado = valor.Trim().ToUpperInvariant();

        if (normalizado == "1" || normalizado == "TRUE" || normalizado == "HIGH" || normalizado == "ON")
        {
            resultado = true;
            return true;
        }

        if (normalizado == "0" || normalizado == "FALSE" || normalizado == "LOW" || normalizado == "OFF")
        {
            resultado = false;
            return true;
        }

        resultado = false;
        return false;
    }

    private bool SwitchesValidos(int switchesActivos)
    {
        return switchesActivos >= 0 && switchesActivos <= 8;
    }

    private void RedirigirInputPorEstado(int switchesActivos, bool botonPresionado, bool esEventoDeBoton)
    {
        bool botonAcabaDePresionarse = esEventoDeBoton || (botonPresionado && !botonPresionadoAnterior);

        if (GameManager.instance.estadoActual == GameManager.EstadoJuego.Menu)
        {
            if (botonAcabaDePresionarse)
            {
                Debug.Log("Boton presionado en el menu. Iniciando juego...");
                GameManager.instance.Iniciar();
            }
        }
        else if (GameManager.instance.estadoActual == GameManager.EstadoJuego.Video)
        {
            ControladorVideo scriptVideo = FindFirstObjectByType<ControladorVideo>();
            if (scriptVideo != null && (esEventoDeBoton || botonPresionado != botonPresionadoAnterior))
            {
                scriptVideo.RecibirInputBotonFisico(botonPresionado);
            }
        }
        else if (GameManager.instance.estadoActual == GameManager.EstadoJuego.Juego)
        {
            UiJuegoScript scriptJuego = FindFirstObjectByType<UiJuegoScript>();
            if (scriptJuego != null && botonAcabaDePresionarse)
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
