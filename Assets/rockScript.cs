using UnityEngine;
using Platformer.Core;
using Platformer.Gameplay;
using Platformer.Mechanics;

public class rockScript : MonoBehaviour
{
    PolygonCollider2D polygonCollider;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];


    // Start is called before the first frame update
    void Start()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();


    }

    // Update is called once per frame

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            Simulation.Schedule<PlayerDeath>(0);
            Destroy(this.gameObject);
            return;
        }

        if (!collision.gameObject.CompareTag("boss") && !collision.gameObject.CompareTag("rock"))
        {
            if (collision.gameObject.CompareTag("line"))
            {
                Destroy(collision.gameObject);
            }
            Destroy(this.gameObject);
        }

    }

}
