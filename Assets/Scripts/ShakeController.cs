using UnityEngine;
using System.Collections;

namespace Nuke.Scripts
{
    /// <summary>
    /// 相机抖动控制
    /// </summary>
    public class ShakeController : MonoBehaviour
    {
        [Tooltip("控制相机\n(默认主相机)")]
        public Camera relatedCamera;
        [Tooltip("持续时间")]
        public float duration = 0.75f;
        [Tooltip("最大影响距离\n(0无效, -1无限大)")]
        public float effectDistance;
        [Tooltip("抖动速度")]
        public float speed = 25;
        [Tooltip("震幅\n(视角方向)")]
        public float magnitude = 2.5f;
        [Tooltip("震动放大系数\n(晃动方向)")]
        public float amplifier = 0.5f;
        [Tooltip("晃动调整\n(单位向量，填比例)")]
        public Vector3 shakeOffset;
        [Tooltip("动画曲线")]
        public AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        [Tooltip("结束后重制")]
        public bool isResetOnEnd;
        [Tooltip("是否默认启用")]
        public bool isEnabled = true;

        // 是否执行中
        bool isPlaying;
        // 允许更新(实际执行类似单例)
        [HideInInspector]
        public bool canUpdate;

        // OnEnable
        void OnEnable()
        {
            // 默认开启执行效果
            isPlaying = true;
            // 静止其它控制脚本控制相机抖动
            foreach (var shake in FindObjectsOfType<ShakeController>())
            {
                shake.canUpdate = false;
            }
            canUpdate = true;
            // 找到相机
            if (relatedCamera == null)
            {
                // 允许绑定在相机上产生0距离抖动
                if (TryGetComponent(out Camera cam))
                {
                    effectDistance = -1;
                    relatedCamera = cam;
                }
                else
                {
                    relatedCamera = Camera.main;
                }
            }
        }

        // Update
        void Update()
        {
            if (isPlaying && isEnabled)
            {
                // 可随时开关(放在start里面就不能像这样开关了)
                isPlaying = false;
                StopAllCoroutines();
                StartCoroutine(Shake());
            }
        }

        // 抖动
        IEnumerator Shake()
        {
            // 相机
            var camT = relatedCamera.transform;
            Debug.Log($"[Dev]: Camera {camT} is shaking! (Should only start once)");
            // 逝去时间
            float elapsed = 0;
            // 影响距离阻尼(小于0 = 无阻碍， 等于0 = 无限阻碍)
            float distanceDamper = effectDistance > 0
                ? 1 - Mathf.Clamp01((camT.position - transform.position).magnitude / effectDistance)
                : effectDistance < 0 ? 1 : 0;
            // 晃动朝向
            var facing = (transform.position - camT.position).normalized;
            // 初始位置信息
            var originalCamPos = camT.position;
            var originalCamRot = camT.rotation.eulerAngles;
            // 旋转调整(欧拉角)
            Vector3 rotOffset = Vector3.zero;

            // 持续执行直到完成
            while (canUpdate && elapsed < duration)
            {
                // Debug.Log($"[Dev]: Shaking!");
                // 更新相机初始状态
                if (Quaternion.Euler(originalCamRot + rotOffset) != camT.rotation)
                {
                    originalCamRot = camT.rotation.eulerAngles;
                }

                // 已过时间
                elapsed += Time.deltaTime;
                // 完成比例
                var percentComplete = elapsed / duration;
                // 通过曲线控制衰减(曲线最终位置为0等于归位)
                var damper = shakeCurve.Evaluate(percentComplete) * distanceDamper;

                // 位移调整 - 此处为近似模拟冲击波的影响，damper(本身小于0)计算次数越多衰减越明显。
                var moveOffset = Mathf.Sin(elapsed * speed * damper) * magnitude * damper; // * damper
                                                                                           // 视角周期性缩放(放大晃动效果)
                camT.position -= facing * Time.deltaTime * moveOffset / 2;
                // 随机晃动
                var lookDir = (2 * (Random.insideUnitSphere + new Vector3(-0.5f, -0.5f, -0.5f)) + shakeOffset.normalized) * amplifier;
                // 朝目标方向旋转
                rotOffset = lookDir * moveOffset;
                camT.rotation = Quaternion.Euler(originalCamRot + rotOffset);

                // 下次迭代时继续从此处开始执行while内计算
                yield return null;
            }

            // 结束后重制(无视中间一些计算误差与非归0曲线等)
            // 与其它相机位置控制组合时建议关闭
            if (isResetOnEnd && elapsed >= duration)
            {
                camT.transform.position = originalCamPos;
                camT.transform.rotation = Quaternion.Euler(originalCamRot);
            }
        }
    }
}