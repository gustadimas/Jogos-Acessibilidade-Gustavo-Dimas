using UnityEngine;

public class Arraste : MonoBehaviour
{
    void OnMouseDrag()
	{
		float _posicaoEmZObjetoNaTela = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 _posicaoObjetoNaTela = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _posicaoEmZObjetoNaTela);
        Vector3 _posicaoObjetoNoMundo = Camera.main.ScreenToWorldPoint(_posicaoObjetoNaTela);
		transform.position = _posicaoObjetoNoMundo;
	}
}