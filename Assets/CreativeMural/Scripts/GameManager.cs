using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum EnvironmentType { Ocean, Forest, Snow }

public class GameManager : MonoBehaviour
{
    public GameObject environmentOcean;
    public GameObject environmentForest;
    public GameObject environmentSnow;

    public GameObject animalPanel;
    public Transform[] panelPositions;

    public GameObject associationPanel;
    public Transform[] associationPositions;
    public bool[] associationOccupied;

    public List<Animal> animals = new List<Animal>();

    public Button verifyButton;
    public EnvironmentType currentEnvironment;

    public Animal animalInDrag = null;
    public Vector2 offset;

    public void OpenAnimalPanel() => animalPanel.SetActive(true);

    public void Start()
    {
        animalPanel.SetActive(false);

        foreach (Animal _a in animals)
            _a.originalParent = _a.animalObj.transform.parent;

        RandomizePanelPositions();

        if (associationPositions != null && associationPositions.Length > 0)
            associationOccupied = new bool[associationPositions.Length];

        if (environmentOcean != null) environmentOcean.SetActive(true);
        if (environmentForest != null) environmentForest.SetActive(false);
        if (environmentSnow != null) environmentSnow.SetActive(false);

        currentEnvironment = EnvironmentType.Ocean;

        if (verifyButton != null)
            verifyButton.onClick.AddListener(CheckEnvironment);
    }

    public void RandomizePanelPositions()
    {
        List<Vector2> _positions = new List<Vector2>();
        foreach (Animal _a in animals)
        {
            RectTransform _rt = _a.animalObj.GetComponent<RectTransform>();
            _positions.Add(_rt.anchoredPosition);
        }

        if (_positions.Count < animals.Count) return;

        for (int _i = 0; _i < _positions.Count; _i++)
        {
            int _randomIndex = Random.Range(_i, _positions.Count);
            Vector2 _temp = _positions[_i];
            _positions[_i] = _positions[_randomIndex];
            _positions[_randomIndex] = _temp;
        }

        for (int _i = 0; _i < animals.Count; _i++)
        {
            RectTransform _rt = animals[_i].animalObj.GetComponent<RectTransform>();
            _rt.SetParent(animalPanel.transform, false);
            _rt.anchoredPosition = _positions[_i];
            animals[_i].originalPanelPosition = _positions[_i];
        }
    }

    public void BeginDrag(GameObject animalObj, PointerEventData eventData)
    {
        animalInDrag = animals.Find(_a => _a.animalObj == animalObj);

        if (animalInDrag != null)
        {
            RectTransform _rt = animalObj.GetComponent<RectTransform>();
            RectTransform _parentRect = _rt.parent as RectTransform;
            Vector2 _localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, eventData.position, eventData.pressEventCamera, out _localPoint);
            offset = _rt.anchoredPosition - _localPoint;
        }
    }

    public void DuringDrag(GameObject animalObj, PointerEventData eventData)
    {
        if (animalInDrag != null)
        {
            RectTransform _rt = animalObj.GetComponent<RectTransform>();
            RectTransform _parentRect = _rt.parent as RectTransform;
            Vector2 _localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, eventData.position, eventData.pressEventCamera, out _localPoint);
            _rt.anchoredPosition = _localPoint + offset;
        }
    }

    public void EndDrag(GameObject animalObj, PointerEventData eventData)
    {
        if (animalInDrag != null)
        {
            bool _inAnimalPanel = IsPositionInPanel(eventData.position, eventData.pressEventCamera);
            bool _inAssociationPanel = IsPositionInAssociationPanel(eventData.position, eventData.pressEventCamera);

            if (_inAnimalPanel)
            {
                RectTransform _rt = animalObj.GetComponent<RectTransform>();
                _rt.anchoredPosition = animalInDrag.originalPanelPosition;
            }
            else if (_inAssociationPanel)
            {
                AssignAnimalToAssociationPosition(animalObj);
                animalPanel.SetActive(false);
            }
            else
            {
                animalPanel.SetActive(false);
                RectTransform _rt = animalObj.GetComponent<RectTransform>();
                _rt.anchoredPosition = animalInDrag.originalPanelPosition;
            }
            animalInDrag = null;
        }
    }

    public bool IsPositionInPanel(Vector3 screenPos, Camera cam)
    {
        RectTransform _rt = animalPanel.GetComponent<RectTransform>();
        Vector2 _localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rt, screenPos, cam, out _localPoint);
        return _rt.rect.Contains(_localPoint);
    }

    public bool IsPositionInAssociationPanel(Vector3 screenPos, Camera cam)
    {
        RectTransform _rt = associationPanel.GetComponent<RectTransform>();
        Vector2 _localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rt, screenPos, cam, out _localPoint);
        return _rt.rect.Contains(_localPoint);
    }

    public void AssignAnimalToAssociationPosition(GameObject animalObj)
    {
        if (associationPositions == null || associationPositions.Length == 0)
        {
            Debug.LogError("Variável associationPositions não definida!");
            return;
        }

        if (associationOccupied == null || associationOccupied.Length != associationPositions.Length)
        {
            associationOccupied = new bool[associationPositions.Length];
        }

        List<int> _freeIndices = new List<int>();

        for (int _i = 0; _i < associationPositions.Length; _i++)
        {
            if (!associationOccupied[_i])
                _freeIndices.Add(_i);
        }

        if (_freeIndices.Count > 0)
        {
            int _randomIndex = Random.Range(0, _freeIndices.Count);
            int _posIndex = _freeIndices[_randomIndex];
            RectTransform _animalRect = animalObj.GetComponent<RectTransform>();
            _animalRect.SetParent(associationPositions[_posIndex], false);
            _animalRect.anchoredPosition = Vector2.zero;
            associationOccupied[_posIndex] = true;
            Debug.Log("Animal associado a posição: " + _posIndex);
        }

        else
        {
            Debug.Log("Nenhuma posição de associação disponível. Retornando o animal.");
            RectTransform _animalRect = animalObj.GetComponent<RectTransform>();
            _animalRect.SetParent(animalPanel.transform, false);
            _animalRect.anchoredPosition = animalInDrag.originalPanelPosition;
        }
    }

    public bool IsAnimalAssociated(GameObject animalObj)
    {
        foreach (Transform _assoc in associationPositions)
        {
            if (animalObj.transform.parent == _assoc)
                return true;
        }

        return false;
    }

    public void CheckEnvironment()
    {
        bool _allCorrect = true;
        foreach (var _animal in animals)
        {
            if (_animal.correctEnvironment == currentEnvironment)
            {
                if (!IsAnimalAssociated(_animal.animalObj))
                {
                    _allCorrect = false;
                    break;
                }
            }
        }

        if (_allCorrect)
        {
            foreach (var _animal in animals)
            {
                if (_animal.correctEnvironment == currentEnvironment)
                    _animal.animalObj.SetActive(false);
            }

            Debug.Log("Ambiente " + currentEnvironment + " Completo!");
            ActivateNextEnvironment();
        }
        else
        {
            Debug.Log("Alguns animais não estão associados corretamente. Tente de novo!");
            ResetAnimalPositions();
        }
    }

    public void ClearAssociations()
    {
        if (associationPositions == null || associationOccupied == null)
            return;

        for (int _i = 0; _i < associationPositions.Length; _i++)
        {
            associationOccupied[_i] = false;

            if (associationPositions[_i].childCount > 0)
            {
                Transform _child = associationPositions[_i].GetChild(0);
                _child.SetParent(animalPanel.transform, false);
            }
        }
    }

    public void ActivateNextEnvironment()
    {
        ClearAssociations();

        switch (currentEnvironment)
        {
            case EnvironmentType.Ocean:
                if (environmentOcean != null) environmentOcean.SetActive(false);
                if (environmentForest != null) environmentForest.SetActive(true);
                currentEnvironment = EnvironmentType.Forest;
                Debug.Log("Ambiente de Floresta ativo!");
                break;

            case EnvironmentType.Forest:
                if (environmentForest != null) environmentForest.SetActive(false);
                if (environmentSnow != null) environmentSnow.SetActive(true);
                currentEnvironment = EnvironmentType.Snow;
                Debug.Log("Ambiente de Neve ativo!");
                break;

            case EnvironmentType.Snow:
                if (environmentSnow != null) environmentSnow.SetActive(false);
                Debug.Log("Todos os ambientes completos!");
                break;
        }
        ResetAnimalPositions();
    }

    public void ResetAnimalPositions()
    {
        animalPanel.SetActive(true);

        if (associationOccupied != null && associationOccupied.Length > 0)
        {
            for (int _i = 0; _i < associationOccupied.Length; _i++)
                associationOccupied[_i] = false;
        }

        foreach (var _animal in animals)
        {
            RectTransform _animalRect = _animal.animalObj.GetComponent<RectTransform>();
            _animalRect.SetParent(_animal.originalParent, false);
            _animalRect.anchoredPosition = _animal.originalPanelPosition;
        }
        RandomizePanelPositions();
    }
}

[System.Serializable]
public class Animal
{
    public string name;
    public EnvironmentType correctEnvironment;
    public GameObject animalObj;
    [HideInInspector] public Vector2 originalPanelPosition;
    [HideInInspector] public Transform originalParent;
}