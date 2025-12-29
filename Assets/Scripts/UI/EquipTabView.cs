using UnityEngine;

public class EquipTabView : MonoBehaviour
{
    [SerializeField] private GameObject characterPreviewRoot; 
    [SerializeField] private GameObject itemPickerRoot;
    [SerializeField] private GameObject DiceSkill; 
    [SerializeField] private GameObject DiceMod;      

    public void OnShow()
    {
        if (characterPreviewRoot) characterPreviewRoot.SetActive(true);

        if (itemPickerRoot) itemPickerRoot.SetActive(true);

        if (DiceSkill) DiceSkill.SetActive(true);

        if (DiceMod) DiceMod.SetActive(true);
    }

    public void OnHide()
    {
        
        if (itemPickerRoot) itemPickerRoot.SetActive(false);

        if (characterPreviewRoot) characterPreviewRoot.SetActive(false);

        if (DiceSkill) DiceSkill.SetActive(false);

        if (DiceMod) DiceMod.SetActive(false);

    }
}
