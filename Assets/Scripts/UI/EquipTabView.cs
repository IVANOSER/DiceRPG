using UnityEngine;

public class EquipTabView : MonoBehaviour
{
    [SerializeField] private GameObject characterPreviewRoot; 
    [SerializeField] private GameObject itemPickerRoot;     

    public void OnShow()
    {
        if (characterPreviewRoot) characterPreviewRoot.SetActive(true);

        if (itemPickerRoot) itemPickerRoot.SetActive(true);
    }

    public void OnHide()
    {
        
        if (itemPickerRoot) itemPickerRoot.SetActive(false);

        
        if (characterPreviewRoot) characterPreviewRoot.SetActive(false);
    }
}
