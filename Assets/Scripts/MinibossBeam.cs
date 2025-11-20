using UnityEngine;

public class MinibossBeam : MonoBehaviour
{
    private void ApplyDamage(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance != null)
        {
            GameManager.Instance.ApplyPlayerDamage(GameManager.Instance.GetMaxHealth());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ApplyDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        ApplyDamage(other);
    }
}

