using UnityEngine;
using UnityEngine.UI;

public class ParryManager : MonoBehaviour
{
    [SerializeField] BoxCollider parryRange;

    private void Awake()
    {
        parryRange = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        Debug.Log("방패 장착!");
    }

    private void OnDisable()
    {
        Debug.Log("방패 해제!");
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌체가 투사체이고 충돌한 면이 방패의 back 방향일 때
        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            Debug.Log($"패링 성공! 막은 공격 오브젝트의 이름 : { other.name} ");
        }
        else if (other.gameObject.CompareTag("Weapon"))
        {
            Transform trans = other.transform;
            while (!trans.gameObject.CompareTag("Enemy"))
            {
                trans = trans.parent;
            }
            trans.gameObject.GetComponent<Attack>().ParrySccuess();
            Debug.Log($"패링 성공! 막은 공격 오브젝트의 이름 : {other.name} ");
        }
    }

    public void ParryOn() => parryRange.enabled = true;
    public void ParryOff() => parryRange.enabled = false;
}
