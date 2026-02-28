using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nuke.Scripts
{
    /// <summary>
    /// 弹道导弹控制
    /// </summary>
    public class MissileController : MonoBehaviour
    {
        [Header("场景控制")]
        [Tooltip("核爆动画播放延迟")] public float nukeDelay;
        [Tooltip("相机抖动延迟")] public float cameraShakeDelay;
        [Tooltip("导弹")] public GameObject missile;
        [Tooltip("核爆")] public GameObject nuke;
        [Tooltip("物体")] public GameObject subject;
        [Tooltip("闪光")] public Image flash;
        [Tooltip("闪光颜色")] public Color flashColor;

        // 当前动画阶段
        int stage;

        // Start
        void Start()
        {
            // 发射导弹
            missile.SetActive(true);
            StartCoroutine(NukeAnim());
        }

        // 核爆
        IEnumerator NukeAnim()
        {
            // 导弹飞行
            yield return new WaitForSeconds(nukeDelay);
            flash.gameObject.SetActive(true);
            flash.color = flashColor;
            // 核聚变
            nuke.SetActive(true);
            // 冲击波抵达
            yield return new WaitForSeconds(cameraShakeDelay);
            missile.SetActive(false);
            // 相机抖动
            nuke.GetComponent<ShakeController>().enabled = true;
            // 物体炸飞
            var blownAnim = subject.GetComponent<Animation>();
            if (stage < 1 && !blownAnim.isPlaying)
            {
                stage++;
                blownAnim.Play();
            }
        }
    }
}
