using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class UiJuegoScript : MonoBehaviour
{
    public GeneradorDeValores valores;
    private UIDocument menuDocument;
    public Sonidos sonidos;

    public AudioSource altavoz;
    VisualElement root;

    public List<VisualElement> operacionesRealizadas = new List<VisualElement>();

    Label dividendo;
    Label divisor;
    Label resultadoCociente;
    Label publicarResultados;
    Button jugar;
    IntegerField inputNumerico;

    void Awake()
    {
        menuDocument = GetComponent<UIDocument>();
        root = menuDocument.rootVisualElement;
        ConfigurarAsignaciones();
        AsignarVisualizadorOperacionesR();
    }

    void Start()
    {
        GenerarValores();
    }

    void Update()
    {
        
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
        publicarResultados = root.Q<Label>("txt-result");
    }
    

    private void GenerarValores()
    {
        valores.GenerarValores();
        resultadoCociente.text = "?";
        divisor.text = valores.divisor.ToString();
        dividendo.text = valores.dividendo.ToString();
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

        resultadoCociente.text = valorRegistrado.ToString();

        if (valorGenerado == valorRegistrado)
        {
            resultadoCociente.style.color = Color.green;
            altavoz.clip = sonidos.audioClips[0];
            altavoz.Play();
        }
        else
        {
            resultadoCociente .style.color = Color.red;
            altavoz.clip = sonidos.audioClips[1];
            altavoz.Play();
        }
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
        if (valorRegistrado != 0 && valorRegistrado < 9)
        {
            StartCoroutine(DarRespuestaCorrutina(valorRegistrado));
        }
    }





}
