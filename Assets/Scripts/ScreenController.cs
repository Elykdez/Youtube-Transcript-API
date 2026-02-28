using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Nuke.Scripts
{
    /// <summary>
    /// 屏幕控制
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ScreenController : MonoBehaviour
    {
        [Header("测试")]
        [Tooltip("关闭")] public Button closeBtn;
        [Tooltip("输入测试")] public InputField textField;

        private void Start()
        {
            if (closeBtn != null) closeBtn.onClick.AddListener(OnExit);
            if (textField != null) textField.onEndEdit.AddListener(SubmitTest);
        }

        void OnExit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        void SubmitTest(string txt)
        {
            if (txt.Length > 0 && txt[0] == '-') textField.text = txt.Remove(0, 1);
            Debug.Log($"[聊天]: 切换到房间 -> {(int.TryParse(txt, out int txtNumber) ? txtNumber : 0)}");
        }
    }
}