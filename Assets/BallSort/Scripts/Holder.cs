using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Holder : MonoBehaviour
{
    public int maxBalls = 4;
    public float ballRadius = 0.5f;
    public List<Ball> balls = new List<Ball>();
    public bool IsPending;

    public bool IsFull => balls.Count >= maxBalls;
    public Ball TopBall => balls.LastOrDefault();

    public void AddBall(Ball ball, bool instant = false)
    {
        if (IsFull)
            return;
        balls.Add(ball);
        ball.transform.SetParent(transform, false);
        Vector2 _targetPos = GetBallPosition(balls.Count - 1);
        if (instant)
            ball.transform.localPosition = _targetPos;
        else
            ball.Move(_targetPos);
    }

    public Ball RemoveTopBall()
    {
        if (balls.Count == 0)
            throw new InvalidOperationException("Holder is empty!");
        Ball _ball = balls.Last();
        balls.Remove(_ball);
        return _ball;
    }

    public Vector2 GetBallPosition(int index)
    {
        return new Vector2(0, (index + 0.5f) * 2 * ballRadius);
    }
}