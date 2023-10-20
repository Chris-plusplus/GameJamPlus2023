using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(Rigidbody))]
    public class Liftable : Interactable
    {
        [field: SerializeField, ReadOnly] public bool IsLifted { get; private set; } = false;
        [field: SerializeField] public Vector3 LiftDirectionOffset { get; private set; } = Vector3.zero;

        public Rigidbody Rigidbody { get; protected set; }
        private readonly List<(GameObject, int)> defaultLayers = new();

        protected override void Awake()
        {
            base.Awake();
            Rigidbody = GetComponent<Rigidbody>();
        }

        public virtual void PickUp(int layer)
        {
            if (IsLifted)
                return;

            // save layers
            defaultLayers.Clear();
            foreach (Collider col in gameObject.GetComponentsInChildren<Collider>())
                defaultLayers.Add((col.gameObject, col.gameObject.layer));

            // set
            Rigidbody.useGravity = false;
            Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            foreach ((GameObject obj, int defaultLayer) item in defaultLayers)
                item.obj.layer = layer;

            IsLifted = true;
        }
        public virtual void Drop()
        {
            if (!IsLifted)
                return;

            Rigidbody.useGravity = true;
            Rigidbody.interpolation = RigidbodyInterpolation.None;
            foreach ((GameObject obj, int defaultLayer) item in defaultLayers)
                item.obj.layer = item.defaultLayer;

            IsLifted = false;
        }
    }
}