using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool interact;
		public bool dodge;
		public bool equipWeapon;
		public bool lightAttack;
		public bool heavyAttack;
		public bool heal;
		public bool lockOn;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
		}

		public void OnDodge(InputValue value)
		{
			DodgeInput(value.isPressed);
		}

		public void OnEquipWeapon(InputValue value)
		{
			EquipWeaponInput(value.isPressed);
		}

		public void OnLightAttack(InputValue value)
		{
			LightAttackInput(value.isPressed);
		}

		public void OnHeavyAttack(InputValue value)
		{
			HeavyAttackInput(value.isPressed);
		}

		public void OnHeal(InputValue value)
		{
			HealInput(value.isPressed);
		}

		public void OnLockOn(InputValue value)
		{
			LockOnInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
		}

		public void DodgeInput(bool newDodgeState)
		{
			dodge = newDodgeState;
		}

		public void EquipWeaponInput(bool newEquipWeaponState)
		{
			equipWeapon = newEquipWeaponState;
		}

		public void LightAttackInput(bool newLightAttackState)
		{
			lightAttack = newLightAttackState;
		}

		public void HeavyAttackInput(bool newHeavyAttackState)
		{
			heavyAttack = newHeavyAttackState;
		}

		public void HealInput(bool newHealState)
		{
			heal = newHealState;
		}

		public void LockOnInput(bool newLockOnState)
		{
			lockOn = newLockOnState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}