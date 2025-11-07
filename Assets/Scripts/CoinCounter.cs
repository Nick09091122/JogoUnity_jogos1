using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("foi entrado por: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Moeda colhida");
            Destroy(gameObject);
        }
    }


}