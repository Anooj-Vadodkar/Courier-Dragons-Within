using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableText : MonoBehaviour, IPointerClickHandler
{

    public delegate void TextClickedDelegate(
            ClickableText clickable, PointerEventData eventData);

    public TextClickedDelegate TextClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        TextClicked?.Invoke(this, eventData);
    }

    public bool IsPointerOver()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
