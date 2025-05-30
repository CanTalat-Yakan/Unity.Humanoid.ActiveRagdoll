using UnityEngine;


public class HumanoidSetUp : MonoBehaviour
{
    [Tooltip("Static animator hips.")]
    public Transform masterRoot;
    [Tooltip("Ragdoll hips.")]
    public Transform slaveRoot;
    [Tooltip("Ragdoll looses strength when colliding with other objects except for objects with layers contained in this mask.")]
    public LayerMask dontLooseStrengthLayerMask;


    public SlaveController slaveController;
    public AnimationFollowing animFollow;
}
