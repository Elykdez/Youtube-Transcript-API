using UnityEngine;

namespace Nuke.Scripts
{
    /// <summary>
    /// 结束延迟
    /// - 但可决定自己的结束
    /// </summary>
    public class EndHelper : MonoBehaviour
    {
        [Tooltip("需要被销毁的物体")]
        public GameObject DeActivatedGameObject;
        [Tooltip("销毁延迟")]
        public bool isDestroyed;
        [Tooltip("销毁延迟")]
        public float Delay = 0;

        // Awake
        void Awake()
        {
            // 找不到绑定对象 = 自己
            if (DeActivatedGameObject == null)
            {
                DeActivatedGameObject = gameObject;
            }
        }

        // OnEnable
        void OnEnable()
        {
            Invoke(nameof(Collect), Delay);
        }

        // OnDisable
        void OnDisable()
        {
            CancelInvoke(nameof(Collect));
        }

        // 启用
        void Collect()
        {
            if (isDestroyed)
            {
                Destroy(DeActivatedGameObject);
            }
            else
            {
                DeActivatedGameObject.SetActive(false);
            }

        }
    }
}