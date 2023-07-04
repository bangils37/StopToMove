using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] public CharacterController parent;
    [SerializeField] private List<CharacterController> listEnemy = new List<CharacterController>();

    private void OnTriggerEnter(Collider other)
    {
        CharacterController enemy = other.GetComponent<CharacterController>();

        if (enemy != null && enemy != parent && !listEnemy.Contains(enemy))
        {
            /// Ưu tiên pick Player làm target
            if (enemy is PlayerController)  
                listEnemy.Insert(0, enemy);
            else
                listEnemy.Add(enemy);
        }

        if(parent is PlayerController)
        {
            if (other.tag == "Building")
            {
                ChangeAlpha(other.GetComponent<Renderer>().material, 0.2f);
            }
            if (listEnemy.Count > 0 && listEnemy[0] != null)
            {
                listEnemy[0].SetActiveRing(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (parent is PlayerController)
        {
            if (other.tag == "Building")
            {
                ChangeAlpha(other.GetComponent<Renderer>().material, 0.2f);
            }
            if (listEnemy.Count > 0 && listEnemy[0]!=null)
            {
                listEnemy[0].SetActiveRing(true);
            }
        }

        /// Loại bỏ những thằng đã bị giết khi ở trong phạm vi tấn công
        if (listEnemy.Count > 0 && (listEnemy[0] == null || listEnemy[0].isDeath))
        {
            listEnemy[0].SetActiveRing(false);
            listEnemy.Remove(listEnemy[0]);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterController enemy = other.GetComponent<CharacterController>();

        if (parent is PlayerController)
        {
            if (other.tag == "Building")
            {
                ChangeAlpha(other.GetComponent<Renderer>().material, 1f);
            }
            if (listEnemy.Count > 0 && listEnemy[0] != null)
            {
                listEnemy[0].SetActiveRing(false);
            }
        }

        if (enemy != null && listEnemy.Contains(enemy))
        {
            listEnemy.Remove(enemy);
        }
    }

    public void ClearAllTarget()
    {
        listEnemy.Clear();
    }    

    public GameObject FindTheTarget()
    {
        if (listEnemy.Count == 0)
        {
            return null;
        }
        else
        {
            return listEnemy[0].gameObject;
        }
    }

    private void ChangeAlpha(Material mat, float alpha)
    {
        Color curColor = mat.color;
        Color newColor = new Color(curColor.r, curColor.g, curColor.b, alpha);
        mat.SetColor("_Color", newColor);
    }
}
