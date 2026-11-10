using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class GroupLightSwitch : MonoBehaviour
{
    [Header("要控制的所有灯光")]
    public List<Light> allLights = new List<Light>();   // 手动拖入所有路灯的Light组件

    private bool isOn = true;  // 记录当前灯光状态

    void Start()
    {
        
        // 获取本物体上的 XR Simple Interactable 组件
        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
        {
            Debug.LogError("需要 XR Simple Interactable 组件，已自动添加");
            interactable = gameObject.AddComponent<XRSimpleInteractable>();
        }

        // 监听点击事件
        interactable.selectEntered.AddListener(OnLampClicked);

        // 如果没有手动拖拽 allLights，就自动查找场景中所有的 Light（不推荐，可能会误包含）
        if (allLights == null || allLights.Count == 0)
        {
            Debug.LogWarning("没有手动指定灯光列表，将自动查找场景中所有 Light 组件，请确认是否包含不需要控制的灯");
            Light[] foundLights = FindObjectsOfType<Light>();
            allLights = new List<Light>(foundLights);
        }

        // 确保所有灯的初始状态一致（全部打开）
        SetAllLights(true);
    }

    private void OnLampClicked(SelectEnterEventArgs args)
    {
        // 切换状态
        isOn = !isOn;
        SetAllLights(isOn);
    }

    private void SetAllLights(bool turnOn)
    {
        foreach (Light lamp in allLights)
        {
            if (lamp != null)
                lamp.enabled = turnOn;
        }
    }

    private void OnDestroy()
    {
        // 移除事件监听，避免内存泄漏
        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnLampClicked);
    }
}