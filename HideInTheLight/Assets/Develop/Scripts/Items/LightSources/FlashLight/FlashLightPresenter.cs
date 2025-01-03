using Develop.Scripts.Interfaces;
using Mirror;
using UnityEngine;

namespace Develop.Scripts.Items.LightSources.FlashLight
{
    [RequireComponent(typeof(FlashLightModel),typeof(FlashLightView))]
    public class FlashLightPresenter : LightSourceBase, IDetector<MonsterModel>,IInteractable
    {
        [field: SyncVar]
        public override bool IsOn { get; set; }

        [field:Space(1)]
        [field:Header("Input")]

        [field:SerializeField] public float Radius { get; set; }
        [field:SerializeField] public float Height { get; set; }
        [field:SerializeField] public Vector3 Center { get; set; }
        
        [Space(1)]
        [Header("Debug")]

        [SerializeField] private bool _gizmos;
        
        private FlashLightView _view;
        private FlashLightModel _model;

        #region Initialize
        public override void OnStartClient()
        {
            base.OnStartClient();

            Debug.Log("FLASHLIGHT START CLIENT");
            Initialize();

            SubscribeEvents();
        }

        private void Initialize()
        {
            _view = GetComponent<FlashLightView>();
            _model = GetComponent<FlashLightModel>();

            _model._lastDamageTime = 0f;
            _model._lastlossTime = 0f;
        }
        private void SubscribeEvents()
        {
            _model.OnOutOfCharge += CmdTurnOff;
        }
        #endregion

        [Command(requiresAuthority = false)]
        public override void CmdTurnOn()
        {
            IsOn = true;
            _view.RpcTurnOn();
        }

        [Command(requiresAuthority = false)]
        public override void CmdTurnOff()
        {
            IsOn = false;
            _view.RpcTurnOff();
        }

        private void FixedUpdate()
        {
            if (!isServer && authority)
            {
                CmdRequestDetection();
            }
            else if (isServer)
            {
                DetectAndDamage();
            }

            //НУЖНО ПРОВЕРИТЬ НЕ С ЛОКАЛЬНЫМИ БОЛВАНЧИКАМИ, А С РЕАЛЬНЫМИ ИГРОКАМИ
            //if (authority)
            //{
            //    CmdRequestDetection();
            //}
        }

        public override void Interact()
        {
            if (!_model.HasEnergy())
                return;

            if (IsOn)
                CmdTurnOff();
            else
                CmdTurnOn();
        }
        [Command(requiresAuthority = true)]
        public void CmdRequestDetection()
        {
            Debug.Log("CMD REQUEST DETECTION");
            DetectAndDamage();
        }

        [Server]
        private void DetectAndDamage()
        {
            if (!IsOn || !_model.HasEnergy() || !TryChargeLoss())
                return;

            if (Time.time - _model._lastDamageTime < _model.DamageCooldown)
                return;

            _model._lastDamageTime = Time.time;

            float radius = Radius * Mathf.Max(transform.lossyScale.y, transform.lossyScale.z);

            if (Physics.Raycast(transform.position, transform.forward, out var hit, radius))
            {
                Debug.Log(hit.collider ? $"HIT OBJECT = <color=yellow>{hit.collider.gameObject.name}</color>" : "No hit detected.");

                if (hit.collider.gameObject != gameObject)
                {
                    if(hit.collider.TryGetComponent(out MonsterModel monsterModel))
                    {
                        ApplyDamageToMonster(monsterModel);
                    }
                }
            }
        }

        [Server]
        private bool TryChargeLoss()
        {
            if (Time.time - _model._lastlossTime < _model.ChargeLossCooldown)
                return false;

            _model.ChangeChargeLevel(-_model.ChargeLoss);
            _model._lastlossTime = Time.time;

            return true;
        }

        [Server]
        private void ApplyDamageToMonster(MonsterModel monster)
        {
            if (monster == null || monster.GetHealth() <= 0)
                return;

            monster.ChangeHealth(-_model.LightDamage);

            RpcOnMonsterHit(monster);
        }

        [ClientRpc]
        private void RpcOnMonsterHit(MonsterModel monster)
        {
           //OnHitLogic
           PlayHitEffects(monster);
        }

        private void PlayHitEffects(MonsterModel monster)
        {
            //Add visual/audio effects here
            Debug.Log($"<color=yellow><b>Hit effects</b></color> played on {monster.gameObject.name}");
        }

        void OnDrawGizmos()
        {
            if (!_gizmos)
                return;

            Gizmos.DrawRay(transform.position,transform.forward*Radius);
            
            return;
            
            Vector3 capsuleCenter = transform.position + transform.TransformDirection(Center);

            // Радиус и высота капсулы с учётом масштаба
            float radius = Radius * Mathf.Max(transform.lossyScale.y, transform.lossyScale.z); // Масштаб по Y и Z для радиуса
            float height = Mathf.Max(0, Height * transform.lossyScale.x - 2 * radius); // Масштаб по X для высоты

            // Ориентация капсулы вдоль оси X
            Vector3 rightDirection = transform.right;

            // Вычисляем центры верхнего и нижнего шариков
            Vector3 leftSphereCenter = capsuleCenter - rightDirection * (height / 2);
            Vector3 rightSphereCenter = capsuleCenter + rightDirection * (height / 2);

            // Рисуем капсулу
            DrawCapsulePart(leftSphereCenter, rightSphereCenter, radius);
        }

        void DrawCapsulePart(Vector3 leftCenter, Vector3 rightCenter, float radius)
        {
            // Рисуем сферические концы
            Gizmos.DrawWireSphere(leftCenter, radius);
            Gizmos.DrawWireSphere(rightCenter, radius);

            // Количество сегментов для кругов
            int segments = 16;

            // Рисуем цилиндрическую часть и соединительные линии
            for (int i = 0; i < segments; i++)
            {
                float angle = i * Mathf.PI * 2 / segments;
                float nextAngle = (i + 1) * Mathf.PI * 2 / segments;

                // Вычисляем точки окружности
                Vector3 offset = new Vector3(0, Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
                Vector3 nextOffset = new Vector3(0, Mathf.Cos(nextAngle) * radius, Mathf.Sin(nextAngle) * radius);

                // Линии верхней и нижней окружности
                Gizmos.DrawLine(leftCenter + offset, leftCenter + nextOffset);
                Gizmos.DrawLine(rightCenter + offset, rightCenter + nextOffset);

                // Линии между верхним и нижним кругами
                Gizmos.DrawLine(leftCenter + offset, rightCenter + offset);
            }
        }
    }
}
