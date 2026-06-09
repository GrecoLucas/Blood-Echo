using UnityEngine;
using FMODUnity;

public class BossBackgroundMusic : MonoBehaviour
{
    [Header("FMOD Audio")]
    [Tooltip("Arraste o StudioEventEmitter da música do Boss para cá")]
    [SerializeField] private StudioEventEmitter bossMusicEmitter;

    [Header("Referências")]
    public EnemyHealth bossHealth; // Referência para a vida do Boss

    private PlayerHealth playerHealth;
    
    // Começa como true para a verificação do Update ficar inativa até a luta começar
    private bool musicaParada = true; 

    void Start()
    {
        // Tenta pegar o EnemyHealth automaticamente caso não tenha sido arrastado no Inspector
        if (bossHealth == null)
        {
            bossHealth = GetComponent<EnemyHealth>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Pega o health direto do objeto que entrou na área
            // Isso é excelente para lidar com respawns caso o player seja recriado
            playerHealth = other.GetComponent<PlayerHealth>();

            // Verifica se as entidades estão vivas
            bool playerMorto = playerHealth != null && playerHealth.currentHealth <= 0;
            bool bossMorto = bossHealth != null && bossHealth.IsDead;

            // Só reinicia a música se AMBOS (Player e Boss) estiverem vivos
            if (!playerMorto && !bossMorto)
            {
                if (!bossMusicEmitter.IsPlaying())
                {
                    bossMusicEmitter.Play();
                    musicaParada = false; // Destrava a verificação do Update para parar a música depois
                }
            }
        }
    }

    // Opcional: Se o jogador puder fugir da arena, a música para ao sair
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !musicaParada)
        {
            PararMusicaDoBoss();
        }
    }

    void Update()
    {
        // Se a música já parou, não precisamos ficar verificando a vida o tempo todo
        if (musicaParada) return;

        // Verifica status do player
        bool playerMorto = playerHealth != null && playerHealth.currentHealth <= 0;
        
        // Verifica status do boss
        bool bossMorto = bossHealth != null && bossHealth.IsDead;

        // Se o player OU o boss morrerem durante a luta, chama a função de parar a música
        if (playerMorto || bossMorto)
        {
            PararMusicaDoBoss();
        }
    }

    private void PararMusicaDoBoss()
    {
        if (bossMusicEmitter != null && bossMusicEmitter.IsPlaying())
        {
            // Manda o FMOD parar com fade out
            bossMusicEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        
        // Trava essa lógica para não rodar repedidas vezes e permite que o OnTriggerEnter reinicie depois
        musicaParada = true; 
    }
}