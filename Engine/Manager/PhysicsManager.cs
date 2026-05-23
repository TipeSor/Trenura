using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine;

public static class PhysicsManager
{
    private static readonly Vector2 Gravity = new(0, 980.0f);
    private const float GroundCheckDistance = 1.0f;
    private static readonly Dictionary<Scene, HashSet<TriggerPair>> TriggerPairsByScene = [];

    public static void Update(Scene scene, float delta)
    {
        if (delta <= 0)
            return;

        var colliders = new List<Collider>();
        foreach (var entity in scene.Entities)
        {
            if (!ShouldSimulateEntity(scene, entity))
                continue;

            CollectColliders(entity, colliders);
        }

        for (int i = 0; i < colliders.Count; i++)
        {
            Collider collider = colliders[i];
            Rigidbody? rigidbody = collider.AttachedRigidbody;
            Transform? transform = collider.Transform;

            if (rigidbody == null || transform == null || !rigidbody.Simulated || rigidbody.BodyKind == RigidbodyType.Static)
                continue;

            if (rigidbody.BodyKind == RigidbodyType.Dynamic)
                rigidbody.Velocity += Gravity * rigidbody.GravityScale * delta;

            ApplyConstraintVelocity(rigidbody);

            float deltaX = rigidbody.Velocity.X * delta;
            if (!IsAxisFrozen(rigidbody, Axis.X) && deltaX != 0)
            {
                transform.LocalPosition += new Vector2(deltaX, 0);
                ResolveAxisCollisions(colliders, collider, rigidbody, Axis.X);
            }

            float deltaY = rigidbody.Velocity.Y * delta;
            if (!IsAxisFrozen(rigidbody, Axis.Y) && deltaY != 0)
            {
                transform.LocalPosition += new Vector2(0, deltaY);
                ResolveAxisCollisions(colliders, collider, rigidbody, Axis.Y);
            }

            ApplyConstraintVelocity(rigidbody);
        }

        ResolveTriggerCallbacks(scene, colliders);
    }

    /// <summary>
    /// Check if collider is standing on another solid collider
    /// </summary>
    /// <param name="collider">Collider to check</param>
    /// <returns>If collider is grounded</returns>
    public static bool IsGrounded(Collider collider)
    {
        if (collider.Entity?.Scene == null || !collider.CanParticipateInPhysics)
            return false;

        PhysicsAabb bounds = collider.GetWorldBounds();
        if (bounds.IsEmpty)
            return false;

        PhysicsAabb groundedBounds = new(
            new Vector2(bounds.Min.X, bounds.Max.Y),
            new Vector2(bounds.Max.X, bounds.Max.Y + GroundCheckDistance)
        );

        var colliders = new List<Collider>();
        foreach (var entity in collider.Entity.Scene.Entities)
        {
            if (!ShouldSimulateEntity(collider.Entity.Scene, entity))
                continue;

            CollectColliders(entity, colliders);
        }

        for (int i = 0; i < colliders.Count; i++)
        {
            Collider otherCollider = colliders[i];
            if (ReferenceEquals(otherCollider, collider) || ReferenceEquals(otherCollider.Entity, collider.Entity))
                continue;

            if (!otherCollider.CanParticipateInPhysics || otherCollider.IsTrigger)
                continue;

            PhysicsAabb otherBounds = otherCollider.GetWorldBounds();
            if (groundedBounds.Intersects(otherBounds))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Move collider using the physics collision solver without a rigidbody
    /// </summary>
    /// <param name="collider">Collider to move</param>
    /// <param name="delta">Movement delta</param>
    /// <returns>Result of the movement</returns>
    public static PhysicsMoveResult Move(Collider collider, Vector2 delta)
    {
        if (collider.Entity?.Scene == null || collider.Transform == null || !collider.CanParticipateInPhysics)
            return PhysicsMoveResult.None;

        var colliders = new List<Collider>();
        foreach (var entity in collider.Entity.Scene.Entities)
        {
            if (!ShouldSimulateEntity(collider.Entity.Scene, entity))
                continue;

            CollectColliders(entity, colliders);
        }

        PhysicsMoveResult result = PhysicsMoveResult.None;

        if (delta.X != 0)
        {
            collider.Transform.LocalPosition += new Vector2(delta.X, 0);
            if (ResolveKinematicAxisCollisions(colliders, collider, Axis.X))
                result |= delta.X > 0 ? PhysicsMoveResult.Right : PhysicsMoveResult.Left;
        }

        if (delta.Y != 0)
        {
            collider.Transform.LocalPosition += new Vector2(0, delta.Y);
            if (ResolveKinematicAxisCollisions(colliders, collider, Axis.Y))
                result |= delta.Y > 0 ? PhysicsMoveResult.Down : PhysicsMoveResult.Up;
        }

        return result;
    }

    private static void CollectColliders(Entity entity, List<Collider> colliders)
    {
        if (!entity.ActiveInHierarchy)
            return;

        foreach (var collider in entity.GetComponents<Collider>())
            if (collider.CanParticipateInPhysics)
                colliders.Add(collider);

        foreach (var child in entity.Children)
            CollectColliders(child, colliders);
    }

    private static bool ShouldSimulateEntity(Scene scene, Entity entity)
    {
        return entity.ActiveInHierarchy
            && (
                entity.PauseState is PauseState.Enabled
                || !scene.Paused && entity.PauseState is PauseState.Normal
                || scene.Paused && entity.PauseState is PauseState.WhenPaused
            )
            ;
    }

    private static void ResolveTriggerCallbacks(Scene scene, List<Collider> colliders)
    {
        if (!TriggerPairsByScene.TryGetValue(scene, out HashSet<TriggerPair>? previousPairs))
            previousPairs = [];

        var currentPairs = new HashSet<TriggerPair>();

        for (int i = 0; i < colliders.Count; i++)
        {
            Collider firstCollider = colliders[i];
            for (int j = i + 1; j < colliders.Count; j++)
            {
                Collider secondCollider = colliders[j];
                if (ReferenceEquals(firstCollider.Entity, secondCollider.Entity))
                    continue;

                if (!IsTriggerPair(firstCollider, secondCollider))
                    continue;

                if (!firstCollider.GetWorldBounds().Intersects(secondCollider.GetWorldBounds()))
                    continue;

                var pair = new TriggerPair(firstCollider, secondCollider);
                currentPairs.Add(pair);

                if (previousPairs.Contains(pair))
                    InvokeTriggerStay(pair);
                else
                    InvokeTriggerEnter(pair);
            }
        }

        foreach (TriggerPair pair in previousPairs)
            if (!currentPairs.Contains(pair))
                InvokeTriggerExit(pair);

        TriggerPairsByScene[scene] = currentPairs;
    }

    private static void ResolveAxisCollisions(List<Collider> colliders, Collider movingCollider, Rigidbody movingBody, Axis axis)
    {
        Transform? movingTransform = movingCollider.Transform;
        if (movingTransform == null)
            return;

        for (int i = 0; i < colliders.Count; i++)
        {
            Collider otherCollider = colliders[i];
            if (ReferenceEquals(otherCollider, movingCollider) || ReferenceEquals(otherCollider.Entity, movingCollider.Entity))
                continue;

            PhysicsAabb movingBounds = movingCollider.GetWorldBounds();
            PhysicsAabb otherBounds = otherCollider.GetWorldBounds();
            if (!movingBounds.Intersects(otherBounds))
                continue;

            bool isTrigger = movingCollider.IsTrigger || otherCollider.IsTrigger;

            if (isTrigger)
                continue;

            float overlap = GetAxisOverlap(movingBounds, otherBounds, axis);
            if (overlap <= 0)
                continue;

            float direction = axis == Axis.X
                ? movingBounds.Min.X < otherBounds.Min.X ? -1.0f : 1.0f
                : movingBounds.Min.Y < otherBounds.Min.Y ? -1.0f : 1.0f;

            Rigidbody? otherBody = otherCollider.AttachedRigidbody;
            float movingShare = GetMovingShare(movingBody, otherBody, axis);
            float otherShare = 1.0f - movingShare;

            if (movingShare > 0)
                movingTransform.LocalPosition += GetAxisVector(axis, direction * overlap * movingShare);

            if (otherShare > 0 && otherCollider.Transform != null)
                otherCollider.Transform.LocalPosition -= GetAxisVector(axis, direction * overlap * otherShare);

            ZeroAxisVelocity(movingBody, axis);

            if (otherShare > 0 && otherBody != null)
                ZeroAxisVelocity(otherBody, axis);
        }
    }

    private static bool ResolveKinematicAxisCollisions(List<Collider> colliders, Collider movingCollider, Axis axis)
    {
        Transform? movingTransform = movingCollider.Transform;
        if (movingTransform == null)
            return false;

        bool collided = false;

        for (int i = 0; i < colliders.Count; i++)
        {
            Collider otherCollider = colliders[i];
            if (ReferenceEquals(otherCollider, movingCollider) || ReferenceEquals(otherCollider.Entity, movingCollider.Entity))
                continue;

            if (!otherCollider.CanParticipateInPhysics || otherCollider.IsTrigger)
                continue;

            PhysicsAabb movingBounds = movingCollider.GetWorldBounds();
            PhysicsAabb otherBounds = otherCollider.GetWorldBounds();
            if (!movingBounds.Intersects(otherBounds))
                continue;

            float overlap = GetAxisOverlap(movingBounds, otherBounds, axis);
            if (overlap <= 0)
                continue;

            float direction = axis == Axis.X
                ? movingBounds.Min.X < otherBounds.Min.X ? -1.0f : 1.0f
                : movingBounds.Min.Y < otherBounds.Min.Y ? -1.0f : 1.0f;

            movingTransform.LocalPosition += GetAxisVector(axis, direction * overlap);
            collided = true;
        }

        return collided;
    }

    private static bool IsTriggerPair(Collider firstCollider, Collider secondCollider)
    {
        return IsTriggerCollider(firstCollider) || IsTriggerCollider(secondCollider);
    }

    private static void InvokeTriggerEnter(TriggerPair pair)
    {
        if (IsTriggerCollider(pair.First))
            pair.First.InvokeOnEnter(pair.Second);

        if (IsTriggerCollider(pair.Second))
            pair.Second.InvokeOnEnter(pair.First);
    }

    private static void InvokeTriggerStay(TriggerPair pair)
    {
        if (IsTriggerCollider(pair.First))
            pair.First.InvokeOnStay(pair.Second);

        if (IsTriggerCollider(pair.Second))
            pair.Second.InvokeOnStay(pair.First);
    }

    private static void InvokeTriggerExit(TriggerPair pair)
    {
        if (IsTriggerCollider(pair.First))
            pair.First.InvokeOnExit(pair.Second);

        if (IsTriggerCollider(pair.Second))
            pair.Second.InvokeOnExit(pair.First);
    }

    private static bool IsTriggerCollider(Collider collider)
    {
        return collider.IsTrigger;
    }

    private static float GetMovingShare(Rigidbody movingBody, Rigidbody? otherBody, Axis axis)
    {
        bool movingCanMove = CanMoveOnAxis(movingBody, axis);
        bool otherCanMove = otherBody != null && CanMoveOnAxis(otherBody, axis);

        if (!movingCanMove)
            return 0;

        if (!otherCanMove)
            return 1.0f;

        if (movingBody.BodyKind == RigidbodyType.Dynamic && otherBody!.BodyKind == RigidbodyType.Dynamic)
        {
            float movingInverseMass = GetInverseMass(movingBody);
            float otherInverseMass = GetInverseMass(otherBody);
            float totalInverseMass = movingInverseMass + otherInverseMass;

            if (totalInverseMass <= 0)
                return 1.0f;

            return movingInverseMass / totalInverseMass;
        }

        return 1.0f;
    }

    private static bool CanMoveOnAxis(Rigidbody rigidbody, Axis axis)
    {
        return rigidbody.Simulated
            && rigidbody.BodyKind != RigidbodyType.Static
            && !IsAxisFrozen(rigidbody, axis);
    }

    private static float GetInverseMass(Rigidbody rigidbody)
    {
        return rigidbody.BodyKind == RigidbodyType.Dynamic && rigidbody.Mass > 0
            ? 1.0f / rigidbody.Mass
            : 0.0f;
    }

    private static float GetAxisOverlap(PhysicsAabb first, PhysicsAabb second, Axis axis)
    {
        return axis == Axis.X
            ? MathF.Min(first.Max.X, second.Max.X) - MathF.Max(first.Min.X, second.Min.X)
            : MathF.Min(first.Max.Y, second.Max.Y) - MathF.Max(first.Min.Y, second.Min.Y);
    }

    private static Vector2 GetAxisVector(Axis axis, float value)
    {
        return axis == Axis.X
            ? new Vector2(value, 0)
            : new Vector2(0, value);
    }

    private static void ApplyConstraintVelocity(Rigidbody rigidbody)
    {
        Vector2 velocity = rigidbody.Velocity;

        if (rigidbody.Constraints.HasFlag(RigidbodyConstraint.FreezePositionX))
            velocity.X = 0;

        if (rigidbody.Constraints.HasFlag(RigidbodyConstraint.FreezePositionY))
            velocity.Y = 0;

        rigidbody.Velocity = velocity;
    }

    private static void ZeroAxisVelocity(Rigidbody rigidbody, Axis axis)
    {
        rigidbody.Velocity = axis == Axis.X
            ? new Vector2(0, rigidbody.Velocity.Y)
            : new Vector2(rigidbody.Velocity.X, 0);
    }

    private static bool IsAxisFrozen(Rigidbody rigidbody, Axis axis)
    {
        return axis == Axis.X
            ? rigidbody.Constraints.HasFlag(RigidbodyConstraint.FreezePositionX)
            : rigidbody.Constraints.HasFlag(RigidbodyConstraint.FreezePositionY);
    }

    private enum Axis
    {
        X,
        Y,
    }
}

internal readonly record struct TriggerPair(Collider First, Collider Second)
{
    public bool Equals(TriggerPair other)
    {
        return ReferenceEquals(First, other.First) && ReferenceEquals(Second, other.Second)
            || ReferenceEquals(First, other.Second) && ReferenceEquals(Second, other.First);
    }

    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(First) ^ RuntimeHelpers.GetHashCode(Second);
    }
}

[Flags]
public enum PhysicsMoveResult
{
    None  = 0,
    Left  = 1,
    Right = 2,
    Up    = 4,
    Down  = 8,
}
