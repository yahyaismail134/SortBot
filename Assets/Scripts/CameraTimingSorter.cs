using System.Collections;
using UnityEngine;

public class CameraTimingSorter : MonoBehaviour
{
    [System.Serializable]
    public class SortTask
    {
        public string categoryName;
        public Transform item;
        public Transform targetBasket;

        [HideInInspector] public bool isSorted;
        [HideInInspector] public bool isBeingSorted;
    }

    [Header("Points")]
    public Transform sortingPoint;

    [Header("Items and Baskets")]
    public SortTask[] tasks;

    [Header("Conveyor Settings")]
    public float beltSpeed = 1.5f;
    public float sortingDistanceThreshold = 0.08f;

    [Header("Basket Movement")]
    public float moveToBasketDuration = 1f;
    public float finalDropAmount = 0.25f;

    void Update()
    {
        MoveItemsOnBelt();
    }

    void MoveItemsOnBelt()
    {
        if (sortingPoint == null) return;

        foreach (SortTask task in tasks)
        {
            if (task.item == null || task.targetBasket == null) continue;
            if (task.isSorted || task.isBeingSorted) continue;

            float itemY = task.item.position.y;

            Vector3 sortingPos = new Vector3(
                sortingPoint.position.x,
                itemY,
                sortingPoint.position.z
            );

            // كل العناصر تتحرك مع السير في نفس الوقت ناحية SortingPoint
            task.item.position = Vector3.MoveTowards(
                task.item.position,
                sortingPos,
                beltSpeed * Time.deltaTime
            );

            float distanceToSortingPoint = Vector3.Distance(task.item.position, sortingPos);

            // أول ما أي item يوصل نقطة الفرز، يبدأ يتحول للباسكت بتاعه
            if (distanceToSortingPoint <= sortingDistanceThreshold)
            {
                StartCoroutine(SortToBasket(task));
            }
        }
    }

    IEnumerator SortToBasket(SortTask task)
    {
        task.isBeingSorted = true;

        Transform itemToSort = task.item;
        Transform targetBasket = task.targetBasket;

        float itemY = itemToSort.position.y;

        Debug.Log("Sorting " + task.categoryName + " to " + targetBasket.name);

        // يتحرك أفقياً ناحية الباسكت بدون ما يغطس في السير
        Vector3 sideMoveStart = itemToSort.position;

        Vector3 sideMoveTarget = new Vector3(
            targetBasket.position.x,
            itemY,
            targetBasket.position.z
        );

        float t = 0;
        while (t < moveToBasketDuration)
        {
            t += Time.deltaTime;
            itemToSort.position = Vector3.Lerp(sideMoveStart, sideMoveTarget, t / moveToBasketDuration);
            yield return null;
        }

        // نزلة بسيطة في آخر الحركة كأنه وقع جوه الباسكت
        Vector3 dropStart = itemToSort.position;
        Vector3 dropTarget = dropStart + new Vector3(0, -finalDropAmount, 0);

        t = 0;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            itemToSort.position = Vector3.Lerp(dropStart, dropTarget, t / 0.4f);
            yield return null;
        }

        task.isSorted = true;
        task.isBeingSorted = false;

        Debug.Log(task.categoryName + " sorted successfully.");
    }
}