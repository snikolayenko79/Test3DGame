using System.Collections;
using UnityEngine;

public class ClickDestroy : MonoBehaviour
{
    [Header("Настройки эффектов")]
    [SerializeField] private ParticleSystem explosionParticles; // Сюда перетащим префаб партиклов
    [SerializeField] private float fadeDuration = 1.0f;          // Время растворения

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
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isDisappearing) return;

        if (Input.GetMouseButtonDown(0))
        {
            CheckClick();
        }
    }

    private void CheckClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

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

        // 1. Спавним партиклы взрыва в точке нахождения куба
        if (explosionParticles != null)
        {
            // Создаем систему частиц на сцене
            ParticleSystem spawnedParticles = InstantiatingParticles();
            spawnedParticles.Play();
            
            // Настраиваем автоматическое удаление объекта партиклов, когда они закончатся
            Destroy(spawnedParticles.gameObject, spawnedParticles.main.duration + spawnedParticles.main.startLifetime.constantMax);
        }

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

Destroy(gameObject);
yield break;

        // Отключаем коллайдер, чтобы больше не кликать
        if (objCollider != null) objCollider.enabled = false;

        // 2. Плавное растворение остатков куба
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
            
            // Скрываем куб полностью после растворения, пока доигрывает звук
            meshRenderer.enabled = false;
        }

        // 3. Ждем окончания звука
        if (audioSource != null && audioSource.clip != null)
        {
            while (audioSource.isPlaying)
            {
                yield return null;
            }
        }

        Destroy(gameObject);
    }

    // Хелпер для красивого создания эффекта
    private ParticleSystem InstantiatingParticles()
    {
        return Instantiate(explosionParticles, transform.position, transform.rotation);
    }
}