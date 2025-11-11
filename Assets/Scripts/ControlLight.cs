using UnityEngine;

public class ControlLight : MonoBehaviour
{
    // Variables asignables desde el Inspector
    [Header("Componentes")]
    public Light luzAmarilla; // Arrastra tu componente Light aquí
    public GameObject stickInterruptor; // Arrastra el objeto que debe rotar (el stick)

    [Header("Configuración de Rotación")]
    public float duracionAnimacion = 0.5f; // Duración en segundos para la rotación

    // Variables internas
    private bool estaEncendida = false;
    private Quaternion rotacionApagado;
    private Quaternion rotacionEncendido;
    private Coroutine animacionActual;

    void Start()
    {
        // Al inicio, guardamos la rotación actual como el estado "Apagado"
        rotacionApagado = stickInterruptor.transform.localRotation;

        // Calculamos la rotación de "Encendido" (180 grados sobre el Eje X o Z, ajusta el eje según tu modelo)
        // Usaremos el eje Z para este ejemplo. AJÚSTALO si tu modelo rota en X o Y.
        rotacionEncendido = rotacionApagado * Quaternion.Euler(0, 0, 180f);

        // Aseguramos que la luz esté en el estado inicial
        luzAmarilla.enabled = estaEncendida;
    }

    // Método para ser llamado al interactuar (e.g., al pulsar un botón cerca del Trigger)
    public void AlternarLuz()
    {
     
        estaEncendida = !estaEncendida; // Invertir el estado

        // 1. Controlar la luz
        luzAmarilla.enabled = estaEncendida; // Activar/Desactivar el componente Light

        // Opcional: Cambiar el material de la bombilla (si tienes un material emisivo)
        // Ejemplo: Renderer bombillaRenderer = luzAmarilla.GetComponent<Renderer>();
        // if (bombillaRenderer != null) { bombillaRenderer.material = estaEncendida ? materialEncendido : materialApagado; }

        // 2. Iniciar la animación de rotación del stick
        if (animacionActual != null)
        {
            StopCoroutine(animacionActual);
        }

        Quaternion objetivo = estaEncendida ? rotacionEncendido : rotacionApagado;
        animacionActual = StartCoroutine(RotarStick(objetivo));
    }

    // Corrutina para la animación de rotación suave
    System.Collections.IEnumerator RotarStick(Quaternion objetivo)
    {
        float tiempoTranscurrido = 0;
        Quaternion rotacionInicial = stickInterruptor.transform.localRotation;

        while (tiempoTranscurrido < duracionAnimacion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / duracionAnimacion;
            // Opcional: Usar una curva de aceleración (como SmoothStep) para un movimiento más suave
            t = Mathf.SmoothStep(0.0f, 1.0f, t);

            stickInterruptor.transform.localRotation = Quaternion.Lerp(rotacionInicial, objetivo, t);
            yield return null;
        }

        // Asegurar la rotación final
        stickInterruptor.transform.localRotation = objetivo;
        animacionActual = null;
    }

    // Ejemplo de interacción con el jugador:
    // Puedes usar un Raycast o un OnMouseDown si el interruptor tiene un Collider
    // o un OnTriggerEnter si usas un área de interacción.

    // Ejemplo con Raycast/Clic:
    void OnMouseDown()
    {
        // Asegúrate de que el stickInterruptor tenga un Collider
        AlternarLuz();
    }
}