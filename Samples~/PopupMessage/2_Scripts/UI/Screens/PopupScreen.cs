using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RealMethod;
#if TMP_PRESENT
using TMPro;
#endif

public sealed class PopupScreen : MonoBehaviour, IWidget, IInformer
{
    [Header("Setting")]
    [SerializeField]
    private float moveUpDistance = 50f;
    [SerializeField]
    private float duration = 1.0f;   // move+fade duration
    [SerializeField]
    private float popDuration = 0.12f;
    [SerializeField]
    private float popOvershoot = 1.08f;
    [Header("Dependency")]
#if TMP_PRESENT
[SerializeField]
    private TMP_Text messageText;
#else
    [SerializeField]
    private Text messageText;
#endif


    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Vector2 baseAnchoredPos;                  // baseline position
    private Coroutine routine;

    // Implement IWidget Interface
    public MonoBehaviour GetWidgetClass()
    {
        return this;
    }
    public void InitiateWidget(Object Owner)
    {
        rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.LogWarning($"{this}: should add beside RectTransform");
            return;
        }
        baseAnchoredPos = rect.anchoredPosition;      // capture baseline once
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogWarning($"{this}: should add beside CanvasGroup");
            return;
        }
        canvasGroup.alpha = 0f;
        rect.localScale = Vector3.one;
        // Optional (prevents blocking clicks behind it):
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        messageText.supportRichText = true; // make sure rich text is on
        if (Owner is ScreenManager manager)
        {
            manager.SetInformer(this);
        }
    }

    // Implement IScreenMessage Interface
    public void Popup(string text)
    {
        if (!enabled)
            return;
        // Always reset to baseline so repeated taps don't accumulate offset
        if (routine != null) StopCoroutine(routine);
        rect.anchoredPosition = baseAnchoredPos;
        rect.localScale = Vector3.one;
        canvasGroup.alpha = 0f;

        routine = StartCoroutine(PopupRoutine(text, duration));
    }
    public void Popup(string text, float dura)
    {
        if (!enabled)
            return;
        // Always reset to baseline so repeated taps don't accumulate offset
        if (routine != null) StopCoroutine(routine);
        rect.anchoredPosition = baseAnchoredPos;
        rect.localScale = Vector3.one;
        canvasGroup.alpha = 0f;

        routine = StartCoroutine(PopupRoutine(text, dura));
    }
    // If your UI layout can move this element at runtime (e.g., safe area/orientation),
    // you can expose this to re-capture baseline:
    public void RecalibrateBaseline() => baseAnchoredPos = rect.anchoredPosition;

    // Enumerator
    private IEnumerator PopupRoutine(string text, float timer)
    {
        messageText.text = text;

        // Quick "pop" in
        float t = 0f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / popDuration);
            float s = Mathf.Lerp(1f, popOvershoot, n);
            rect.localScale = new Vector3(s, s, 1f);
            canvasGroup.alpha = n; // fade in with the pop
            yield return null;
        }

        // Settle scale back to 1
        const float settle = 0.08f;
        t = 0f;
        while (t < settle)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / settle);
            float s = Mathf.Lerp(popOvershoot, 1f, n);
            rect.localScale = new Vector3(s, s, 1f);
            yield return null;
        }

        // Move up + fade out
        t = 0f;
        Vector2 start = baseAnchoredPos;
        Vector2 end = baseAnchoredPos + new Vector2(0f, moveUpDistance);
        while (t < timer)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / timer);
            // Ease-out cubic for nicer motion
            float ease = 1f - Mathf.Pow(1f - n, 3f);
            rect.anchoredPosition = Vector2.LerpUnclamped(start, end, ease);
            canvasGroup.alpha = 1f - n;
            yield return null;
        }

        // Clean reset
        rect.anchoredPosition = baseAnchoredPos;
        rect.localScale = Vector3.one;
        canvasGroup.alpha = 0f;
        routine = null;
    }

}
