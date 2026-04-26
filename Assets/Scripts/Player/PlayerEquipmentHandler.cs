using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Backpack))]
    public class PlayerEquipmentHandler : MonoBehaviour
    {
        [SerializeField] private bool addStarterWeaponsIfEmpty = true;
        [SerializeField] private float starterKnifeFireRate = 2f;
        [SerializeField] private float starterKnifeDamage = 1f;
        [SerializeField] private float starterRifleFireRate = 6f;
        [SerializeField] private float starterRifleDamage = 1f;

        private Backpack backpack = new Backpack();
        private int _weaponIndex = -1;

        private void Awake()
        {
            if (addStarterWeaponsIfEmpty && (backpack.GetWeapons() == null || backpack.GetWeapons().Length == 0))
            {
                Weapon knife = new Knife(starterKnifeFireRate, starterKnifeDamage);
                Weapon rifle = new Rifle(starterRifleFireRate, starterRifleDamage);
                backpack.AddEquipable(knife);
                backpack.AddEquipable(rifle);
                backpack.EquipWeapon(knife);
                _weaponIndex = 0;
            }
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            CycleNextWeapon();
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            CyclePreviousWeapon();
        }

        public void CycleNextWeapon()
        {
            CycleWeapon(1);
        }

        public void CyclePreviousWeapon()
        {
            CycleWeapon(-1);
        }

        public Weapon GetEquippedWeapon()
        {
            return backpack != null ? backpack.EquipedWeapon : null;
        }

        public string GetEquippedWeaponName()
        {
            if (backpack == null || backpack.EquipedWeapon == null)
                return "None";
            return backpack.EquipedWeapon.GetType().Name;
        }

        private void CycleWeapon(int direction)
        {
            if (backpack == null) return;

            IAmEquipable[] weapons = backpack.GetWeapons();
            if (weapons == null || weapons.Length == 0) return;

            if (_weaponIndex < 0 || _weaponIndex >= weapons.Length)
            {
                _weaponIndex = 0;
                for (int i = 0; i < weapons.Length; i++)
                {
                    if (weapons[i] == backpack.EquipedWeapon)
                    {
                        _weaponIndex = i;
                        break;
                    }
                }
            }

            _weaponIndex = (_weaponIndex + direction % weapons.Length + weapons.Length) % weapons.Length;

            if (weapons[_weaponIndex] is Weapon nextWeapon)
            {
                backpack.EquipWeapon(nextWeapon);
                Debug.Log($"[Equipment] Equipped: {nextWeapon.GetType().Name} (slot {_weaponIndex + 1}/{weapons.Length})");
            }
        }
    }
}
