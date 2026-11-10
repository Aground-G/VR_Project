using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class DoorRotate : MonoBehaviour
{
    [Header("旋转设置")]
    public float openAngle = -90f;      // 开门角度（负值表示向外开）
    public float duration = 0.5f;       // 动画持续时间
    private bool isOpen = false;
    private bool isAnimating = false;

    private Quaternion closedRot;
    private Quaternion openRot;
    private XRSimpleInteractable interactable;

    void Start()
    {
        closedRot = transform.localRotation;
        openRot = closedRot * Quaternion.Euler(0, openAngle, 0);

        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
            interactable = gameObject.AddComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnInteract);
    }

    void OnInteract(SelectEnterEventArgs args)
    {
        if (isAnimating) return;
        StartCoroutine(RotateDoor(isOpen ? closedRot : openRot));
        isOpen = !isOpen;
    }

    IEnumerator RotateDoor(Quaternion targetRot)
    {
        isAnimating = true;
        Quaternion startRot = transform.localRotation;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0, 1, t);
            transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        transform.localRotation = targetRot;
        isAnimating = false;
    }
}