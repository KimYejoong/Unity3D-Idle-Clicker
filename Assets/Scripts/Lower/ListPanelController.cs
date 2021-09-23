using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ListPanelController : MonoBehaviour
{
    [SerializeField] private RectTransform scrollContent;

    public string tabName;

    public bool isSorted;
    private List<Transform> _originalListInOrder;
    private Vector2 _originalLocation;

    private RectTransform _rect;
    private TabNameDisplay _tabNameDisplay;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _originalLocation = _rect.anchoredPosition;
        
        _originalListInOrder = new List<Transform>();

        foreach (Transform child in scrollContent)
            _originalListInOrder.Add(child.transform);

        _tabNameDisplay = FindObjectOfType<TabNameDisplay>();
    }

    public void OpenTab()
    {
        _rect.anchoredPosition = _originalLocation;
        scrollContent.anchoredPosition = new Vector3(0, 0, 0);

        _tabNameDisplay.UpdateTabName(tabName);
    }

    public void CloseTab()
    {
        _rect.anchoredPosition = Vector2.down * 1000f;
    }

    public void SortContents()
    {
        var sortedListInOrder =
            _originalListInOrder.OrderBy(item => item.GetComponent<Purchasable>().GetCost()).ToList();

        for (var i = 0; i < sortedListInOrder.Count; i++) sortedListInOrder[i].SetSiblingIndex(i);

        isSorted = true;
    }

    public void UndoSortContents()
    {
        for (var i = 0; i < _originalListInOrder.Count; i++) _originalListInOrder[i].SetSiblingIndex(i);

        isSorted = false;
    }

    public void TryUpdateSortContents()
    {
        if (isSorted) SortContents();
    }
}