
using UnityEngine;

namespace RPG.Attributes
{
    public abstract class EnemyBar : MonoBehaviour, IBar
    {
        [SerializeField] protected RectTransform foreground = null;
        [SerializeField] protected Canvas canvas = null;
        [SerializeField] protected GameObject followTarget = null;
        [SerializeField, SerializeReference] protected IAttribute component = null;

        float y = 0;

        private void Awake()
        {
            y = transform.position.y;
        }

        protected void Follow()
        {
            Vector3 newPos = followTarget.transform.position;
            newPos.y = y;
            transform.position = newPos;
        }

        public void UpdateUI()
        {
            float fraction = component.GetFraction();
            if (foreground != null)
            {
                foreground.localScale = new Vector3(fraction, 1, 1);
                if (Mathf.Approximately(fraction, 0) || Mathf.Approximately(fraction, 1))
                {
                    canvas.enabled = false;
                    return;
                }
                else
                {
                    canvas.enabled = true;
                }
            }
        }
    }
}