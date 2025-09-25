using UnityEngine;

public class DeactivateAfterDelay : MonoBehaviour
{
    [SerializeField] private float delay = 1f; // 失活延迟时间，默认2秒

    private void OnEnable()
    {
        // 当GameObject被激活时，调用Deactivate方法，延迟指定时间
        Invoke(nameof(Deactivate), delay);
    }

    private void OnDisable()
    {
        // 当GameObject被禁用时，取消Invoke调用，防止重复执行
        CancelInvoke();
    }

    private void Deactivate()
    {
        // 将当前GameObject设置为非激活状态
        gameObject.SetActive(false);
    }
}