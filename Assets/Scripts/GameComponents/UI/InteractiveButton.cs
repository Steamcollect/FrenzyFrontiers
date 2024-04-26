using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class InteractiveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ButtonState initialState, finalState;
    public float smoothTime;

    [Header("References")]
    public Image image;
    public TMP_Text text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null) image.DOColor(finalState.color, smoothTime);
        if (text != null) text.DOColor(finalState.color, smoothTime);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null) image.DOColor(initialState.color, smoothTime);
        if (text != null) text.DOColor(initialState.color, smoothTime);
    }
}

[System.Serializable]
public class ButtonState
{
    public Vector2 scale;
    public Color color;
}