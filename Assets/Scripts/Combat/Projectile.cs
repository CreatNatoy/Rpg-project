using RPG.Core;
using RPG.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGP.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 1;
        [SerializeField] private bool _isHoming = true;
        [SerializeField] private GameObject _hitEffect = null;
        [SerializeField] private float _maxLifeTime = 10;
        [SerializeField] private GameObject[] _destroyOnHit = null;
        [SerializeField] private float _lifeAfterImpact = 2;

        private Health _target = null;
        GameObject instigator = null;
        private float _damage = 0f;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (_target == null) return;
            if (_isHoming && !_target.IsDead())
                transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            _target = target;
            _damage = damage;
            this.instigator = instigator;
            

            Destroy(gameObject, _maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
                return _target.transform.position;
            return _target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != _target) return;
            if (_target.IsDead()) return;
            _target.TakeDamage(instigator, _damage);

            _speed = 0;

            if (_hitEffect != null)
            {
                Instantiate(_hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in _destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, _lifeAfterImpact);
        }
    }
}