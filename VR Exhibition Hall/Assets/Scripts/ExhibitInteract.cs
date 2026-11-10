using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using Unity.XR.CoreUtils;

public class ExhibitWithUIControl : MonoBehaviour
{
    [Header("UI画布")]
    public GameObject infoCanvas;

    [Header("音频")]
    public AudioSource audioSource;
    public AudioClip audioClip;

    [Header("高亮材质")]
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer objRenderer;

    [Header("面向玩家设置")]
    public bool faceToPlayer = true;
    public float rotationSpeed = 5f;
    private Transform cameraTransform;

    private XRSimpleInteractable interactable;

    void Start()
    {
        // 获取渲染器
        objRenderer = GetComponent<Renderer>();
        if (objRenderer == null)
            objRenderer =transform.GetChild(4).GetComponent<Renderer>();
        if (objRenderer != null)
            originalMaterial = objRenderer.material;

        // 交互组件
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
            interactable = gameObject.AddComponent<XRSimpleInteractable>();

        interactable.selectEntered.AddListener(OnExhibitClicked);

        if (infoCanvas != null)
            infoCanvas.SetActive(false);

        // 音频
        if (audioSource == null && audioClip != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.playOnAwake = false;
        }

        // 绑定按钮事件
        BindButtonEvents();

        // 获取摄像机
        if (faceToPlayer)
        {
            XROrigin xrOrigin = FindObjectOfType<XROrigin>();
            if (xrOrigin != null)
                cameraTransform = xrOrigin.Camera.transform;
            else
                Debug.LogWarning("未找到 XR Origin 或 Camera");
        }
    }

    void Update()
    {

        if (!faceToPlayer) return;
        if (infoCanvas == null) return;
        if (!infoCanvas.activeInHierarchy) return;
        if (cameraTransform == null) return;

        Vector3 direction = cameraTransform.position - infoCanvas.transform.position;
        direction.y = 0;
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(-direction);
        if (rotationSpeed <= 0f)
            infoCanvas.transform.rotation = targetRotation;
        else
            infoCanvas.transform.rotation = Quaternion.Slerp(infoCanvas.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnExhibitClicked(SelectEnterEventArgs args)
    {
        ShowCanvas();
        ApplyHighlight();
    }

    public void ShowCanvas()
    {
        if (infoCanvas != null)
            infoCanvas.SetActive(true);
    }

    public void CloseCanvas()
    {
        if (infoCanvas != null)
            infoCanvas.SetActive(false);
        RemoveHighlight();
    }

    public void PlayVoice()
    {
        if (audioSource == null || audioSource.clip == null)
        {
            Debug.LogWarning("没有设置音频源或音频片段！");
            return;
        }

        // 判断当前状态，实现播放/暂停切换
        if (audioSource.isPlaying)
        {
            // 正在播放 → 暂停
            audioSource.Pause();
        }
        else
        {
            // 已暂停或停止 → 继续播放（如果是暂停状态则恢复，否则从头开始）
            if (audioSource.time > 0 && !audioSource.isPlaying)
            {
                // 如果之前暂停过，从暂停位置继续
                audioSource.UnPause();
            }
            else
            {
                // 从头开始播放
                audioSource.Play();
            }
        }
    }

    private void ApplyHighlight()
    {
        if (objRenderer != null && highlightMaterial != null)
        {
            if (originalMaterial == null)
                originalMaterial = objRenderer.material;
            objRenderer.material = highlightMaterial;
        }
    }

    private void RemoveHighlight()
    {
        if (objRenderer != null && originalMaterial != null)
            objRenderer.material = originalMaterial;
    }

    private void BindButtonEvents()
    {
        if (infoCanvas == null) return;

        Button playBtn = infoCanvas.transform.Find("PlayButton")?.GetComponent<Button>();
        if (playBtn != null)
        {
            playBtn.onClick.RemoveAllListeners();
            playBtn.onClick.AddListener(PlayVoice);
        }

        Button closeBtn = infoCanvas.transform.Find("CloseButton")?.GetComponent<Button>();
        if (closeBtn != null)
        {
            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener(CloseCanvas);
        }
    }

    public void OnPlayButtonClick() { PlayVoice(); }
    public void OnCloseButtonClick() { CloseCanvas(); }

    private void OnDestroy()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnExhibitClicked);
    }
}