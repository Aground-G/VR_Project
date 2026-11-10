using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResetOnRelease : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private XRGrabInteractable grabInteractable;

    void Start()
    {
        // 记录初始位置和旋转
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // 获取 XR Grab Interactable 组件（确保物体有这个组件）
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            // 监听“松开”事件
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // 重置位置和旋转
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // 可选：重置速度（防止物理残留）
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // 可选：在物体被销毁时移除监听，避免内存泄漏
    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.selectExited.RemoveListener(OnRelease);
    }
}
