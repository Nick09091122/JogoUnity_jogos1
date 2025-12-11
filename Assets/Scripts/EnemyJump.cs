using UnityEngine;

public class EnemyJump : MonoBehaviour
{
  public float JumpForce;

  void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.gameObject.tag == "Enemy") collision.gameObject.SendMessage("Jump", JumpForce);
  }

}
