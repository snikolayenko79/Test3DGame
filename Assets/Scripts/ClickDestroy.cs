using System.Collections;
using UnityEngine;

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
        mainCamera = Camera.main; // Находим главную камеру
    }

    private void Update()
    {
        // Если уже исчезает, клики не обрабатываем
        if (isDisappearing) return;

        // Проверяем нажатие левой кнопки мыши (работает и в старой, и в новой Input System)
        if (Input.GetMouseButtonDown(0))
        {
            CheckClick();
        }
    }

    private void CheckClick()
    {
        // Создаем луч из камеры через точку курсора на экране
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Проверяем, попал ли луч в коллайдер этого конкретного объекта
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