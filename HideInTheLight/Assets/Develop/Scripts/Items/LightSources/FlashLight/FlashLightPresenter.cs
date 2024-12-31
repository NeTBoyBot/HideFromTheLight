using System;
using Develop.Scripts.Interfaces;
using UnityEngine;

namespace Develop.Scripts.Items.LightSources.FlashLight
{
    [RequireComponent(typeof(FlashLightModel),typeof(FlashLightView))]
    public class FlashLightPresenter : LightSourceBase, IDetector<FlashLightPresenter>
    {
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

        private void Awake()
        {
            _view = GetComponent<FlashLightView>();
            _model = GetComponent<FlashLightModel>();
        }

        public override void TurnOn()
        {
            IsOn = true;
            _view.TurnOn();
        }

        public override void TurnOff()
        {
            IsOn = false;
            _view.TurnOff();
        }

        public override void Interact()
        {
            if(IsOn)
                TurnOff();
            else
                TurnOn();
        }
        
        public FlashLightPresenter Detect()
        {
            Vector3 capsuleCenter = transform.position + transform.TransformDirection(transform.position + Center);
            float radius = Radius * Mathf.Max(transform.lossyScale.y, transform.lossyScale.z);
            float height = Mathf.Max(0, Height * transform.lossyScale.x - 2 * radius);

            Vector3 offset = transform.right * (height / 2); // Ориентируем вдоль оси X
            Vector3 start = capsuleCenter - offset;
            Vector3 end = capsuleCenter + offset;

            Collider[] cols = Physics.OverlapCapsule(start, end, radius);
            foreach (var col in cols)
            {
                if (col.gameObject != gameObject) // Исключаем саму себя
                {
                    if (col.gameObject.TryGetComponent(out FlashLightPresenter presenter))
                        return presenter;
                }
            }

            return null;
        }

        
        
        void OnDrawGizmos()
        {
            if (!_gizmos)
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
