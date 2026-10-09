using UnityEngine;

public class Pickupable : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private ItemData _itemData;

    [Header("Interaction")]
    [SerializeField] private float _interactRadius = 2.2f;

    [Header("Optional")]
    [SerializeField] private bool _destroyOnPickup = true;

    public ItemData ItemData => _itemData;
    public float InteractRadius => _interactRadius;
    public void OnPickedUp()
    {

        gameObject.SetActive(false);

        if (_destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.35f);
        Gizmos.DrawSphere(transform.position, _interactRadius);

        Gizmos.color = new Color(0.2f, 1f, 0.4f, 1f);
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
}
