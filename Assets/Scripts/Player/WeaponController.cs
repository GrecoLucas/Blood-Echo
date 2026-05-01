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
    private static readonly int HeavyAttackHash = Animator.StringToHash("HeavyAttack");

    // Adicione estes Hashes no topo com os outros
    private static readonly int DrawHash = Animator.StringToHash("Draw");
    private static readonly int SheathHash = Animator.StringToHash("Sheath");
 
    public bool IsArmed { get; private set; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        ResolvePlayerDamageDealer();

        string damageDealerName = playerDamageDealer != null ? playerDamageDealer.name : "NULL";
    }

    private void Start()
    {
        // Começa desarmado: espada nas costas visível, na mão invisível
        IsArmed = false;
        SetWeaponVisuals(false);
    }

    private void Update()
    {
        // F para sacar / guardar a espada
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleWeapon();
        }
    }

    private void ToggleWeapon()
    {
        IsArmed = !IsArmed;
        
        // Dispara o Trigger específico para a camada UpperBody
        if (IsArmed)
        {
            _animator.SetTrigger(DrawHash);
        }
        else
        {
            _animator.SetTrigger(SheathHash);
            
            if (playerDamageDealer != null)
            {
                playerDamageDealer.EndDealingDamage();
            }
        }
    
        // Mantém o Armed bool para a Base Layer trocar a pose das pernas
        _animator.SetBool(ArmedHash, IsArmed);
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
            return;
        }

        _animator.SetTrigger(Attack1Hash);
    }
    // Chamado pelo ThirdPersonController
    public void TriggerHeavyAttack()
    {
        if (!IsArmed)
        {
            return;
        }

        _animator.SetTrigger(HeavyAttackHash);
    }
    
    // Chamado via Animation Event no ataque do player
    public void StartDealingHeavyDamage()
    {
        if (playerDamageDealer != null)
        {
            playerDamageDealer.StartDealingDamage(true);
        }
    }
        // Chamado via Animation Event no ataque do player
        public void EndDealingHeavyDamage()
    {
        if (playerDamageDealer != null)
        {
            playerDamageDealer.EndDealingDamage();
        }
    }

    // Chamado via Animation Event no ataque do player
    public void StartDealingDamage()
    {
        if (playerDamageDealer != null)
        {
            playerDamageDealer.StartDealingDamage();
        }
    }

    // Chamado via Animation Event no ataque do player
    public void EndDealingDamage()
    {
        if (playerDamageDealer != null)
        {
            playerDamageDealer.EndDealingDamage();
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
                playerDamageDealer.owner = DamageDealer.DamageOwner.Player;
            }
        }
    }
}