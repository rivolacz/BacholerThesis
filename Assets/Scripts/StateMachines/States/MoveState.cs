using FunkyCode.SuperTilemapEditorSupport.Light.Shadow;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace Project.StateMachines.States
{
    public class MoveState : BaseState
    {
        private List<Vector3> wholePath = new List<Vector3>();
        public Vector3 TargetPosition { get; set; }

        private Vector3 movePartTarget;
        private List<Node> pathNodes = new();
        private int currentPathIndex = 0;
        private int currentNodeIndex = 0;
        private Transform unitTransform;
        private Transform attackTarget;
        public BaseState stateAfterMovingToTarget;
        private float offsetToTarget = 0;
        private bool usePathfinding = true;

        public MoveState(List<Vector3> pathPositions, StateMachine stateMachine) : base(stateMachine)
        {
            wholePath = pathPositions;
            TargetPosition = wholePath.First();
            unitTransform = stateMachine.transform;
        }

        public MoveState(List<Vector3> pathPositions, StateMachine stateMachine, Transform attackTarget) : base(stateMachine)
        {
            wholePath = pathPositions;
            TargetPosition = wholePath.First();
            unitTransform = stateMachine.transform;
            this.attackTarget = attackTarget;
        }

        public MoveState(List<Vector3> pathPositions, StateMachine stateMachine, float offsetToTarget, BaseState stateAfterMovingToTarget) : base(stateMachine)
        {
            wholePath = pathPositions;
            TargetPosition = wholePath.First();
            unitTransform = stateMachine.transform;
            this.offsetToTarget = offsetToTarget;
            this.stateAfterMovingToTarget = stateAfterMovingToTarget;
        }

        public MoveState(Vector3 pathPosition, StateMachine stateMachine, float offsetToTarget, BaseState stateAfterMovingToTarget) : base(stateMachine)
        {
            movePartTarget = pathPosition;
            wholePath.Add(pathPosition);
            TargetPosition = movePartTarget;
            unitTransform = stateMachine.transform;
            this.stateAfterMovingToTarget = stateAfterMovingToTarget;
            this.offsetToTarget = offsetToTarget;
        }

        public MoveState(Vector3 movePosition, StateMachine stateMachine, Transform attackTarget) : base(stateMachine)
        {
            wholePath.Clear();
            wholePath.Add(movePosition);
            TargetPosition = movePosition;
            unitTransform = stateMachine.transform;
            this.attackTarget = attackTarget;
        }

        public MoveState(Vector3 movePosition, StateMachine stateMachine) : base(stateMachine)
        {
            wholePath.Clear();
            wholePath.Add(movePosition);
            TargetPosition = movePosition;
            unitTransform = stateMachine.transform;
        }

        public MoveState(Vector3 movePosition,bool usePathfinding, StateMachine stateMachine) : base(stateMachine)
        {
            wholePath.Clear();
            wholePath.Add(movePosition);
            TargetPosition = movePosition;
            unitTransform = stateMachine.transform;
            this.usePathfinding = usePathfinding;
        }

        public override void Enter()
        {
            Debug.Log("Move state" + Vector3.Distance(TargetPosition, unitTransform.position), stateMachine.gameObject);
            if(Vector3.Distance(TargetPosition, unitTransform.position) < 3)
            {
                movePartTarget = TargetPosition;
                return;
            }
            pathNodes = PathFinding.FindPath(unitTransform.position, TargetPosition);
            if (pathNodes == null || pathNodes.Count < 1)
            {
                Debug.Log("Moving straight to target");
                movePartTarget = TargetPosition;
            }
            else
            {
                Debug.Log("Using pathfinding " + pathNodes.Count);
                movePartTarget = pathNodes[currentNodeIndex].position;
            }
        }

        public override void StateUpdate()
        {
            Vector2 directionToTarget = GetDirectionToTarget();
#if UNITY_EDITOR
            Debug.DrawLine(stateMachine.transform.position, stateMachine.transform.position + (Vector3)directionToTarget, UnityEngine.Color.green, 0.1f);
#endif
            Vector3 currentPosition = stateMachine.transform.position;
            float distance = Vector3.Distance(TargetPosition ,currentPosition);
            if(distance < 2 && stateAfterMovingToTarget != null)
            {
                Debug.Log("Next state is " + stateAfterMovingToTarget.GetType());
                stateMachine.ChangeState(stateAfterMovingToTarget);
            }
            if (IsClosetToAttackTarget())
            {
                stateMachine.ChangeState(new AttackState(attackTarget, stateMachine));
            }
            if (directionToTarget.magnitude < 1f + offsetToTarget)
            {
                if (IsAtEndOfPath())
                {
                    Debug.Log("Is at end");
                    if(stateAfterMovingToTarget != null)
                    {
                        Debug.Log("Next state is " + stateAfterMovingToTarget.GetType());
                        stateMachine.ChangeState(stateAfterMovingToTarget);
                    }
                    else if (attackTarget != null)
                    {
                        stateMachine.ChangeState(new AttackState(attackTarget, stateMachine));
                    }
                    else
                    {
                        stateMachine.ChangeState(new IdleState(stateMachine));
                    }
                }
                if (IsAtTarget())
                {
                    ChangePathSegment();
                }
                else
                {
                    IncreaseNodeIndex();
                }
            }
            stateMachine.MoveUnit(directionToTarget.normalized);
        }

        private bool IsClosetToAttackTarget()
        {
            if (attackTarget == null) return false;
            Collider2D collider = attackTarget.GetComponent<Collider2D>();
            Vector3 closestPointOnCollider = collider.bounds.ClosestPoint(stateMachine.transform.position);
            float distance = Vector3.Distance(stateMachine.transform.position, closestPointOnCollider);
#if UNITY_EDITOR
            Debug.DrawLine(stateMachine.transform.position, closestPointOnCollider, UnityEngine.Color.green, 0.1f);
#endif
            return distance < 1f;
        }

        private Vector2 GetDirectionToTarget()
        {
            Vector2 direction = movePartTarget - stateMachine.transform.position;
            return direction;
        }

        private bool IsAtTarget()
        {
            if (pathNodes == null) return true;
            return currentNodeIndex + 1 == pathNodes?.Count;
        }

        private bool IsAtEndOfPath()
        {
            //Vector3 endPosition = wholePath.Last();
            Vector3 currentPosition = stateMachine.transform.position;
            float distance = (TargetPosition - currentPosition).magnitude;
            return distance < 1f + offsetToTarget;
        }

        private void IncreaseNodeIndex()
        {
            currentNodeIndex++;
            if (currentNodeIndex < pathNodes?.Count)
            {
                movePartTarget = pathNodes[currentNodeIndex].position;
            }
        }

        private void ChangePathSegment()
        {
            currentPathIndex++;
            if (currentPathIndex >= wholePath.Count) return;
            TargetPosition = wholePath[currentPathIndex];
            if (currentPathIndex + 1 < wholePath.Count) {
                pathNodes = PathFinding.FindPath(unitTransform.position, TargetPosition);
            }
            else
            {
                movePartTarget = TargetPosition;
                pathNodes.Clear();
            }
            currentNodeIndex = 0;
            if (pathNodes == null || pathNodes.Count == 0) return;
            var node = pathNodes[currentNodeIndex];
            movePartTarget = (Vector3)(node?.position);
        }
    }
}
