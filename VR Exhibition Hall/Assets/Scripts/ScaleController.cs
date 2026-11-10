using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI; // 记得引用UI的命名空间

public class ScaleController : MonoBehaviour
{
    [Header("缩放设置")]
    [Tooltip("缩放的最小值")]
    public float minScale = 0.5f;
    [Tooltip("缩放的最大值")]
    public float maxScale = 2.0f;

    private Slider scaleSlider;
    public bool faceToPlayer = true;
    private Transform cameraTransform;

    // Start 会在物体被激活时调用
    void Start()
    {
        // 获取挂载在子物体上的Slider组件
        scaleSlider = GetComponentInChildren<Slider>();
        if (scaleSlider != null)
        {
            // 设置滑块的取值范围，并添加监听事件
            scaleSlider.minValue = minScale;
            scaleSlider.maxValue = maxScale;
            scaleSlider.value = 1.0f; // 初始缩放为1倍
            scaleSlider.onValueChanged.AddListener(OnScaleValueChanged);
        }
        if (faceToPlayer)
        {
            XROrigin xrOrigin = FindObjectOfType<XROrigin>();
            if (xrOrigin != null)
                cameraTransform = xrOrigin.Camera.transform;
            else
                Debug.LogWarning("未找到 XR Origin 或 Camera");
        }
    }

    // 当滑块的值发生变化时，这个方法会被自动调用
    void OnScaleValueChanged(float value)
    {
        // 调整当前物体的缩放比例
        transform.localScale = Vector3.one * value;
    }
    private void Update()
    {
        if (!faceToPlayer) return;
        Vector3 direction = cameraTransform.position - transform.position;
        direction.y = 0;
        if (direction == Vector3.zero) return;
        Quaternion targetRotation = Quaternion.LookRotation(-direction);
    }
}