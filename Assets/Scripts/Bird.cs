//@author ExoFlow
//https://github.com/exoflow
//https://www.youtube.com/channel/UC019wlpKUC9Cae_IocyVRIA

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bird : MonoBehaviour
{
    // Bird and wing animation
    bool alive;
    public Sprite[] birdFrames;
    bool isAnimation;

    // Distances to the nearest obstacles
    float distUp;
    float distUpRight;
    float distRight;
    float distRightDown;
    float distDown;

    // Neural network
    public NeuralNetwork brain;
	public float fitness;
    bool initilized;

    void Start()
    {
        alive = true;
    }

    void FixedUpdate()
    {
        if (alive)
        {
            fitness += 0.01f;
            SimulateWind();

            if (initilized)
            {
                CalculateDistances();
                UseNeuralNetwork();
            }
        }

    }

    #region neural network

    public void SetBrain(NeuralNetwork network)
    {
        brain = network;
        initilized = true;
        fitness = 0f;
    }

    public void UseNeuralNetwork()
    {
        // Initializes the input array
        float[] inputs = new float[5];

        // Sets inputs to the distances which the bird 'sees'
        inputs[0] = distUp;
        inputs[1] = distUpRight;
        inputs[2] = distRight;
        inputs[3] = distRightDown;
        inputs[4] = distDown;

        // Feeds inputs through the neural network 
        float[] output = brain.FireNeurons(inputs);

        // Uses output[0] for the jump decision
        if (output[0] > 0)
            Jump();

        // Uses output[1] for the rotatation decision
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,0,-output[1]*90), 0.1f);  
    }

    void CalculateDistances()
    {
        // Defines directions relative to the bird
        Vector2 vectorUp = transform.up * 800;
        Vector2 vectorUpRight = (transform.up + transform.right * 2) * 500;
        Vector2 vectorRight = transform.right * 1000;
        Vector2 vectorRightDown = (-transform.up + transform.right * 2) * 500;
        Vector2 vectorDown = (-transform.up) * 800;

        // Sends rays to the defined directions 
        RaycastHit2D up = Physics2D.Raycast(transform.position, vectorUp);
        RaycastHit2D upRight = Physics2D.Raycast(transform.position, vectorUpRight);
        RaycastHit2D right = Physics2D.Raycast(transform.position, vectorRight);
        RaycastHit2D rightDown = Physics2D.Raycast(transform.position, vectorRightDown);
        RaycastHit2D down = Physics2D.Raycast(transform.position, vectorDown);

        // Calculates the distance between the bird and the closest hit point of the rays
        // Normalizes distances from 0 to 1
        // Sets the distance to 1 if the RaycastHit2D is null
        distUp = up ? (up.distance / 785) : 1;
        distUpRight = upRight ? (upRight.distance / 1000) : 1;
        distRight = right ? (right.distance / 1000) : 1;
        distRightDown = rightDown ? (rightDown.distance / 1000) : 1;
        distDown = down ? (down.distance / 785) : 1;

        // Draws line in editor mode, not necessary for the game
        Debug.DrawRay(transform.position, vectorUp);
        Debug.DrawRay(transform.position, vectorUpRight);
        Debug.DrawRay(transform.position, vectorRight);
        Debug.DrawRay(transform.position, vectorRightDown);
        Debug.DrawRay(transform.position, vectorDown);
    }

    #endregion


    #region jump

    public void Jump()
    {
        // Animates the wings
        if (!isAnimation)
            StartCoroutine(WingAnimation());

        // Adds an impulse force upwards relative to the rotation of the bird
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * 500, ForceMode2D.Impulse);
    }

    public IEnumerator WingAnimation()
    {
        isAnimation = true;
        for (int i = 1; i < birdFrames.Length; i++)
        {
            GetComponent<Image>().overrideSprite = birdFrames[i];
            yield return new WaitForSeconds(0.09f);
        }
        GetComponent<Image>().overrideSprite = birdFrames[0];
        isAnimation = false;
    }

    public void SimulateWind()
    {
        // Adds a force to the left which gets intensified with the rotation of the bird
        GetComponent<Rigidbody2D>().AddForce(Vector2.left * Mathf.Abs(transform.rotation.z) * 10, ForceMode2D.Impulse);
    }

    #endregion


    #region collision

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Bird(Clone)")
        {
            alive = false;
            FallDown();
            GameObject.Find("Canvas").GetComponent<Manager>().BirdCrashed();
        }
    }

    void FallDown()
    {
        GetComponent<CircleCollider2D>().enabled = false;
        StopAllCoroutines();
        GetComponent<Image>().overrideSprite = birdFrames[0];
        GetComponent<Rigidbody2D>().gravityScale = 200;
        GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.left * 500, ForceMode2D.Impulse);
    }

    #endregion


}



