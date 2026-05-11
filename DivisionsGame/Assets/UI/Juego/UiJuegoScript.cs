using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class UiJuegoScript : MonoBehaviour
{
    public GeneradorDeValores valores;
    private UIDocument menuDocument;
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
        Valores();
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
        jugar = root.Q<Button>("Jugar");
        jugar.RegisterCallback<PointerDownEvent>(ValidarValorInput, TrickleDown.TrickleDown);
        inputNumerico = root.Q<IntegerField>("InputCociente");
    }

    private void Valores()
    {
        valores.GenerarValores();
        resultadoCociente.text = "?";
        divisor.text = valores.divisor.ToString();
        dividendo.text = valores.dividendo.ToString();
    }
    

    private void ValidarValorInput(PointerDownEvent evt)
    {
        int valorRegistrado = inputNumerico.value;
        if (valorRegistrado != 0)
        {
            jugar.style.backgroundColor = Color.green;
            Valores();
            OrdenarOperaciones();
        }
        else
        {
            jugar.style.backgroundColor = Color.red;
        }
    }
    public string[] ValorCorrecto()
    {
        int valorRegistrado = inputNumerico.value;
        bool esCorrecto = valorRegistrado == valores.cociente;
        string var = esCorrecto ? " ✔ " : " X ";
        return new string[] {$"{valores.dividendo} ÷ {valores.divisor} = {valores.cociente}",var};
    }

    public void OrdenarOperaciones()
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
        string[] valores = ValorCorrecto();
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

    public void ValidaInputArduino()
    {
        // Logica aqui.
        // Se deberia poner aqui la logica de las entradas del arduino
        // Y se debe invocar el metodo ValidarInput y quitar el parametro que recibe
        // O usar los metodos de los cuales hace uso
        // en caso de quitar el parametro comentar las siguientes lineas de codigo: 
        /// jugar = root.Q<Button>("Jugar");
        /// jugar.RegisterCallback<PointerDownEvent>(ValidarValorInput, TrickleDown.TrickleDown);
    }





}
