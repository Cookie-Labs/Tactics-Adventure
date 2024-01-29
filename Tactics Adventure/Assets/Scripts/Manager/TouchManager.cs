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
        Card_Player playerCard = SpawnManager.Instance.playerCard;

        if (!card.availableTouch || isTouching || playerCard.isMoving)
            yield break;

        isTouching = true;

        card.DoCard();
        yield return new WaitForSeconds(0.1f);
        spawnManager.DoTurnCards();
        yield return new WaitForSeconds(0.1f);

        SetTouch(playerCard, spawnManager.cardList);

        isTouching = false;
    }

    // 플레이어 위치에 따른 카드 설정
    public void SetTouch(Card_Player playerCard, List<Card> cardList)
    {
        Card[] nearCard = playerCard.FindNeighbors(new Direction[] { Direction.T, Direction.B, Direction.L, Direction.R });

        // 모든 카드 터치 비활성화
        foreach (Card card in cardList)
            card.availableTouch = false;

        // 인접한 카드 터치 활성화
        foreach (Card near in nearCard)
        {
            near.availableTouch = true;
        }

        playerCard.availableTouch = true; // 자신은 항상 터치 활성화
    }
}
