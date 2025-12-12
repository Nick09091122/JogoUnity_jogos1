using UnityEngine;

public class MinimalFinishTrigger : MonoBehaviour
{
    private bool showMessage = false;
    private float messageTimer = 3f;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            showMessage = true;
            messageTimer = 3f;
            Debug.Log("Bom trabalho!");
        }
    }
    
    void Update()
    {
        if (showMessage)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0)
            {
                showMessage = false;
            }
        }
    }
    
    void OnGUI()
    {
        if (showMessage)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 40;
            style.normal.textColor = Color.green;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            
            GUI.Label(new Rect(0, Screen.height / 2 - 50, Screen.width, 100), "Bom trabalho!", style);
        }
    }
}