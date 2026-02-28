using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nuke.Scripts
{
    /// <summary>
    /// 闪光光源控制
    /// </summary>
    public class FlashController : MonoBehaviour
    {
        [Header("相机瞄准设置")]
        [Tooltip("对准相机")]
        public Camera cam;

        [Tooltip("闪光强度")]
        public float flashStrength;

        [Tooltip("渐变时间")]
        public float fadeTime;

        [Tooltip("渐变过渡")]
        public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Header("日光遮蔽设置")]
        [Tooltip("太阳")]
        public Light sunLight;

        [Tooltip("日光遮蔽")]
        public AnimationCurve occultationCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        Quaternion originalRot;
        Light flashLight;
        bool isFlashing;

        // Start is called before the first frame update
        void Awake()
        {
            originalRot = transform.rotation;
            flashLight = GetComponent<Light>();
            if (cam == null)
            {
                cam = Camera.main;
            }
        }

        // Update is called once per frame
        void Update()
        {
            AlignWithMainCam();
            if (!isFlashing && cam.isActiveAndEnabled)
            {
                StopAllCoroutines();
                StartCoroutine(NukeFlash());
            }
        }

        void OnDisable()
        {
            transform.rotation = originalRot;
        }

        // 与主相机对齐
        void AlignWithMainCam()
        {
            transform.LookAt(cam.transform.position);
        }

        // 聚变闪光
        IEnumerator NukeFlash()
        {
            isFlashing = true;
            float elapsed = 0;
            float percentComplete;
            float sunStrength = sunLight.intensity;
            // 切换显示
            // Debug.Log("[UI]: Showing!");
            while (elapsed < fadeTime)
            {
                // 已过时间
                elapsed += Time.deltaTime;
                // 完成比例
                percentComplete = elapsed / fadeTime;
                flashLight.intensity = flashStrength * fadeCurve.Evaluate(percentComplete);
                sunLight.intensity = sunStrength * occultationCurve.Evaluate(percentComplete);
                // Debug.Log($"Time = {percentComplete}, Intensity = {flashLight.intensity}");
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }
}
