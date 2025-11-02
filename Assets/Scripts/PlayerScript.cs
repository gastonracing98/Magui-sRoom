using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PLayerScript : MonoBehaviour
{
    private float speed = 125f;
    public float MinSpeed = 50f;
    public float MaxSpeed = 200f;

    public float jumpForce = 125f;

    public bool isGrounded;
    private Rigidbody rb;

    //camara
    public float Sensibility = 100f;
    public float LimitX = 45;
    public Transform cam;

    private float rotationX;
    private float rotationY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
      float x= Input.GetAxis("Horizontal");
      float y = Input.GetAxis("Vertical");

        if(Input.GetKey(KeyCode.LeftShift))
        {
            speed = MaxSpeed;
        }
        else
        {
            speed = MinSpeed;
        }

        transform.Translate(new Vector3(x, 0, y)*Time.deltaTime*speed);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        //camara

        rotationX = -Input.GetAxis("Mouse Y") * Sensibility ;
        rotationX = Mathf.Clamp(rotationX, -LimitX, LimitX);
        cam.localRotation = Quaternion.Euler(-rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * Sensibility, 0);

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
}
