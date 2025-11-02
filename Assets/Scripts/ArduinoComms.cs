using UnityEngine;
using System.IO.Ports; // Necesario para la comunicación Serial
using System.Globalization;

public class ArduinoComms : MonoBehaviour
{
    // --- Configuración Serial ---
    // ¡CAMBIA ESTO! Debe coincidir con el puerto de tu Arduino (ej: COM3 en Windows, /dev/ttyUSB0 en Linux)
    public string portName = "COM5";
    public int baudRate = 9600; // Debe coincidir con el Serial.begin(9600) de Arduino

    private SerialPort serialPort;
    private string receivedData;

    // Variables públicas para mostrar los datos en otros scripts o en el Inspector de Unity
    [Header("Datos de Lectura")]
    public float currentHumidity = 0f;
    public float currentTemperature = 0f;

    void Start()
    {
        // 1. Inicializar el Puerto Serial
        serialPort = new SerialPort(portName, baudRate);

        try
        {
            serialPort.Open(); // Abre la conexión
            serialPort.ReadTimeout = 100; // Tiempo de espera para la lectura
            Debug.Log("Serial: Conexión abierta en " + portName);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Serial Error: No se pudo abrir el puerto " + portName + ". " + ex.Message);
        }
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                // 2. Leer los datos
                receivedData = serialPort.ReadLine();

                if (!string.IsNullOrEmpty(receivedData))
                {
                    // 3. Procesar los datos
                    ParseData(receivedData);
                }
            }
            catch (System.TimeoutException)
            {
                // Es normal si Arduino no ha enviado una línea en el ReadTimeout
            }
            catch (System.Exception ex)
            {
                // Otros errores (ej. puerto desconectado)
                Debug.LogError("Serial Read Error: " + ex.Message);
            }
        }
    }

    void ParseData(string data)
    {
        string[] dataArray = data.Split(',');

        if (dataArray.Length == 2)
        {
            // --- ¡IMPORTANTE! Forzar el uso del punto '.' ---
            CultureInfo culture = CultureInfo.InvariantCulture;

            // Intentar convertir la Humedad
            if (float.TryParse(dataArray[0], NumberStyles.Any, culture, out float humidity))
            {
                currentHumidity = humidity;
            }
            else
            {
                Debug.LogError("Fallo al convertir Humedad: " + dataArray[0]); // Añadir para depuración
            }

            // Intentar convertir la Temperatura
            if (float.TryParse(dataArray[1], NumberStyles.Any, culture, out float temperature))
            {
                currentTemperature = temperature;
            }
            else
            {
                Debug.LogError("Fallo al convertir Temperatura: " + dataArray[1]); // Añadir para depuración
            }
        }
    }

    void OnApplicationQuit()
    {
        // Cierra el puerto cuando la aplicación se detiene
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial: Puerto cerrado.");
        }
    }
}