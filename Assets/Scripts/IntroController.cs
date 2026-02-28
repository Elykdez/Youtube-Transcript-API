using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Nuke.Scripts
{
    /// <summary>
    /// UI元素显示隐藏
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class IntroController : MonoBehaviour
    {
        [Tooltip("显示延迟")]
        public float showDelay;

        [Tooltip("渐变时间")]
        public float fadeTime;

        [Tooltip("渐变过渡")]
        public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // 元素控制
        CanvasGroup uiGroup;

        // OnEnable
        void OnEnable()
        {
            // 画布控制
            uiGroup = GetComponent<CanvasGroup>();
            // 简单切换显示
            if (showDelay >= 0)
            {
                Show(showDelay);
            }
            else
            {
                Hide(Mathf.Abs(showDelay));
            }
        }

        // 切换显示隐藏
        IEnumerator SetVisible(float delay)
        {
            yield return new WaitForSeconds(delay);
            float elapsed = 0;
            float percentComplete;
            bool state = uiGroup.alpha == 0;
            uiGroup.interactable = state;
            uiGroup.blocksRaycasts = state;
            // 切换显示
            // Debug.Log("[UI]: Showing!");
            while (elapsed < fadeTime)
            {
                // 已过时间
                elapsed += Time.deltaTime;
                // 完成比例
                percentComplete = elapsed / fadeTime;
                uiGroup.alpha = fadeCurve.Evaluate(percentComplete);
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        /// <summary>
        /// 设置面板显示或者隐藏(延迟)
        /// </summary>
        /// <param name="state"></param>
        /// <param name="delay"></param>
        public void Display(float delay = 0)
        {
            StartCoroutine(SetVisible(delay));
        }

        /// <summary>
        /// 显示面板 - Fade
        /// </summary>
        void Show(float time = 0)
        {
            uiGroup.alpha = 0;
            Display(time);
        }

        /// <summary>
        /// 隐藏面板 - Fade
        /// </summary>
        void Hide(float time = 0)
        {
            uiGroup.alpha = 1;
            Display(time);
        }

    }
}