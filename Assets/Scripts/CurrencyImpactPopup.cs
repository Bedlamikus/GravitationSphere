using System.Collections;
using TMPro;
using UnityEngine;

public class CurrencyImpactPopup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text rewardText;

    [Header("Animation")]
    [SerializeField] private float lifetime = 1.0f;
    [SerializeField] private float floatUpDistance = 0.75f;
    [SerializeField] private AnimationCurve alphaCurve = new AnimationCurve(
        new Keyframe(0f, 1f),
        new Keyframe(1f, 0f)
    );

    [Header("Billboard")]
    [SerializeField] private bool faceCamera = true;

    private Camera _camera;
    private Coroutine _routine;
    private Color _baseColor = Color.white;

    private void Awake()
    {
        if (rewardText == null)
            rewardText = GetComponentInChildren<TMP_Text>(true);

        if (rewardText != null)
            _baseColor = rewardText.color;
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
    }

    private void LateUpdate()
    {
        if (!faceCamera) return;

        if (_camera == null)
            _camera = Camera.main;

        if (_camera == null) return;

        Vector3 dir = _camera.transform.position - transform.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        // Canvas "front" is often flipped, so rotate to face the camera.
        transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0f, 180f, 0f);
    }

    public void Play(Vector3 worldPosition, float reward, Camera cameraOverride = null)
    {
        transform.position = worldPosition;

        if (cameraOverride != null)
            _camera = cameraOverride;

        if (rewardText != null)
        {
            rewardText.text = $"+{Mathf.RoundToInt(reward)}$";
            SetAlpha(1f);
        }

        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(AnimateRoutine());
    }

    private IEnumerator AnimateRoutine()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * floatUpDistance;

        float t = 0f;
        while (t < lifetime)
        {
            t += Time.deltaTime;
            float n = lifetime <= 0.0001f ? 1f : Mathf.Clamp01(t / lifetime);

            transform.position = Vector3.LerpUnclamped(startPos, endPos, n);

            float alpha = alphaCurve != null ? alphaCurve.Evaluate(n) : (1f - n);
            SetAlpha(alpha);

            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void SetAlpha(float a)
    {
        if (rewardText == null) return;

        Color c = _baseColor;
        c.a = Mathf.Clamp01(a);
        rewardText.color = c;
    }
}

