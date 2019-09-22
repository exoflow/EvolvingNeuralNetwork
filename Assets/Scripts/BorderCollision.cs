using UnityEngine;

public class BorderCollision : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name != "Bird(Clone)")
        {
            Destroy(collision.gameObject);
        }
    }
}
