using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CurrencyRewardOnImpact : MonoBehaviour
{
    [Header("Reward (interpolated by impact strength)")]
    [SerializeField] private int minReward = 1;
    [SerializeField] private int maxReward = 10;

    [Header("Impact thresholds")]
    [Tooltip("If impact is below this value, reward is not granted.")]
    [SerializeField] private float minImpactForReward = 3f;

    [Tooltip("At/above this impact value, reward reaches Max Reward.")]
    [SerializeField] private float maxImpactForMaxReward = 20f;

    [Header("Anti-spam (optional)")]
    [SerializeField] private float minTimeBetweenRewards = 0.05f;

    private Rigidbody _rb;
    private float _lastRewardTime = -999f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (minTimeBetweenRewards > 0f && Time.time - _lastRewardTime < minTimeBetweenRewards)
            return;

        float impact = GetImpactStrength(collision);
        if (impact < minImpactForReward)
            return;

        float t = Mathf.InverseLerp(minImpactForReward, maxImpactForMaxReward, impact);
        t = Mathf.Clamp01(t);

        float rewardFloat = Mathf.Lerp(minReward, maxReward, t);
        int reward = Mathf.RoundToInt(rewardFloat);
        if (reward <= 0)
            return;

        Currency.AddCurrency(reward);

        Vector3 hitPoint = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
        GlobalEvents.CurrencyImpactReward.Invoke(hitPoint, reward);

        _lastRewardTime = Time.time;
    }

    private float GetImpactStrength(Collision collision)
    {
        // Prefer impulse (physics-derived) when available.
        float impulse = collision.impulse.magnitude;
        if (impulse > 0.0001f)
            return impulse;

        // Fallback: relative velocity scaled by mass (rough).
        return collision.relativeVelocity.magnitude * (_rb != null ? _rb.mass : 1f);
    }
}

