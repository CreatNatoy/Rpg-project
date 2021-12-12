using RGP.Combat;
using RPG.Core;
using UnityEngine; 

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order =0)]
    public class Weapon: ScriptableObject
    {
        [SerializeField] private GameObject equippedPrefab = null;
        [SerializeField] private AnimatorOverrideController _animatorOverride = null;
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private float _weaponDamage = 2f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] private Projectile _projectile = null;

        const string _weaponName = "Weapon"; 

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand); 
            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
               GameObject weapon =  Instantiate(equippedPrefab, handTransform);
                weapon.name = _weaponName; 
            }

               var overrideController =  animator.runtimeAnimatorController as AnimatorOverrideController;

            if (_animatorOverride != null)
            {
                animator.runtimeAnimatorController = _animatorOverride;
            }
            else if(overrideController != null)
                    {
                    animator.runtimeAnimatorController = overrideController.runtimeAnimatorController; 
            }
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(_weaponName);
            if (oldWeapon == null)
                oldWeapon = leftHand.Find(_weaponName);
            if (oldWeapon == null) 
                return;

            oldWeapon.name = "DESTROYING"; 
            Destroy(oldWeapon.gameObject); 
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded)
                handTransform = rightHand;
            else
                handTransform = leftHand;
            return handTransform;
        }

        public bool HasProjectile()
        {
            return _projectile != null; 
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)   
        {
            Projectile projectileInstace = Instantiate(_projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstace.SetTarget(target, _weaponDamage); 
        }

        public float GetGamage()
        {
            return _weaponDamage; 
        }

        public float GetRange()
        {
            return _weaponRange; 
        }    
    }
}