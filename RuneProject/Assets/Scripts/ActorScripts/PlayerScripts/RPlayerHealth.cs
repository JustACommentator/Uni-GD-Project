using UnityEngine;

namespace RuneProject.ActorSystem
{
    public class RPlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 10;
     
        private int currentHealth = 0;
        public bool isAlive = false;

        public int MaxHealth { get => maxHealth; set => maxHealth = value; }

        public event System.EventHandler<int> OnDamageTaken;
        public event System.EventHandler<int> OnHealReceived;
        public event System.EventHandler OnDeath;
        
        private void Start()
        {
            isAlive = true;
            currentHealth = maxHealth;
        }

        public void TakeDamage(GameObject source, int damage)
        {
            if (!isAlive)
                return;
            this.currentHealth -= damage;
            if (this.currentHealth < 1)
            {
                Die(source);
            }
            OnDamageTaken?.Invoke(this, damage);
        }

        public void Heal(GameObject source, int heal)
        {
            if (!isAlive)
                return;
            this.currentHealth += heal;
            currentHealth = Mathf.Min(maxHealth, currentHealth);
            OnHealReceived?.Invoke(this, heal);
        }

        private void Die(GameObject source)
        {
            currentHealth = 0;
            isAlive = false;
            OnDeath?.Invoke(this, null);
        }
    }
}