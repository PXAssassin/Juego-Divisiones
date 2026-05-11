using UnityEngine;

public class GeneradorDeValores : MonoBehaviour
{
    [Header("Valores")]
    public int dividendo;
    public int divisor;
    public int cociente;

    public void GenerarValores()
    {
        cociente = Random.Range(1,9);

        divisor = Random.Range(10,100);

        dividendo = divisor * cociente;

        if (dividendo < 100 || dividendo > 780)
        {
            GenerarValores();
            return;
        }

        int operacion = dividendo % divisor;
        Debug.Log($"Division Generada: {dividendo} / {divisor} = cociente{cociente} residuo:{operacion}");

    }

}
