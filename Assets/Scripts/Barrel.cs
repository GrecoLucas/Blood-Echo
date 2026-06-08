using UnityEngine;

public class TravaBarril : MonoBehaviour
{
    [Header("Configurações Opcionais")]
    public bool centralizarNoAlvo = true;

    // Digite aqui o nome exato da Layer do chão do seu jogo (geralmente "Default")
    public string nomeLayerChao = "Default";

    private void OnTriggerEnter(Collider outro)
    {
        if (outro.CompareTag("BarrilPuzzle"))
        {
            Rigidbody rb = outro.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                // 1. Trava a física
                rb.isKinematic = true;
                
                // 2. Troca os colisores
                CapsuleCollider capsula = outro.GetComponent<CapsuleCollider>();
                if (capsula != null) capsula.enabled = false;

                BoxCollider caixa = outro.GetComponent<BoxCollider>();
                if (caixa != null) caixa.enabled = true;
                
                // 3. A SUA SOLUÇÃO: Muda a Layer para sair do CanPush
                // Isso diz ao jogador que este objeto agora é "chão" pisável
                outro.gameObject.layer = LayerMask.NameToLayer(nomeLayerChao);
                
                // 4. Centraliza no alvo
                if (centralizarNoAlvo)
                {
                    Vector3 novaPosicao = new Vector3(transform.position.x, outro.transform.position.y, transform.position.z);
                    outro.transform.position = novaPosicao;
                    outro.transform.rotation = Quaternion.identity; 
                }

                // Desliga o Trigger
                GetComponent<Collider>().enabled = false;
            }
        }
    }
}