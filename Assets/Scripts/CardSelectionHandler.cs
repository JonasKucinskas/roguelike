using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private AudioManager audioManager;
    [SerializeField] private float _verticalMoveAmmount = 30f;
    [SerializeField] private float _moveTime = 0.1f;
    [Range(0f, 2f), SerializeField] private float _scaleAmmount = 1.1f;

    private Vector3 _startScale;
    private Card card;
    private CardManager cardManager;
    private Vector2 initialCursorPosition;
    private void Start()
    {
        card = gameObject.GetComponent<Card>();
        _startScale = transform.localScale;
        cardManager = GameObject.Find("CardManager").GetComponent<CardManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    private IEnumerator MoveCard(bool startingAnimation)
    {
        Vector3 endPosition;
        Vector3 endScale;
        Quaternion endRotation;

        Vector3 originalPosition = gameObject.GetComponent<Card>().originalPosition;
        Vector3 currentPosition = gameObject.GetComponent<RectTransform>().anchoredPosition3D;
        Quaternion originalRotation = gameObject.GetComponent<Card>().originalRotation;

        float elapsedTime = 0f;
        while(elapsedTime < _moveTime)
        {
            elapsedTime += Time.deltaTime;

            if(startingAnimation)
            {
                endPosition = currentPosition + new Vector3(0f, _verticalMoveAmmount, 2f);
                endScale = _startScale * _scaleAmmount;
                endRotation = Quaternion.Euler(Vector3.zero);
            }
            else
            {
                endPosition = originalPosition;
                endScale = _startScale;
                endRotation = originalRotation;
            }


            Vector3 lerpedPos = Vector3.Lerp(currentPosition, endPosition, elapsedTime / _moveTime);
            Vector3 lerpedScale = Vector3.Lerp(transform.localScale, endScale, elapsedTime / _moveTime);
            Quaternion lerpedRotation = Quaternion.Slerp(transform.rotation, endRotation, elapsedTime / _moveTime);
            

            gameObject.GetComponent<RectTransform>().anchoredPosition3D = lerpedPos;
            transform.localScale = lerpedScale;
            gameObject.GetComponent<RectTransform>().localRotation = lerpedRotation;
            
            yield return null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        initialCursorPosition = eventData.position;
        if (!cardManager.IsDragging())
        {
            eventData.selectedObject = gameObject;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (initialCursorPosition == eventData.position)
        {
            return;
        }

        //player is dragging some other card.
        if (cardManager.IsDragging() && !card.IsDragging)
        {
            return;
        }
        eventData.selectedObject = null;
    }

    public void OnSelect(BaseEventData eventData)
    {
        StartCoroutine(audioManager.PlaySound(audioManager.CardJump,0f));
        StartCoroutine(MoveCard(true));
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!cardManager.IsDragging() && !card.IsDragging)
        {
            StartCoroutine(MoveCard(false));
        }
    }
}
