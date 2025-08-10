using UnityEngine;
using UnityEngine.EventSystems;
public class Part : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;
    private CanvasGroup canvasGroup;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
    }
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
    }
    public void OnDrop(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = new Vector2(
            Mathf.Round(rectTransform.anchoredPosition.x / 100f) * 100f,
            Mathf.Round(rectTransform.anchoredPosition.y / 100f) * 100f
        );
    }
}
