using UnityEngine;

[CreateAssetMenu(fileName = "PlayerVariables", menuName = "Scriptable Objects/PlayerVariables")]
public class PlayerVariables : ScriptableObject
{
    [SerializeField]
    private float deadzone = 0.1f;

    [SerializeField]
    private float moveSpeed = 1000f;

    [SerializeField]
    private float lookTowardsRotationModifier = 250f;

    [SerializeField]
    float hideSoundIndicatorDelay = 0.5f;

    [SerializeField]
    float runningSoundWaveRadius = 3f;

    [SerializeField]
    float sneakingRadius = 4f;

    [SerializeField]
    float sneakingSpeedMultiplier = 0.5f;

    [SerializeField]
    float interactionRadius = 2f;

    // Public read-only accessors for other scripts
    public float MoveSpeed => moveSpeed;
    public float Deadzone => deadzone;
    public float LookTowardsRotationModifier => lookTowardsRotationModifier;
    public float HideSoundIndicatorDelay => hideSoundIndicatorDelay;
    public float RunningSoundWaveRadius => runningSoundWaveRadius;
    public float SneakingRadius => sneakingRadius;
    public float SneakingSpeedMultiplier => sneakingSpeedMultiplier;
    public float InteractionRadius => interactionRadius;
}
