using UnityEngine;
using UnityEngine.SceneManagement;

public class VerificadorVitoria : MonoBehaviour
{
    public Associacao[] locaisDeAssociacao;
    public Associar[] locaisDeArraste;
    int quantidadeDePecasNoLugar = 0;

    void Update()
    {
        int _totalEsperado = 0;
        if (locaisDeAssociacao != null && locaisDeAssociacao.Length > 0)
            _totalEsperado = Mathf.Max(_totalEsperado, locaisDeAssociacao.Length);
        if (locaisDeArraste != null && locaisDeArraste.Length > 0)
            _totalEsperado = Mathf.Max(_totalEsperado, locaisDeArraste.Length);

        if (_totalEsperado > 0 && quantidadeDePecasNoLugar >= _totalEsperado)
        {
            Debug.Log("Você Venceu!");
        }
    }

    public void RegistrarPecaNoLugar() => quantidadeDePecasNoLugar++;
}