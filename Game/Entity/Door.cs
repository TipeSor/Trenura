using System.Numerics;
using Engine;
using Raylib_cs;

public class Door : Entity
{
    private static readonly Random Random = new();

    private readonly Entity _targetEntity;
    private readonly int[] _sceneIndexes;

    public Transform Transform;

    public Door(Entity targetEntity, Vector2 position, int nextSceneIndex, Vector2? size = null)
        : this(targetEntity, position, [nextSceneIndex], size) { }

    public Door(Entity targetEntity, Vector2 position, IEnumerable<int> sceneIndexes, Vector2? size = null)
    {
        _targetEntity = targetEntity;
        _sceneIndexes = sceneIndexes.Distinct().ToArray();

        if (_sceneIndexes.Length == 0)
            throw new ArgumentException("Door requires at least one destination scene index.", nameof(sceneIndexes));

        Vector2 doorSize = size ?? new Vector2(28, 48);
        Transform = AddComponent(new Transform(position, doorSize, zLayer: 14));

        BoxCollider collider = AddComponent(new BoxCollider()
        {
            IsTrigger = true,
        });

        collider.OnEnter += other =>
        {
            if (other.Entity != _targetEntity || Scene?.Window == null)
                return;

            Scene.Window.CurrentSceneIndex = GetNextSceneIndex();
        };

        AddComponent(new RectRenderer(new Color(170, 116, 58, 255)));
    }

    private int GetNextSceneIndex()
    {
        if (_sceneIndexes.Length == 1)
            return _sceneIndexes[0];

        return _sceneIndexes[Random.Next(0, _sceneIndexes.Length)];
    }
}
