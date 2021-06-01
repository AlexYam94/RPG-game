
using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG.Attributes
{
    public abstract class EnemyBar : SerializedMonoBehaviour, IBar
    {
        [SerializeField] protected RectTransform foreground = null;
        [SerializeField] protected Canvas canvas = null;
        [SerializeField] protected GameObject followTarget = null;
        [SerializeField] protected IAttribute component;

        float y = 0;

        private void Awake()
        {
            y = transform.position.y;
            component = GetAttriute();
        }

        protected abstract IAttribute GetAttriute();

        protected void Follow()
        {
            Vector3 newPos = followTarget.transform.position;
            newPos.y = y;
            transform.position = newPos;
        }

        public void UpdateUI()
        {
            if(!component.isEnable()){
                SetCanvas(false);
                this.enabled=false;
            }
            float fraction = component.GetFraction();
            if (foreground != null)
            {
                foreground.localScale = new Vector3(fraction, 1, 1);
                if (Mathf.Approximately(fraction, 0) || Mathf.Approximately(fraction, 1))
                {
                    SetCanvas(false);
                }
                else
                {
                    SetCanvas(true);
                }
            }
        }

        protected void SetCanvas(bool value){
            canvas.enabled = value;
        }
    }
}