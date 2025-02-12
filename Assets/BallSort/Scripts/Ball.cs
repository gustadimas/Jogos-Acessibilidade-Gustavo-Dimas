using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int GroupId;
    public SpriteRenderer spriteRenderer;
    public Color[] groupColors = new Color[0];
    public Sprite[] groupSprites = new Sprite[0];
    public bool useSprites = true;

    public int GroupIdProperty
    {
        get { return GroupId; }
        set
        {
            GroupId = value;
            if (groupSprites.Length <= GroupId)
            {
                Debug.LogWarning("No sprite/color for this group");
                return;
            }
            if (useSprites)
                spriteRenderer.sprite = groupSprites[GroupId];
            else
                spriteRenderer.color = groupColors[GroupId];
        }
    }

    public IEnumerator MoveTo(Vector2 target, float duration = 0.3f)
    {
        Vector2 _start = transform.localPosition;
        float _time = 0;
        while (_time < duration)
        {
            transform.localPosition = Vector2.Lerp(_start, target, _time / duration);
            _time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = target;
    }

    public void Move(Vector2 target, float duration = 0.3f)
    {
        StopAllCoroutines();
        StartCoroutine(MoveTo(target, duration));
    }
}