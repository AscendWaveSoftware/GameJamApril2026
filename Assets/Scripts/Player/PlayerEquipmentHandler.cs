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

            var knife = new Rifle(2.2f, 2.0f);
            
            backpack.AddEquipable(knife);
            backpack.EquipWeapon(knife);
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
