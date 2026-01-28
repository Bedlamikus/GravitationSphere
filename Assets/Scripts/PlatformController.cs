using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private Platform[] platforms;
    [SerializeField] private Sphere sphere;
    [SerializeField] private RagdollCharacter character;

    private int currentTargetIndex = 0;

    private void Start()
    {
        if (character == null)
            character = FindObjectOfType<RagdollCharacter>();

        SetNextTarget();
        GlobalEvents.NextPlatform.AddListener(SetNextTarget);
        GlobalEvents.RestartLevel.AddListener(RestartLevel);
    }

    private void SetNextTarget()
    {
        if (currentTargetIndex < platforms.Length)
        {
            if (currentTargetIndex != 0)
            {
                platforms[currentTargetIndex - 1].Hide();
            }
            
            var nextStart = platforms[currentTargetIndex].GetSphereStartPosition();

            // Переносим сферу на следующую платформу
            if (sphere != null && nextStart != null)
            {
                sphere.MoveToPosition(nextStart.position);
            }

            // Переносим персонажа на стартовую точку новой платформы и выставляем аниматор State = 0
            if (character != null && nextStart != null)
            {
                character.SimpleTeleport(nextStart);
                character.ExitRagdollWithState0();
                // Через секунду после прилёта снова разрешаем управление рэгдоллом
                character.EnableRagdollInputAfterDelay(1f);
            }

            //cameraController.SetTarget(targets[currentTargetIndex].GetTarget());
            currentTargetIndex++;
        }
        else
        {
            // Если следующей платформы нет - показываем все платформы, замораживаем сферу и вызываем конец игры
            foreach (var platform in platforms)
            {
                platform.gameObject.SetActive(true);
            }
            
            // Замораживаем сферу (как будто шар бесконечно падал)
            if (sphere != null)
            {
                sphere.Freeze();
            }
            
            // Сбрасываем индекс для корректной работы при следующем рестарте
            currentTargetIndex = 0;
            
            // Вызываем событие конца игры
            GlobalEvents.GameOver.Invoke();
        }
    }

    private void RestartLevel()
    {
        currentTargetIndex = 0;
        foreach (var platform  in platforms)
        {
            platform.gameObject.SetActive(true);
        }
        SetNextTarget();
    }

    public Platform GetCurrentPlatform()
    {
        // Текущая платформа - это та, на которой мы сейчас находимся
        // currentTargetIndex указывает на следующую платформу, поэтому текущая - это currentTargetIndex - 1
        if (currentTargetIndex > 0 && currentTargetIndex <= platforms.Length)
        {
            return platforms[currentTargetIndex - 1];
        }
        // Если мы еще на первой платформе (currentTargetIndex == 0 после Start)
        if (currentTargetIndex == 0 && platforms.Length > 0)
        {
            return platforms[0];
        }
        return null;
    }
}
