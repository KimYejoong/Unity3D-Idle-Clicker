using UnityEngine;
using UnityEngine.UI;

namespace Lower
{
    public class ListPanelSortButton : MonoBehaviour
    {
        [SerializeField]
        private ListPanelController listPanelController;
    
        private Text _buttonText;

        private void Awake()
        {
            _buttonText = GetComponentInChildren<Text>();
            UpdateText();
        }

        public void OnClick()
        {
            if (!listPanelController.isSorted) {
                listPanelController.SortContents();
                listPanelController.isSorted = true;
            }
            else
            {
                listPanelController.UndoSortContents();
                listPanelController.isSorted = false;
            }

            UpdateText();
        }

        private void UpdateText()
        {
            if (!listPanelController.isSorted)
                _buttonText.text = "Sort";
            else
                _buttonText.text = "Undo";
        }
    }
}
