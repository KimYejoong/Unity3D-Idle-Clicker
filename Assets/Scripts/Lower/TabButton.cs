using UnityEngine;

namespace Lower
{
    public class TabButton : MonoBehaviour
    {
        private LowerMenu _lowerMenu;

        [SerializeField]
        private ListPanelController listPanelController;

        private void Awake()
        {
            _lowerMenu = GetComponentInParent<LowerMenu>();
        }

        public void OnClick()
        {
            _lowerMenu.CloseAllTabs();
            listPanelController.OpenTab();
        }
    }
}
