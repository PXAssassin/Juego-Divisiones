using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class UiJuegoScript : MonoBehaviour
{
    public GeneradorDeValores valores;
    private UIDocument menuDocument;
    public Sonidos sonidos;
    public int puntaje = 0;
    public AudioSource altavoz;
    VisualElement root;

    public List<VisualElement> operacionesRealizadas = new List<VisualElement>();

    Label dividendo;
    Label divisor;
    Label resultadoCociente;
    Label score;

    IntegerField inputNumerico;

    void Awake()
    {
        menuDocument = GetComponent<UIDocument>();
        if (valores == null) valores = GetComponent<GeneradorDeValores>();
        if (sonidos == null) sonidos = GetComponent<Sonidos>();
        if (altavoz == null) altavoz = GetComponent<AudioSource>();

        root = menuDocument.rootVisualElement;
        ConfigurarAsignaciones();
        AsignarVisualizadorOperacionesR();
    }

    void Start()
    {
        GenerarValores();
    }

    void AsignarVisualizadorOperacionesR()
    {
        for (int i = 1; i <=4 ; i++)
        {
            VisualElement sOperacionRealizada = root.Q<VisualElement>($"contenedorOperacion{i}");
            if (sOperacionRealizada != null)
            {
                operacionesRealizadas.Add(sOperacionRealizada);
            }
        }
    }
    void ConfigurarAsignaciones()
    {
        dividendo = root.Q<Label>("txt-dividendo");
        divisor = root.Q<Label>("txt-divisor");
        resultadoCociente = root.Q<Label>("txt-resultado");
        score = root.Q<Label>("txt-score");
    }
    

    private void GenerarValores()
    {
        valores.GenerarValores();
        resultadoCociente.text = "?";
        divisor.text = valores.divisor.ToString();
        dividendo.text = valores.dividendo.ToString();
        score.text = puntaje.ToString();
    }
    

   
    public string[] ValorCorrecto(int valorRegistrado)
    {
        bool esCorrecto = valorRegistrado == valores.cociente;
        string var = esCorrecto ? " ✔ " : " X ";
        return new string[] {$"{valores.dividendo} ÷ {valores.divisor} = {valores.cociente}",var};
    }

    public void OrdenarOperaciones(int valorRegistrado)
    {
        VisualElement primerElemento = operacionesRealizadas[0];
        Label hijo1 = primerElemento[0] as Label;
        Label hijo2 = primerElemento[1] as Label;

        for (int i = operacionesRealizadas.Count - 1; i > 0; i--)
        {
            VisualElement destino = operacionesRealizadas[i];
            VisualElement origen = operacionesRealizadas[i-1];

            (destino[0] as Label).text = (origen[0] as Label).text;
            (destino[1] as Label).text = (origen[1] as Label).text;
            if ((origen[1] as Label).text == " ✔ ")
            {
                (destino[1] as Label).style.color = Color.green;
            }
            else
            {
                (destino[1] as Label).style.color = Color.red;
            }

        }
        string[] valores = ValorCorrecto(valorRegistrado);
        hijo1.text = valores[0];
        hijo2 .text = valores[1];

        if (valores[1] == " ✔ ")
        {
            hijo2.style.color = Color.green;
        }
        else
        {
            hijo2.style.color = Color.red;
        }
    }


    public void DarRespuesta(int valorRegistrado)
    {
        int valorGenerado = valores.cociente;
        bool esCorrecto = valorGenerado == valorRegistrado;

        resultadoCociente.text = valorRegistrado.ToString();

        if (esCorrecto)
        {
            puntaje += 30;
            resultadoCociente.style.color = Color.green;
            score.text = puntaje.ToString();
            ReproducirSonidoRespuesta(0);
        }
        else
        {
            puntaje -= 30;
            resultadoCociente .style.color = Color.red;
            score.text = puntaje.ToString();
            ReproducirSonidoRespuesta(1);
        }

        if (GameManager.instance != null && GameManager.instance.comunicacion != null)
        {
            GameManager.instance.comunicacion.EnviarRespuesta(esCorrecto);
        }
    }

    private void ReproducirSonidoRespuesta(int indiceClip)
    {
        if (altavoz == null || sonidos == null || sonidos.audioClips == null || sonidos.audioClips.Count == 0)
        {
            Debug.LogWarning("No hay AudioSource o clips de sonido configurados para la respuesta.");
            return;
        }

        if (indiceClip >= sonidos.audioClips.Count)
        {
            Debug.LogWarning($"Falta configurar el clip de sonido en la posicion {indiceClip}.");
            return;
        }

        AudioClip clip = sonidos.audioClips[indiceClip];
        if (clip == null)
        {
            Debug.LogWarning($"El clip de sonido en la posicion {indiceClip} esta vacio.");
            return;
        }

        altavoz.clip = clip;
        altavoz.Play();
    }


    public IEnumerator DarRespuestaCorrutina(int valorRegistrado)
    {
        DarRespuesta(valorRegistrado);
        yield return new WaitForSeconds(1f);
        OrdenarOperaciones(valorRegistrado);
        GenerarValores();

    }

    public void ValidaInputArduino(int valorRegistrado)
    {
        if (GameManager.instance.estadoActual == GameManager.EstadoJuego.Juego)
        {
            if (valorRegistrado != 0 && valorRegistrado < 9)
            {
                StartCoroutine(DarRespuestaCorrutina(valorRegistrado));
            }
        }
        
    }
}
