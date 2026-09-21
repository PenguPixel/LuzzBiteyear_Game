using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonTextController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("TMP Font Colors")]
    public Color normalColor = new();
    public Color hoverColor = new();
    public Color pressedColor = new();

    private TMP_Text tmpText;
    private Button parentButton;

    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        parentButton = GetComponentInParent<Button>();

        if (tmpText != null) tmpText.color = normalColor;
        
        if (parentButton != null)
        {
            parentButton.onClick.AddListener(() => EventSystem.current.SetSelectedGameObject(null));
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => tmpText.color = hoverColor;
    public void OnPointerExit(PointerEventData eventData)  => tmpText.color = normalColor;
    public void OnPointerDown(PointerEventData eventData)  => tmpText.color = pressedColor;
    public void OnPointerUp(PointerEventData eventData)    => tmpText.color = hoverColor;
}
