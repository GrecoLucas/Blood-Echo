using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Objects")]
    [SerializeField] private GameObject greatswordInHand;
    [SerializeField] private GameObject greatswordOnBack;

    [Header("Combat")]
    [SerializeField] private DamageDealer playerDamageDealer;

    [Header("Animation")]
    private Animator _animator;
    private static readonly int ArmedHash = Animator.StringToHash("Armed");
    private static readonly int Attack1Hash = Animator.StringToHash("Attack1");

    public bool IsArmed { get; private set; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        ResolvePlayerDamageDealer();

        string damageDealerName = playerDamageDealer != null ? playerDamageDealer.name : "NULL";
        Debug.Log($"[WeaponController] Awake -> IsArmed={IsArmed}, playerDamageDealer={damageDealerName}");
    }

    private void Start()
    {
        // Começa desarmado: espada nas costas visível, na mão invisível
        IsArmed = false;
        SetWeaponVisuals(false);
        Debug.Log("[WeaponController] Start -> arma guardada, visuals atualizados.");
    }

    private void Update()
    {
        // F para sacar / guardar a espada
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log($"[WeaponController] Input F detetado -> antes do toggle IsArmed={IsArmed}");
            ToggleWeapon();
        }
    }

    private void ToggleWeapon()
    {
        IsArmed = !IsArmed;
        _animator.SetBool(ArmedHash, IsArmed);
        Debug.Log($"[WeaponController] ToggleWeapon -> IsArmed={IsArmed}");

        if (!IsArmed && playerDamageDealer != null)
        {
            Debug.Log("[WeaponController] Arma guardada -> a parar dano ativo.");
            playerDamageDealer.EndDealingDamage();
        }
    }

    public void ShowWeaponOnBack()
    {
        if (greatswordInHand != null) greatswordInHand.SetActive(false);
        if (greatswordOnBack != null) greatswordOnBack.SetActive(true);
    }

    public void ShowWeaponInHand()
    {
        if (greatswordInHand != null) greatswordInHand.SetActive(true);
        if (greatswordOnBack != null) greatswordOnBack.SetActive(false);
    }
    private void SetWeaponVisuals(bool armed)
    {
        if (greatswordInHand != null)
            greatswordInHand.SetActive(armed);

        if (greatswordOnBack != null)
            greatswordOnBack.SetActive(!armed);
    }

    // Chamado pelo ThirdPersonController
    public void TriggerAttack()
    {
        if (!IsArmed)
        {
            Debug.LogWarning("[WeaponController] TriggerAttack ignorado porque o player está desarmado.");
            return;
        }

        Debug.Log("[WeaponController] TriggerAttack -> a disparar animação Attack1.");
        _animator.SetTrigger(Attack1Hash);
    }

    // Chamado via Animation Event no ataque do player
    public void StartDealingDamage()
    {
        if (playerDamageDealer != null)
        {
            Debug.Log("[WeaponController] StartDealingDamage -> evento de animação recebido.");
            playerDamageDealer.StartDealingDamage();
        }
        else
        {
            Debug.LogWarning("[WeaponController] StartDealingDamage chamado mas playerDamageDealer é NULL.");
        }
    }

    // Chamado via Animation Event no ataque do player
    public void EndDealingDamage()
    {
        if (playerDamageDealer != null)
        {
            Debug.Log("[WeaponController] EndDealingDamage -> evento de animação recebido.");
            playerDamageDealer.EndDealingDamage();
        }
        else
        {
            Debug.LogWarning("[WeaponController] EndDealingDamage chamado mas playerDamageDealer é NULL.");
        }
    }

    private void ResolvePlayerDamageDealer()
    {
        if (greatswordInHand != null)
        {
            DamageDealer[] swordDamageDealers = greatswordInHand.GetComponentsInChildren<DamageDealer>(true);
            if (swordDamageDealers.Length > 0)
            {
                playerDamageDealer = swordDamageDealers[0];
            }
        }

        if (playerDamageDealer == null)
        {
            playerDamageDealer = GetComponentInChildren<DamageDealer>(true);
        }

        if (playerDamageDealer != null)
        {
            if (playerDamageDealer.owner != DamageDealer.DamageOwner.Player)
            {
                Debug.LogWarning($"[WeaponController] Forcing {playerDamageDealer.name} owner to Player for player attacks.");
                playerDamageDealer.owner = DamageDealer.DamageOwner.Player;
            }
        }
        else
        {
            Debug.LogWarning("[WeaponController] Nenhum DamageDealer encontrado na espada do player.");
        }
    }
}