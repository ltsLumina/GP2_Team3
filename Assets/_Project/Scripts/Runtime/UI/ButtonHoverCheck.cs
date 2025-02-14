using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverCheck : MonoBehaviour, IPointerEnterHandler
{

    [SerializeField] private FMODUnity.EventReference buttonHoverSFX;

    public void OnPointerEnter(PointerEventData eventData)
    {
        FMODUnity.RuntimeManager.PlayOneShot(buttonHoverSFX);
    }

}
