using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private Platform[] platforms;
    [SerializeField] private Sphere sphere;

    private int currentTargetIndex = 0;

    private void Start()
    {
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
            
            // Переносим сферу на следующую платформу
            if (sphere != null && platforms[currentTargetIndex].GetSphereStartPosition() != null)
            {
                sphere.MoveToPosition(platforms[currentTargetIndex].GetSphereStartPosition().position);
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
