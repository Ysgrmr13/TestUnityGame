using UnityEngine;

public static class Extensions
{
    public static Vector2 GetRandomPointInBounds(this Bounds bounds)
    {
        return new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
    }
    
    public static float GetDistance2D(this Transform from, Transform to)
    {
        return Vector2.Distance(from.position, to.position);
    }
    
    public static void SetAlpha(this SpriteRenderer renderer, float alpha)
    {
        Color color = renderer.color;
        color.a = alpha;
        renderer.color = color;
    }
}