using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameFeedback : MonoBehaviour
{
    [SerializeField] private AudioManager audio;
    [SerializeField] private Image screenFlash;
    [SerializeField]
    private Color damageColor =
        new Color(0.75f, 0.12f, 0.10f, 0.22f);
    [SerializeField]
    private Color gainColor =
        new Color(1f, 0.78f, 0.18f, 0.12f);
    [SerializeField]
    private Color portalColor =
        new Color(0.52f, 0.22f, 0.72f, 0.16f);

    private Coroutine _flashRoutine;

    private void Awake()
    {
        if (audio == null)
        {
            Debug.LogError(
                "GameFeedback: 请指定 Audio。",
                this);
            enabled = false;
            return;
        }

        if (screenFlash != null)
        {
            screenFlash.raycastTarget = false;
            ClearFlash();
        }
    }

    public void Damage()
    {
        audio.PlayDamage();
        Flash(damageColor, 0.2f);
    }

    public void Pickup()
    {
        audio.PlayPickup();
        Flash(gainColor, 0.12f);
    }

    public void ChestOpened()
    {
        audio.PlayChestOpen();
        Flash(gainColor, 0.18f);
    }

    public void StoneBroken()
    {
        audio.PlayStoneBreak();
        Flash(gainColor, 0.1f);
    }

    public void PortalUsed()
    {
        audio.PlayPortal();
        Flash(portalColor, 0.16f);
    }

    public void Error()
    {
        audio.PlayUiError();
        Flash(damageColor, 0.1f);
    }

    public void BombPlaced()
    {
        audio.PlayUiConfirm();
        Flash(portalColor, 0.08f);
    }

    public void BombExploded()
    {
        audio.PlayStoneBreak();
        Flash(damageColor, 0.14f);
    }

    private void Flash(Color color, float duration)
    {
        if (screenFlash == null)
        {
            return;
        }

        if (_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
        }

        _flashRoutine = StartCoroutine(
            FlashRoutine(color, duration));
    }

    private IEnumerator FlashRoutine(
        Color color,
        float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress =
                Mathf.Clamp01(elapsed / duration);

            screenFlash.color = new Color(
                color.r,
                color.g,
                color.b,
                Mathf.Lerp(color.a, 0f, progress));

            yield return null;
        }

        ClearFlash();
        _flashRoutine = null;
    }

    private void ClearFlash()
    {
        if (screenFlash == null)
        {
            return;
        }

        Color color = screenFlash.color;
        color.a = 0f;
        screenFlash.color = color;
    }
}
