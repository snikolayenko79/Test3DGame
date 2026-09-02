using System.Collections;
using UnityEngine;
// Обязательно добавляем пространство имен новой Input System
using UnityEngine.InputSystem; 

public class ClickDestroy : MonoBehaviour
{
    [Header("Настройки эффекта")]
    [SerializeField] private float fadeDuration = 1.0f; // Время растворения в секундах

    private Renderer meshRenderer;
    private AudioSource audioSource;
    private Collider objCollider;
    private bool isDisappearing = false;
    private Camera mainCamera;

    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        objCollider = GetComponent<Collider>();
        mainCamera = Camera.main; // Ищет камеру с тегом MainCamera
    }

    private void Update()
    {
        if (isDisappearing) return;

        // Проверяем существование указателя (мыши или тача)
        if (Pointer.current == null) return;

        // Была ли нажата кнопка/сделан тач в этом кадре
        if (Pointer.current.press.wasPressedThisFrame)
        {
            CheckClick();
        }
    }

    private void CheckClick()
    {
        // Получаем позицию курсора через новую Input System
        Vector2 mousePosition = Pointer.current.position.ReadValue();
        
        // Строим луч из камеры
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        // Проверяем попадание по коллайдеру
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == objCollider)
            {
                StartCoroutine(FadeAndDestroy());
            }
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        isDisappearing = true;

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        if (objCollider != null)
        {
            objCollider.enabled = false;
        }

        if (meshRenderer != null)
        {
            Material mat = meshRenderer.material;
            Color originalColor = mat.color;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                mat.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }

        if (audioSource != null && audioSource.clip != null)
        {
            while (audioSource.isPlaying)
            {
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}