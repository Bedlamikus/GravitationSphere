using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Персонаж с рэгдоллом: при начале движения активируются Rigidbody из списка (isKinematic = false).
/// Вращение задаётся одной костью (драйвер), остальные тянутся за ней через ConfigurableJoint (настраиваются вручную).
/// Управление джойстиком применяет крутящий момент к драйвер-кости.
/// </summary>
public class RagdollCharacter : MonoBehaviour
{
    [Header("Управление")]
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private float torqueForce = 1f;
    [SerializeField] private float maxAngularVelocity = 5f;

    [Tooltip("Минимальный ввод джойстика для перехода в рэгдолл. При первом таком вводе после старта уровня персонаж «сваливается» в рэгдолл.")]
    [SerializeField] private float minInputToActivateRagdoll = 0.01f;

    [Tooltip("Если включено — не ждём GlobalEvents.StartLevel, считаем уровень стартовавшим с первого кадра. Нужно, т.к. StartLevel в проекте вызывается только при нажатии «Заново».")]
    [SerializeField] private bool levelStartedFromFirstFrame = true;

    [Header("Рэгдолл")]
    [Tooltip("Аниматор персонажа. Отключается при переходе в режим рэгдолла.")]
    [SerializeField] private Animator animator;

    [Tooltip("Кость, которую крутит джойстик. На ней должен быть Rigidbody. Масса драйвера должна быть больше массы следующей за ним кости (см. «Массы» ниже).")]
    [SerializeField] private Rigidbody driverBone;

    [Tooltip("Rigidbody частей рэгдолла. При начале движения на всех выставляется isKinematic = false. Настраивать в инспекторе, без поиска в дочерних.")]
    [SerializeField] private Rigidbody[] ragdollRigidbodies;

    [Header("Портал")]
    [Tooltip("Родительский объект, который выставляется в точку портала. Если не задан — используется transform этого объекта.")]
    [SerializeField] private Transform mainParent;
    [Tooltip("Камера, в сторону которой поворачивают персонажа по Y при выходе из портала. Если не задана — используется Camera.main.")]
    [SerializeField] private Transform cameraForPortalRotation;
    [Tooltip("Длительность плавного возврата костей в исходное положение при входе в портал (сек).")]
    [SerializeField] private float bonesReturnDuration = 0.5f;
    [Tooltip("Имя int-параметра в аниматоре для состояния. На следующем кадре после включения аниматора выставляется в 1.")]
    [SerializeField] private string animatorStateParamName = "State";

    [Header("Массы (драйвер и следующая кость)")]
    [Tooltip("Рекомендуемое отношение: масса_драйвера / масса_следующей_кости ≥ это значение. 1.5–2 = следующая кость тянется за драйвером при вращении. Если следующая тяжелее — будет «тянуть» драйвер, контроль хуже. Поле только для подсказки, массы задаются в Rigidbody каждой кости.")]
    [Range(1f, 3f)]
    [SerializeField] private float recommendedDriverToFollowerMassRatio = 1.5f;

    [Header("Пружины (для всех джойнтов сразу)")]
    [Tooltip("ConfigurableJoint, к которым применяются настройки пружин ниже. Меняя значения — применяются ко всем из списка.")]
    [SerializeField] private ConfigurableJoint[] jointsToConfigure;

    [Tooltip("Общий множитель жёсткости. 1 = базовые значения ниже; >1 = жёстче, форма лучше сохраняется; <1 = мягче.")]
    [Range(0.3f, 3f)]
    [SerializeField] private float stiffnessMultiplier = 1f;

    [Tooltip("Жёсткость пружины по позиции (X, Y, Z). Выше = части меньше «растягиваются». Чтобы не терять форму при вращении: 500–2000.")]
    [SerializeField] private float positionSpring = 800f;
    [Tooltip("Демпфер по позиции. Смягчает раскачку; для сохранения формы: 30–60.")]
    [SerializeField] private float positionDamper = 40f;
    [Tooltip("Макс. сила привода по позиции. Чтобы пружина успевала держать форму при быстром вращении — 2000–5000.")]
    [SerializeField] private float positionMaxForce = 3000f;

    [Tooltip("Жёсткость пружины по углу (вращение). Выше = конечности меньше «разъезжаются». Чтобы не терять форму: 500–2000.")]
    [SerializeField] private float angularSpring = 800f;
    [Tooltip("Демпфер по углу. Убирает болтание; для сохранения формы: 30–60.")]
    [SerializeField] private float angularDamper = 40f;
    [Tooltip("Макс. сила привода по углу. Чтобы суставы держали форму при вращении — 2000–5000.")]
    [SerializeField] private float angularMaxForce = 3000f;

    private bool _ragdollActive;
    private bool _levelStarted;
    private bool _inputDisabled;
    private Transform _rootForPortal;
    private Transform[] _bonesToReset;
    private Vector3[] _initialLocalPos;
    private Quaternion[] _initialLocalRot;

    private void Start()
    {
        if (driverBone != null)
            driverBone.maxAngularVelocity = maxAngularVelocity;

        ApplySpringSettingsToAll();

        if (levelStartedFromFirstFrame)
        {
            _levelStarted = true;
            Debug.Log($"[RagdollCharacter] {gameObject.name}: уровень считается стартовавшим с первого кадра, ждём первый ввод джойстика (порог {minInputToActivateRagdoll}).");
        }
        else
        {
            GlobalEvents.StartLevel.AddListener(OnLevelStarted);
            Debug.Log($"[RagdollCharacter] {gameObject.name}: ждём GlobalEvents.StartLevel, затем первый ввод джойстика.");
        }

        if (joystick == null)
            Debug.LogWarning($"[RagdollCharacter] {gameObject.name}: джойстик не назначен!");

        _rootForPortal = mainParent != null ? mainParent : transform;
        CaptureInitialPose();
    }

    private void CaptureInitialPose()
    {
        var bones = new List<Transform>();
        if (driverBone != null)
            bones.Add(driverBone.transform);
        if (ragdollRigidbodies != null)
        {
            foreach (var rb in ragdollRigidbodies)
            {
                if (rb == null) continue;
                if (driverBone != null && rb == driverBone) continue;
                bones.Add(rb.transform);
            }
        }

        _bonesToReset = bones.ToArray();
        _initialLocalPos = new Vector3[_bonesToReset.Length];
        _initialLocalRot = new Quaternion[_bonesToReset.Length];
        Transform root = _rootForPortal;
        for (int i = 0; i < _bonesToReset.Length; i++)
        {
            _initialLocalPos[i] = root.InverseTransformPoint(_bonesToReset[i].position);
            _initialLocalRot[i] = Quaternion.Inverse(root.rotation) * _bonesToReset[i].rotation;
        }
    }

    private void OnDestroy()
    {
        if (!levelStartedFromFirstFrame)
            GlobalEvents.StartLevel.RemoveListener(OnLevelStarted);
    }

    private void OnLevelStarted()
    {
        _levelStarted = true;
        Debug.Log($"[RagdollCharacter] {gameObject.name}: получен StartLevel, ждём первый ввод джойстика (порог {minInputToActivateRagdoll}).");
    }

    /// <summary>
    /// Включает рэгдолл: на всех Rigidbody из списка isKinematic = false.
    /// </summary>
    public void ActivateRagdoll()
    {
        _ragdollActive = true;
        Debug.Log($"[RagdollCharacter] Переход в режим рэгдолл: {gameObject.name}");

        if (animator != null)
        {
            animator.enabled = false;
            Debug.Log($"[RagdollCharacter] Аниматор отключён: {animator.gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"[RagdollCharacter] Аниматор не назначен, отключать нечего.");
        }

        if (ragdollRigidbodies == null) return;

        foreach (var rb in ragdollRigidbodies)
        {
            if (rb == null) continue;
            rb.isKinematic = false;
        }
    }

    /// <summary>
    /// Применяет настройки пружин ко всем джойнтам из списка jointsToConfigure.
    /// </summary>
    public void ApplySpringSettingsToAll()
    {
        if (jointsToConfigure == null) return;

        float m = Mathf.Max(0.01f, stiffnessMultiplier);
        var posDrive = new JointDrive
        {
            positionSpring = positionSpring * m,
            positionDamper = positionDamper * m,
            maximumForce = positionMaxForce * m
        };
        var angX = new JointDrive
        {
            positionSpring = angularSpring * m,
            positionDamper = angularDamper * m,
            maximumForce = angularMaxForce * m
        };
        var angYZ = new JointDrive
        {
            positionSpring = angularSpring * m,
            positionDamper = angularDamper * m,
            maximumForce = angularMaxForce * m
        };

        foreach (var j in jointsToConfigure)
        {
            if (j == null) continue;
            j.xDrive = posDrive;
            j.yDrive = posDrive;
            j.zDrive = posDrive;
            j.angularXDrive = angX;
            j.angularYZDrive = angYZ;
        }
    }

    private void OnValidate()
    {
        ApplySpringSettingsToAll();
    }

    /// <summary>
    /// Выключает рэгдолл: на всех Rigidbody из списка isKinematic = true.
    /// </summary>
    public void DeactivateRagdoll()
    {
        _ragdollActive = false;

        if (animator != null)
            animator.enabled = true;

        if (ragdollRigidbodies == null) return;

        foreach (var rb in ragdollRigidbodies)
        {
            if (rb == null) continue;
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Вызывается при входе в портал: родитель в точку портала, плавный возврат костей в исходную позу, включение аниматора, на следующем кадре State = 1.
    /// </summary>
    public void EnterPortal(Transform portalPoint)
    {
        if (portalPoint == null) return;
        if (_rootForPortal == null) _rootForPortal = mainParent != null ? mainParent : transform;

        _inputDisabled = true;

        GlobalEvents.CharacterInPortal.Invoke();

        StopAllCoroutines();
        StartCoroutine(ReturnBonesAndEnableAnimator(portalPoint));
    }

    private IEnumerator ReturnBonesAndEnableAnimator(Transform portalPoint)
    {
        _ragdollActive = false;

        Transform root = _rootForPortal;

        // Отключаем все Rigidbody из списка: сначала нулевые скорости, затем кинематика
        if (ragdollRigidbodies != null)
        {
            foreach (var rb in ragdollRigidbodies)
            {
                if (rb == null) continue;
                if (!rb.isKinematic)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                rb.isKinematic = true;
            }
        }

        // Отключаем коллайдеры у объектов из списка джойнтов
        if (jointsToConfigure != null)
        {
            foreach (var j in jointsToConfigure)
            {
                if (j == null) continue;
                var colliders = j.GetComponents<Collider>();
                foreach (var col in colliders)
                {
                    if (col != null)
                        col.enabled = false;
                }
            }
        }

        root.position = portalPoint.position;
        root.rotation = portalPoint.rotation;

        if (_bonesToReset == null || _bonesToReset.Length == 0)
        {
            if (animator != null) animator.enabled = true;
            RotateRootTowardCamera(root);
            yield return null;
            if (animator != null && !string.IsNullOrEmpty(animatorStateParamName))
                animator.SetInteger(animatorStateParamName, 1);
            yield break;
        }

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, bonesReturnDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smooth = t * t * (3f - 2f * t); // SmoothStep

            for (int i = 0; i < _bonesToReset.Length; i++)
            {
                var bone = _bonesToReset[i];
                if (bone == null) continue;

                Vector3 targetPos = root.TransformPoint(_initialLocalPos[i]);
                Quaternion targetRot = root.rotation * _initialLocalRot[i];

                bone.position = Vector3.Lerp(bone.position, targetPos, smooth);
                bone.rotation = Quaternion.Slerp(bone.rotation, targetRot, smooth);

                var rb = bone.GetComponent<Rigidbody>();
                if (rb != null && rb.isKinematic)
                {
                    rb.position = bone.position;
                    rb.rotation = bone.rotation;
                }
            }

            yield return null;
        }

        // Финальная подгонка в исходную позу
        for (int i = 0; i < _bonesToReset.Length; i++)
        {
            var bone = _bonesToReset[i];
            if (bone == null) continue;
            Vector3 targetPos = root.TransformPoint(_initialLocalPos[i]);
            Quaternion targetRot = root.rotation * _initialLocalRot[i];
            bone.position = targetPos;
            bone.rotation = targetRot;
            var rb = bone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.position = targetPos;
                rb.rotation = targetRot;
            }
        }

        if (animator != null)
            animator.enabled = true;

        RotateRootTowardCamera(root);

        yield return null;

        if (animator != null && !string.IsNullOrEmpty(animatorStateParamName))
            animator.SetInteger(animatorStateParamName, 1);
    }

    private void RotateRootTowardCamera(Transform root)
    {
        Transform cam = cameraForPortalRotation != null ? cameraForPortalRotation : (Camera.main != null ? Camera.main.transform : null);
        if (cam == null) return;
        Vector3 toCam = cam.position - root.position;
        toCam.y = 0f;
        if (toCam.sqrMagnitude > 0.001f)
            root.rotation = Quaternion.LookRotation(toCam.normalized);
    }

    private void FixedUpdate()
    {
        if (_inputDisabled) return;
        if (joystick == null) return;

        // После старта уровня ждём первый достаточный ввод — тогда включаем рэгдолл
        if (_levelStarted && !_ragdollActive)
        {
            float v = joystick.Vertical;
            float h = joystick.Horizontal;
            if (Mathf.Abs(v) >= minInputToActivateRagdoll || Mathf.Abs(h) >= minInputToActivateRagdoll)
            {
                Debug.Log($"[RagdollCharacter] {gameObject.name}: ввод джойстика v={v:F3} h={h:F3} >= порог {minInputToActivateRagdoll}, переходим в рэгдолл.");
                ActivateRagdoll();
            }
        }

        if (driverBone == null) return;
        if (!_ragdollActive || driverBone.isKinematic) return;

        float verticalInput = joystick.Vertical;
        float horizontalInput = joystick.Horizontal;

        if (Mathf.Abs(verticalInput) > 0.01f)
            driverBone.AddTorque(Vector3.right * torqueForce * verticalInput, ForceMode.Force);

        if (Mathf.Abs(horizontalInput) > 0.01f)
            driverBone.AddTorque(-Vector3.forward * torqueForce * horizontalInput, ForceMode.Force);
    }
}
