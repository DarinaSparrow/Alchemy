using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.EventSystems;

[System.Serializable]
public class ObjectPrefabDictionary
{
    public string objectName;
    [SerializeField] private GameObject prefab;

    public GameObject Prefab => prefab;

    public ObjectPrefabDictionary(string objectName, GameObject prefab)
    {
        this.objectName = objectName;
        this.prefab = prefab;
    }   
}

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _draggingObject;
    private Collider2D _draggedCollider;

    [SerializeField] private GameObject PlayGround;

    public List<ObjectPrefabDictionary> objectPrefabs = new List<ObjectPrefabDictionary>();

    void Awake()
    {
        _draggingObject = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _draggedCollider = GetComponent<Collider2D>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _draggingObject.anchoredPosition += eventData.delta / _draggingObject.localScale.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Collider2D[] overlappedColliders = Physics2D.OverlapBoxAll(_draggedCollider.bounds.center, _draggedCollider.bounds.size, 0);
        foreach (Collider2D collider in overlappedColliders)
        {
            if (collider != _draggedCollider)
            {
                float overlapArea = CalculateOverlapArea(_draggedCollider, collider);

                float draggedArea = CalculateArea(_draggedCollider);
                float otherArea = CalculateArea(collider);

                if (overlapArea > draggedArea * 0.75f && overlapArea > otherArea * 0.75f)
                {
                    string collidedObjectName = collider.gameObject.name;

                    foreach (ObjectPrefabDictionary objDictionary in objectPrefabs)
                    {
                        if (collidedObjectName == objDictionary.objectName && objDictionary.Prefab!=null)
                        {
                            GameObject newObject = Instantiate(objDictionary.Prefab, collider.transform.position, Quaternion.identity);
                            newObject.transform.SetParent(PlayGround.transform);
                        }
                    }
                }

            }
        }
    }

    private float CalculateOverlapArea(Collider2D collider1, Collider2D collider2)
    {
        Bounds bounds1 = collider1.bounds;
        Bounds bounds2 = collider2.bounds;

        float minX = Mathf.Max(bounds1.min.x, bounds2.min.x);
        float minY = Mathf.Max(bounds1.min.y, bounds2.min.y);
        float maxX = Mathf.Min(bounds1.max.x, bounds2.max.x);
        float maxY = Mathf.Min(bounds1.max.y, bounds2.max.y);

        float width = Mathf.Max(0, maxX - minX);
        float height = Mathf.Max(0, maxY - minY);

        return width * height;
    }

    private float CalculateArea(Collider2D collider)
    {
        Bounds bounds = collider.bounds;
        return bounds.size.x * bounds.size.y;
    }
}
