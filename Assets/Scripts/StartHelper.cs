using UnityEngine;

namespace Nuke.Scripts
{
    /// <summary>
    /// 启动延迟
    /// - 不能决定自己的开始
    /// </summary>
    public class StartHelper : MonoBehaviour
    {

        [Tooltip("需要被启用的物体")]
        public GameObject ActivatedGameObject;
        [Tooltip("启用延迟")]
        public float Delay = 0;

        // Awake
        void Awake()
        {
            // 找不到绑定对象 = 自己
            if (ActivatedGameObject == null)
            {
                ActivatedGameObject = gameObject;
            }
        }

        // OnEnable
        void OnEnable()
        {
            // 尾迹清除
            if (ActivatedGameObject.TryGetComponent(out TrailRenderer trail))
            {
                trail.Clear();
            }

            // 设置禁用
            ActivatedGameObject.SetActive(false);

            // 延迟加载
            Invoke(nameof(Activate), Delay);
        }

        // OnDisable
        void OnDisable()
        {
            CancelInvoke(nameof(Activate));
        }

        // 启用
        void Activate()
        {
            ActivatedGameObject.SetActive(true);
        }
    }
}