using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PLayerScript : MonoBehaviour
{
    private float speed = 150f;
    public float MinSpeed = 50f;
    public float MaxSpeed = 200f;

    public float jumpForce = 25f;

    public bool isGrounded;
    private Rigidbody rb;

    //camara
    public float Sensibility = 90f;
    public float LimitX = 45;
    public Transform cam;

    private float rotationX;
    // private float rotationY;
    [Header("Interaction")]
    public float maxInteractionDistance = 160f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if (Time.timeScale > 0)
        {

            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = MaxSpeed;
            }
            else
            {
                speed = MinSpeed;
            }

            transform.Translate(new Vector3(x, 0, y) * Time.deltaTime * speed);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            //camara

            // 1. Rotación Vertical de la Cámara (Arriba/Abajo) - Eje X
            float mouseInputY = Input.GetAxis("Mouse Y") * Sensibility * Time.deltaTime;
            // Acumular la rotación vertical. El signo negativo corrige la dirección natural del mouse.
            rotationX -= mouseInputY;

            // Limitar la rotación vertical (mirar hacia el cielo/suelo)
            rotationX = Mathf.Clamp(rotationX, -LimitX, LimitX);

            // Aplicar la rotación a la cámara (localmente, solo en el Eje X)
            cam.localRotation = Quaternion.Euler(rotationX, 0, 0);

            // 2. Rotación Horizontal del Jugador (Izquierda/Derecha) - Eje Y
            float mouseInputX = Input.GetAxis("Mouse X") * Sensibility * Time.deltaTime;

            // Aplicar la rotación al cuerpo del jugador (transform.rotation)
            // Esto permite que el jugador gire y, por ende, su eje Z (adelante) cambie.
            transform.rotation *= Quaternion.Euler(0, mouseInputX, 0);

            //Pausa
            if (Input.GetKeyDown(KeyCode.Escape))
            {

                UIManager.inst.ShowPauseScreen();

            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
            Transform switchTransform = GameObject.Find("Swtich").transform; // Solo si el nombre es exactamente "Swtich"
            float distance = Vector3.Distance(transform.position, switchTransform.position);
            Debug.Log("Distancia al Switch: " + distance);
        }
    }

    public void Jump()
    {
        if(isGrounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag=="Ground")
        {
            isGrounded = true;
        }
    }
    public void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag == "Ground")     {
            isGrounded = false;
        }
    }
    void Interact()
    {
        if (cam == null) return;
        // Dispara un rayo desde el centro de la cámara
        Ray ray = new Ray(cam.position, cam.forward);
        Debug.DrawRay(cam.position, cam.forward * maxInteractionDistance, Color.red, 0.1f);
        RaycastHit hit;

        // Si el rayo golpea algo dentro de la distancia de interacción:
        if (Physics.Raycast(ray, out hit, maxInteractionDistance))
        {
            // 1. Verificar si el objeto golpeado tiene el script ControlLuz
            ControlLight lightControl = hit.collider.GetComponent<ControlLight>();

            // 2. Si lo tiene, llamar al método para alternar la luz
            if (lightControl != null)
            {
                lightControl.AlternarLuz();
            }
        }
    }
}

