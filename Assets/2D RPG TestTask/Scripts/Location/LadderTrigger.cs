using UnityEngine;

public class LadderTrigger : MonoBehaviour
{
    [SerializeField] private bool loadLevel;

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool isPlayer = collision.CompareTag("Player");

        if (isPlayer)
        {
            SceneLoadManager.Instance.LoadLevel(loadLevel);
        }
    }
}