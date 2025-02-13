using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallSortManager : MonoBehaviour
{
    public Holder holderPrefab;
    public Ball ballPrefab;
    public Transform holdersParent;
    public Camera mainCamera;

    public int numColors = 3;
    public int ballsPerTube = 4;
    public int totalTubes = 5;

    public List<Holder> holders = new List<Holder>();
    public enum State { Playing, Over }
    public State currentState = State.Playing;

    public Holder pendingHolder = null;

    public List<List<int>> defaultLevel = new List<List<int>>() {
        new List<int> { 0, 1, 2, 0 },
        new List<int> { 2, 1, 0, 1 },
        new List<int> { 2, 0, 1, 2 },
        new List<int>(),
        new List<int>()
    };

    public Stack<MoveData> undoStack = new Stack<MoveData>();

    public struct MoveData
    {
        public Holder FromHolder { get; set; }
        public Holder ToHolder { get; set; }
        public Ball Ball { get; set; }
    }

    public static event Action LevelCompleted;

    public void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        CreateLevel();
        currentState = State.Playing;
    }

    public void Update()
    {
        if (currentState != State.Playing)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 _mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Collider2D _col = Physics2D.OverlapPoint(_mouseWorld);
            if (_col != null)
            {
                Holder _holder = _col.GetComponent<Holder>();
                if (_holder != null)
                    OnHolderClicked(_holder);
            }
        }
    }

    public void OnHolderClicked(Holder _clickedHolder)
    {
        if (pendingHolder == null)
        {
            if (_clickedHolder.balls.Count > 0)
                pendingHolder = _clickedHolder;
        }
        else
        {
            if (pendingHolder == _clickedHolder)
            {
                pendingHolder = null;
                return;
            }
            if (!_clickedHolder.IsFull &&
                (_clickedHolder.balls.Count == 0 || _clickedHolder.TopBall.GroupId == pendingHolder.TopBall.GroupId))
            {
                MoveBall(pendingHolder, _clickedHolder);
                pendingHolder = null;
                CheckVictory();
            }
            else
            {
                pendingHolder = _clickedHolder.balls.Count > 0 ? _clickedHolder : null;
            }
        }
    }

    public void MoveBall(Holder _fromHolder, Holder _toHolder)
    {
        Ball _movingBall = _fromHolder.RemoveTopBall();
        _toHolder.AddBall(_movingBall);
    }

    public void CheckVictory()
    {
        bool _win = holders.All(_h => {
            var _b = _h.balls.ToList();
            return _b.Count == 0 || (_b.Count == ballsPerTube && _b.All(b => b.GroupId == _b.First().GroupId));
        });
        if (_win)
        {
            currentState = State.Over;
            Debug.Log("Você venceu!");
            CheckVictoryScene();
            LevelCompleted?.Invoke();
        }
    }

    public IEnumerable<Vector2> PositionsForHolders(int _count, out float _expectedWidth)
    {
        _expectedWidth = 4 * 2.5f;
        if (_count <= 6)
        {
            Vector2 _minPoint = (Vector2)transform.position - ((_count - 1) / 2f) * 2.5f * Vector2.right - Vector2.up;
            _expectedWidth = Mathf.Max(_count * 2.5f, _expectedWidth);
            return Enumerable.Range(0, _count)
                .Select(_i => _minPoint + _i * 2.5f * Vector2.right);
        }
        float _aspect = (float)Screen.width / Screen.height;
        int _maxCountInRow = Mathf.CeilToInt(_count / 2f);
        if ((_maxCountInRow + 1) * 2.5f > _expectedWidth)
            _expectedWidth = (_maxCountInRow + 1) * 2.5f;
        float _height = _expectedWidth / _aspect;
        List<Vector2> _list = new List<Vector2>();
        Vector2 _topRowMinPoint = (Vector2)transform.position + Vector2.up * _height / 6f - ((_maxCountInRow - 1) / 2f) * 2.5f * Vector2.right - Vector2.up;
        _list.AddRange(Enumerable.Range(0, _maxCountInRow)
                        .Select(_i => _topRowMinPoint + _i * 2.5f * Vector2.right));
        Vector2 _lowRowMinPoint = (Vector2)transform.position - Vector2.up * _height / 6f - (((_count - _maxCountInRow) - 1) / 2f) * 2.5f * Vector2.right - Vector2.up;
        _list.AddRange(Enumerable.Range(0, _count - _maxCountInRow)
                        .Select(_i => _lowRowMinPoint + _i * 2.5f * Vector2.right));
        return _list;
    }

    public void CreateLevel()
    {
        float _spacing = 2.5f;
        int _tubeCount = totalTubes;
        Vector2 _startPos = new Vector2(-_spacing * (_tubeCount - 1) / 2f, -2f);
        List<Vector2> _positions = new List<Vector2>();
        for (int _i = 0; _i < _tubeCount; _i++)
            _positions.Add(_startPos + new Vector2(_i * _spacing, 0));

        holders.Clear();
        for (int _i = 0; _i < _tubeCount; _i++)
        {
            Holder _h = Instantiate(holderPrefab, _positions[_i], Quaternion.identity, holdersParent);
            holders.Add(_h);
        }

        List<int> _ballGroups = new List<int>();
        for (int _c = 0; _c < numColors; _c++)
            for (int _j = 0; _j < ballsPerTube; _j++)
                _ballGroups.Add(_c);

        for (int _i = 0; _i < _ballGroups.Count; _i++)
        {
            int _rand = UnityEngine.Random.Range(_i, _ballGroups.Count);
            int _temp = _ballGroups[_i];
            _ballGroups[_i] = _ballGroups[_rand];
            _ballGroups[_rand] = _temp;
        }

        int _index = 0;
        for (int _i = 0; _i < numColors; _i++)
        {
            Holder _h = holders[_i];
            for (int _j = 0; _j < ballsPerTube; _j++)
            {
                Ball _b = Instantiate(ballPrefab);
                _b.GroupIdProperty = _ballGroups[_index];
                _index++;
                _h.AddBall(_b, true);
            }
        }
    }

    public void ResetLevel()
    {
        foreach (Holder _h in holders)
        {
            Destroy(_h.gameObject);
        }
        holders.Clear();
        undoStack.Clear();
        currentState = State.Playing;
        CreateLevel();
    }

    public void CheckVictoryScene()
    {
        PlayerPrefs.SetInt("LevelWon", 1);
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("ResultScene");
    }
}