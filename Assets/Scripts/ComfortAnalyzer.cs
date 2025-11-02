using TMPro;
using UnityEngine;
using UnityEngine.UI; // Necesario para manipular componentes de Texto

public class ComfortAnalyzer : MonoBehaviour
{
    [Header("Referencias")]
    // Referencia al script que lee los datos seriales
    public ArduinoComms arduinoComms;

    // Referencia a un componente de texto en tu interfaz (¡Debes configurarlo en el Inspector!)
    public TextMeshProUGUI comfortMessageText;
    public TextMeshProUGUI tempDisplay;
    public TextMeshProUGUI humDisplay;

    [Header("Rangos de Confort (Según la OMS y ASHRAE)")]
    // Rango de temperatura óptimo para la mayoría de las personas
    private const float TEMP_MIN_CONFORT = 20.0f; // °C
    private const float TEMP_MAX_CONFORT = 26.0f; // °C

    // Rango de humedad relativa óptima
    private const float HUM_MIN_CONFORT = 40.0f;  // %
    private const float HUM_MAX_CONFORT = 60.0f;  // %

    void Start()
    {
        // Validación: Asegúrate de que el script ArduinoComms esté referenciado
        if (arduinoComms == null)
        {
            Debug.LogError("ComfortAnalyzer: ¡Falta la referencia a ArduinoComms! Asigna el script en el Inspector.");
            // Intentar encontrarlo automáticamente en el mismo GameObject
            arduinoComms = GetComponent<ArduinoComms>();
        }
    }

    void Update()
    {
        if (arduinoComms != null)
        {
            // Obtener las lecturas del script de comunicación
            float temp = arduinoComms.currentTemperature;
            float hum = arduinoComms.currentHumidity;

            // 1. Mostrar los datos crudos en la UI
            UpdateDisplay(temp, hum);

            // 2. Generar el mensaje de confort
            string message = AnalyzeComfort(temp, hum);

            // 3. Actualizar el componente de texto
            if (comfortMessageText != null)
            {
                comfortMessageText.text = message;
            }
        }
    }

    // Función para actualizar los componentes de texto con los valores actuales
    void UpdateDisplay(float temp, float hum)
    {
        if (tempDisplay != null)
        {
            tempDisplay.text = "Temperatura: " + temp.ToString("F1") + " °C"; // Muestra 1 decimal
        }
        if (humDisplay != null)
        {
            humDisplay.text = "Humedad: " + hum.ToString("F1") + " %"; // Muestra 1 decimal
        }
    }

    // Función principal de análisis
    string AnalyzeComfort(float temp, float hum)
    {
        bool tempComoda = (temp >= TEMP_MIN_CONFORT && temp <= TEMP_MAX_CONFORT);
        bool humComoda = (hum >= HUM_MIN_CONFORT && hum <= HUM_MAX_CONFORT);
        string tempStatus = "";

        // Análisis de Temperatura
        if (temp < TEMP_MIN_CONFORT)
        {
            tempStatus = "frío";
        }
        else if (temp > TEMP_MAX_CONFORT)
        {
            tempStatus = "caliente";
        }
        else
        {
            tempStatus = "ideal";
        }

        // --- Determinación del Mensaje Final ---

        if (tempComoda && humComoda)
        {
            // Caso perfecto
            return "¡CONFORT ÓPTIMO! La temperatura y la humedad son ideales. ✨";
        }
        else if (!tempComoda && humComoda)
        {
            // Temperatura no cómoda, pero humedad bien
            return $"El ambiente está demasiado {tempStatus}. Se requiere climatización.";
        }
        else if (tempComoda && !humComoda)
        {
            // Temperatura cómoda, pero humedad no
            if (hum > HUM_MAX_CONFORT)
            {
                return "Humedad MUY ALTA. Puede sentirse bochornoso o pegajoso. 🥵";
            }
            else // hum < HUM_MIN_CONFORT
            {
                return "Humedad MUY BAJA. El ambiente está seco, puede afectar la respiración. 🌬️";
            }
        }
        else
        {
            // Ambos fuera de rango
            return $"ALERTA: El ambiente está {tempStatus} y la humedad es inadecuada. ⚠️";
        }
    }
}