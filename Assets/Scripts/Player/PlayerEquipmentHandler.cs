using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Backpack))]
    public class PlayerEquipmentHandler : MonoBehaviour
    {
        private Backpack backpack;

        private void Awake()
        {
            backpack = GetComponent<Backpack>();
            var weapon = new AN94(0.1f, 1f);
            
            //TODO For testing
            backpack.AddEquipable(weapon);
            backpack.EquipWeapon(weapon);
        }

        public Weapon GetEquippedWeapon()
        {
            return backpack != null ? backpack.EquipedWeapon : null;
        }

        public string GetEquippedWeaponName()
        {
            if (backpack == null || backpack.EquipedWeapon == null)
            {
                return "None";
            }
            return backpack.EquipedWeapon.GetType().Name;
        }
    }
}

