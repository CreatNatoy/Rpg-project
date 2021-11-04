using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;

    /*LateUpdate: LateUpdate() вызываетс€ один раз в кадре, после завершени€ Update().
     * Ћюбые расчеты, которые осуществл€ютс€ в Update() будет завершены, при вызове LateUpdate().
     * ќсновным использованием LateUpdate() обычно €вл€етс€ слежение за камерой от третьего лица. 
     * ≈сли ¬ы осуществите движение ¬ашего персонажа в событии Update(),
     * то движени€ камеры и расчЄтов еЄ месторасположени€ можете вести в событии LateUpdate(). 
     * Ёто будет гарантировать, что персонаж прошел полностью перед камерой, и закрепил свое расположение. */
    private void LateUpdate()
    {
        transform.position = _target.position; 
    }
}
