using UnityEngine;

public class Testemovimentação : MonoBehaviour
{
    public float velocidadeTeste = 5f; // Ajuste no Inspector se precisar
    private Rigidbody2D rbTeste;

    void Start()
    {
        rbTeste = GetComponent<Rigidbody2D>();
    }

//testando
    void FixedUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        Vector2 direcao = new Vector2(inputX, inputY).normalized;
        rbTeste.linearVelocity = direcao * velocidadeTeste;
    }
}
