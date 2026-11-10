using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ExhibitController : MonoBehaviour
{
    [Header("UI 设置")]
    public GameObject infoPanelPrefab;   // 信息面板预制体（需包含 Text 和 Button）
    public string exhibitName = "展品名";
    public string description = "详细介绍……";

    [Header("语音")]
    public AudioClip voiceClip;

    private GameObject currentPanel;      // 当前实例化的面板
    private AudioSource audioSource;
    private Material originalMaterial;    // 保存原始材质
    private Material highlightMaterial;   // 高亮材质（缓存）
    private Renderer objRenderer;         // 展品渲染器
    private bool isPanelActive = false;   // 面板是否显示

    void Start()
    {
        // 获取渲染器和原始材质
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
            originalMaterial = objRenderer.material;

        // 添加交互组件
        var interactable = gameObject.AddComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnInteract);

        // 音频组件
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = voiceClip;
        audioSource.playOnAwake = false;
    }

    private void OnInteract(SelectEnterEventArgs args)
    {
        if (isPanelActive)
        {
            // 如果面板已经显示，则这次点击关闭面板
            ClosePanel();
        }
        else
        {
            // 显示面板、播放语音、高亮
            ShowPanel();
            PlayVoice();
            ApplyHighlight();
        }
    }

    private void ShowPanel()
    {
        if (infoPanelPrefab == null)
        {
            Debug.LogError("缺少信息面板预制体！");
            return;
        }

        // 实例化面板，放在展品上方 2 米处，并旋转180度
        if (currentPanel == null)
        {
            Vector3 panelPos = transform.position + Vector3.forward * 6f+Vector3.up*2f;
            currentPanel = Instantiate(infoPanelPrefab, panelPos, Quaternion.Euler(0, 180, 0));

            // 设置面板上的文字（假设预制体中有两个 Text 组件：标题和描述）
            Text[] texts = currentPanel.GetComponentsInChildren<Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = exhibitName;
                texts[1].text = description;
            }

            // 绑定关闭按钮事件（假设预制体中有一个 Button 组件）
            Button closeBtn = currentPanel.GetComponentInChildren<Button>();
            if (closeBtn != null)
                closeBtn.onClick.AddListener(ClosePanel);
            else
                Debug.LogWarning("信息面板中没有找到 Button 组件，无法关闭");
        }
        else
        {
            currentPanel.SetActive(true);
        }

        isPanelActive = true;
    }

    private void ClosePanel()
    {
        if (currentPanel != null)
            currentPanel.SetActive(false);
        isPanelActive = false;
        RemoveHighlight();   // 恢复材质
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Pause();
    }

    private void PlayVoice()
    {
        if (audioSource != null && voiceClip != null)
        {
            audioSource.Stop();
            audioSource.Play();
        }
    }

    private void ApplyHighlight()
    {
        if (objRenderer == null) return;

        // 如果高亮材质还未创建，则创建一份（基于原始材质的副本，避免丢失贴图）
        if (highlightMaterial == null)
        {
            if (originalMaterial != null)
            {
                highlightMaterial = new Material(originalMaterial);
                highlightMaterial.color = Color.yellow;
            }
            else
            {
                highlightMaterial = new Material(Shader.Find("Standard"));
                highlightMaterial.color = Color.yellow;
            }
        }
        objRenderer.material = highlightMaterial;
    }

    private void RemoveHighlight()
    {
        if (objRenderer != null && originalMaterial != null)
        {
            objRenderer.material = originalMaterial;
        }
    }
}