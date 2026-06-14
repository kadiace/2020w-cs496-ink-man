using Platformer.Core;
using Platformer.Model;
using Platformer.Mechanics;
using UnityEngine;

public class damaged : MonoBehaviour
{
    bool bossClearTriggered;
    PlatformerModel model = Simulation.GetModel<PlatformerModel>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            Simulation.Schedule<Platformer.Gameplay.PlayerDeath>(0);
            return;
        }

        Debug.Log("collided");
        if (collision.gameObject.CompareTag("line"))
        {
            Destroy(collision.gameObject);
            Animator anim = GetComponentInParent<Animator>();
            anim.SetTrigger("isDamaged");
            var nextLife = anim.GetInteger("life") - 1;
            anim.SetInteger("life", nextLife);
            Debug.Log(anim.GetInteger("life"));

            if (!bossClearTriggered && nextLife <= 0)
            {
                bossClearTriggered = true;

                var player = model.player;
                if (player != null)
                {
                    player.controlEnabled = false;
                    player.velocity = Vector2.zero;
                    if (player.animator != null)
                        player.animator.SetTrigger("victory");

                    var playerBody = player.GetComponent<Rigidbody2D>();
                    if (playerBody != null)
                    {
                        playerBody.linearVelocity = Vector2.zero;
                        playerBody.gravityScale = 0;
                    }
                }

                Simulation.Schedule<Platformer.Gameplay.ShowBossClearOverlay>(1.2f);
            }

        }
        //else if ()
        //{

        //}
    }

}
