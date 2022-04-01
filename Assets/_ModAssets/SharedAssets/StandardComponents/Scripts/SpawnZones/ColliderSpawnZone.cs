using System;
using System.Collections;
using System.Collections.Generic;
using MarblePhysics.Modding.Shared.Constants;
using MarblePhysics.Modding.Shared.Extensions;
using MarblePhysics.Modding.Shared.Player;
using UnityEngine;

namespace MarblePhysics.Modding
{
    public class ColliderSpawnZone : SpawnZone
    {
        [Serializable]
        private enum SpawnTreatment
        {
            RandomWithinBounds,
            Line
        }

        [SerializeField]
        private new Collider2D collider2D = default;

        [SerializeField]
        private SpawnTreatment spawnTreatment = default;


        private Vector3 nextPosition;
        private List<Collider2D> overlapBoxResults;

        public override void PlaceMarbles(params Marble[] marbles)
        {
            switch (spawnTreatment)
            {
                case SpawnTreatment.RandomWithinBounds:
                    foreach (Marble marble in marbles)
                    {
                        marble.Teleport(GetPositionWithinBounds(), false, true, true);
                    }

                    break;
                case SpawnTreatment.Line:
                    BoxCollider2D boxCollider2D = (BoxCollider2D) collider2D;
                    ContactFilter2D filter = new()
                    {
                        useLayerMask = false
                    };

                    boxCollider2D.OverlapCollider(filter, overlapBoxResults);

                    float maxY = -.5f;
                    foreach (Collider2D overlapBoxResult in overlapBoxResults)
                    {
                        if (overlapBoxResult.gameObject.TryGetComponent(out Marble collidingPlayer))
                        {
                            maxY = Mathf.Max(maxY, transform.InverseTransformPoint(collidingPlayer.transform.position).y);
                        }
                    }

                    nextPosition = transform.TransformPoint(new Vector2(0, maxY)) - (collider2D.transform.up * .5f);

                    foreach (Marble marble in marbles)
                    {
                        nextPosition += collider2D.transform.up * marble.Size.y;
                        marble.Teleport(nextPosition, false, true, true);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(spawnTreatment.ToString());
            }
        }

        public void SetCollider(Collider2D collider2D)
        {
            this.collider2D = collider2D;
        }

        private void Awake()
        {
            overlapBoxResults = new List<Collider2D>(300);
        }


        private Vector2 GetPositionWithinBounds()
        {
            switch (collider2D)
            {
                case BoxCollider2D box:
                    return box.RandomPositionWithinBounds();
                case CircleCollider2D circle:
                    return circle.RandomPositionWithinBounds();
                default:

                    Debug.LogError("Unhandled collider type: " + collider2D.GetType());
                    return transform.position;
            }
        }
    }
}