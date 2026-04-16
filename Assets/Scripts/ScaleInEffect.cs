using UnityEngine;
using System.Collections;

public class ScaleInEffect : MonoBehaviour
{
    public float duration = 0.5f;
    public void Play(float targetScale)
    {
        StartCoroutine(ScaleRoutine(targetScale));
    }

    IEnumerator ScaleRoutine(float target)
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.1f)); //Damos tiempo a que se suceda la animación
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float scale = Mathf.Lerp(0f, target, t);
            transform.localScale = Vector3.one * scale;

            yield return null;
        }

        transform.localScale = Vector3.one * target;
    }


}
