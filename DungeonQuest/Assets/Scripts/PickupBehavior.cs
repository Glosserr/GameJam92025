using UnityEngine;
public class PickupBehavior : MonoBehaviour
{
    public string type;

    public void IsPickedUp()
    {
        Destroy(gameObject);
    }
}
