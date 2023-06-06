using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    // The EnemyNavigation attached to this GameObject
    [SerializeField]
    private EnemyNavigation navigation;

    // Object that will receive the "Attack" and "StopAttack" message
    // For example, this could be a weapon
    [SerializeField]
    private GameObject attackMessageReceiver;

    // The range from the target at which to start attacking
    // Only used if useStoppingDistance is false
    public float attackRange = 10f;

    // Whether to use the navigation stopping range as attack range
    public bool useStoppingDistance = true;
    
    private bool attacking = false;

    void Awake()
    {
        // If navigation is not set, try to get it from the GameObject
        if (navigation == null)
        {
            navigation = GetComponent<EnemyNavigation>();
        }
    }

    void FixedUpdate()
    {
        // If navigation is not set, return
        if (navigation == null)
        {
            return;
        }
        
        var remainingDistance = navigation.remainingDistance;
        var range = useStoppingDistance ? navigation.stoppingDistance : attackRange;

        // If the object is within attack range, send the "Attack" message
        if (remainingDistance <= range && !attacking)
        {
            attacking = true;
            if (attackMessageReceiver != null)
            {
                attackMessageReceiver.SendMessage("Attack");
            }
        }
        // If the object is outside of attack range, send the "StopAttack" message
        else if (remainingDistance > range && attacking)
        {
            attacking = false;
            if (attackMessageReceiver != null)
            {
                attackMessageReceiver.SendMessage("StopAttack");
            }
        }
    }
}
