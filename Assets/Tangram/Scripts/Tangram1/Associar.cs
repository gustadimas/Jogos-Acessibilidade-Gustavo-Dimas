using UnityEngine;

public class Associar : MonoBehaviour
{
    public Transform pecaCorretaTransform;
    public Arraste scriptArrastarDaPeca;
    public VerificadorVitoria scriptVerificadorVitoria;
    bool pecaNoLugar = false;

    void Update()
    {
        if(pecaNoLugar) return;

        if(Vector2.Distance(transform.position, pecaCorretaTransform.position) < 0.25f)
        {
            pecaCorretaTransform.position = transform.position;
            Destroy(scriptArrastarDaPeca);
            pecaNoLugar = true;
            scriptVerificadorVitoria.RegistrarPecaNoLugar();
        }
    }

}
