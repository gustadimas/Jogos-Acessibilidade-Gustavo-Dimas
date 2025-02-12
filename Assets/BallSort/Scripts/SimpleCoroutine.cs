using System;
using System.Collections;
using UnityEngine;

public class SimpleCoroutine : MonoBehaviour
{
    public static IEnumerator LerpNormalizedEnumerator(Action<float> onFrame, float targetNormalized = 1.5f, float lerpSpeed = 8f)
    {
        float _current = 0f;
        while (_current < 1f)
        {
            _current = Mathf.Lerp(_current, targetNormalized, lerpSpeed * Time.deltaTime);
            if (_current >= 1f)
                _current = 1f;
            onFrame?.Invoke(_current);
            yield return null;
        }
    }
}