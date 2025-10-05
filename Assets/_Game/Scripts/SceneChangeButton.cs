using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int sceneIndex;

    [Header("Fade Animation")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float fadeMinAlpha=0.1f;

    private Button button;
    private IEnumerator fadeCoroutine;

    private void Awake()
    {
        button = GetComponent<Button>();
        canvasGroup.alpha = fadeMinAlpha;
    }
    private void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(OnButtonClick);
    }
    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(OnButtonClick);
    }
    private void OnButtonClick()
    {
        ChangeScene(sceneIndex);
    }
    private void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartFade(1f);

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        StartFade(fadeMinAlpha);
    }
    private void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = FadeTo(targetAlpha);
        StartCoroutine(fadeCoroutine);
    }
    private IEnumerator FadeTo(float target)
    {
        float start = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
    }
}
