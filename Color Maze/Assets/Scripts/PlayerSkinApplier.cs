using UnityEngine;

/// <summary>
/// 挂在 Player 上：进关时按 GameSession 的装备皮肤换 SpriteRenderer。
/// 找不到条目（如默认皮肤未建条目）时保留场景里摆好的 sprite。
/// </summary>
public class PlayerSkinApplier : MonoBehaviour
{
    [SerializeField] private ContentCatalog catalog;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        if (catalog == null)
        {
            MazeBootstrapper maze =
                FindAnyObjectByType<MazeBootstrapper>();
            if (maze != null)
            {
                catalog = maze.CosmeticsCatalog;
            }
        }

        if (catalog == null || spriteRenderer == null)
        {
            Debug.LogError(
                "PlayerSkinApplier: catalog 或 SpriteRenderer 未绑定。",
                this);
            return;
        }

        Sprite skin =
            catalog.ResolveSkin(GameSession.EquippedSkinId);

        if (skin != null)
        {
            spriteRenderer.sprite = skin;
            spriteRenderer.color = Color.white;
        }
    }
}
