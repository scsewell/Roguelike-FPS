using UnityEngine;
using UnityEngine.EventSystems;

public class SoundListener : MonoBehaviour,
    IPointerEnterHandler, ISelectHandler, 
    IPointerExitHandler, IDeselectHandler, 
    IPointerClickHandler, ISubmitHandler, 
    IDragHandler
{
    [SerializeField]
    AudioClip m_submitOverrideClip;

    private GameMenu m_menu;

	private void Start()
    {
        m_menu = transform.root.GetComponent<GameMenu>();
    }

    public void OnPointerEnter(PointerEventData e)
    {
        m_menu.PlaySelectSound();
    }

    public void OnPointerExit(PointerEventData e)
    {
        m_menu.PlayDeselectSound();
    }

    public void OnSelect(BaseEventData e)
    {
        m_menu.PlaySelectSound();
    }

    public void OnDeselect(BaseEventData e)
    {
        m_menu.PlayDeselectSound();
    }

    public void OnPointerClick(PointerEventData e)
    {
        Submit();
    }

    public void OnSubmit(BaseEventData e)
    {
        Submit();
    }

    private void Submit()
    {
        if (m_submitOverrideClip != null)
        {
            m_menu.PlaySound(m_submitOverrideClip);
        }
        else
        {
            m_menu.PlaySubmitSound();
        }
    }

    public void OnDrag(PointerEventData e)
    {
        m_menu.PlayDragSound();
    }
}
