using UnityEngine;
using UnityEngine.SceneManagement;

public class Associacao : MonoBehaviour
{
    public Transform pecaCorretaTransform;
    public ArrasteComRotacao scriptPecaCorreta;
    public VerificadorVitoria scriptVerificadorVitoria;
    public int variacaoAnguloMaximoPermitido = 5;
    public bool pecaNoLugar = false;

    void Update()
    {
        if (pecaNoLugar) return;

        if (Vector2.Distance(transform.position, pecaCorretaTransform.position) < 0.25f
            && VerificarSeAnguloEstaNosLimitesPermitidos())
        {
            pecaCorretaTransform.position = transform.position;
            pecaCorretaTransform.rotation = transform.rotation;
            scriptVerificadorVitoria.RegistrarPecaNoLugar();
            Destroy(scriptPecaCorreta);
            pecaNoLugar = true;
        }
    }

    bool VerificarSeAnguloEstaNosLimitesPermitidos()
    {
        float _anguloLocal = transform.rotation.eulerAngles.z;
        float _anguloAlvo = pecaCorretaTransform.rotation.eulerAngles.z;
        float _diferencaAngular = Mathf.DeltaAngle(_anguloLocal, _anguloAlvo);
        return Mathf.Abs(_diferencaAngular) <= variacaoAnguloMaximoPermitido;
    }
}