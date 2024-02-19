using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : Singleton<TouchManager>
{
    public bool isTouching;

    private void Update()
    {
#if UNITY_ANDROID
        if (Input.touchCount <= 0 || Input.touchCount >= 2)
            return;

        if(Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            // Raycast to see if there is any object clicked
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

            // Check if an object is clicked
            if (hit.collider != null && hit.collider.CompareTag("Card"))
            {
                Card card = hit.collider.GetComponent<Card>();

                StartCoroutine(TouchEvent(card));
            }
        }
#elif UNITY_EDITOR || UNITY_WEBGL
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 inputPosition = Input.mousePosition;

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

            // Raycast to see if there is any object clicked
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            // Check if an object is clicked
            if (hit.collider != null && hit.collider.CompareTag("Card"))
            {
                Card card = hit.collider.GetComponent<Card>();

                StartCoroutine(TouchEvent(card));
            }
        }
#endif
    }

    IEnumerator TouchEvent(Card card)
    {
        SpawnManager spawnManager = SpawnManager.Instance;
        UIManager uiManager = UIManager.Instance;
        Card_Player playerCard = spawnManager.playerCard;

        if (Array.IndexOf(playerCard.neighborCards, card) == -1 || isTouching || playerCard.isMoving)
            yield break;

        isTouching = true;
        uiManager.skillUI.EnableActive(false);

        yield return card.DoCard();

        if (card != spawnManager.playerCard)
        {
            spawnManager.DoTurnCards();
            yield return new WaitForSeconds(0.1f);
            playerCard.SetNeighbor();
        }

        isTouching = false;
        uiManager.skillUI.EnableActive(true);
        yield return null;
    }


    public IEnumerator TouchEvent()
    {
        SpawnManager sm = SpawnManager.Instance;

        sm.DoTurnCards();
        yield return new WaitForSeconds(0.1f);

        sm.playerCard.SetNeighbor();

        yield return null;
    }
}
