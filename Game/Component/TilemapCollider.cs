using System.Numerics;
using Engine;

class TilemapCollider : Component
{
    private readonly List<Entity> _colliderEntities = [];
    private Tilemap? _tilemap;

    private bool _loaded;

    public override void Load()
    {
        _tilemap = Entity?.GetComponent<Tilemap>();
        _loaded = true;
        Rebuild();
    }

    public override void Unload()
    {
        ClearColliders();
        _loaded = false;
    }

    public void Rebuild()
    {
        if (_tilemap == null || Entity == null)
            return;

        ClearColliders();

        bool[,] visited = new bool[_tilemap.Cells.GetLength(0), _tilemap.Cells.GetLength(1)];

        for (int y = 0; y < _tilemap.Cells.GetLength(0); y++)
        {
            for (int x = 0; x < _tilemap.Cells.GetLength(1); x++)
            {
                if (_tilemap.Cells[y, x] <= 0 || visited[y, x])
                    continue;

                int width = GetRectWidth(x, y, visited);
                int height = GetRectHeight(x, y, width, visited);

                MarkVisited(x, y, width, height, visited);

                Entity colliderEntity = CreateColliderEntity(x, y, width, height);
                Entity.AddChild(colliderEntity);
                _colliderEntities.Add(colliderEntity);

                if (_loaded)
                    colliderEntity.Load();
            }
        }
    }

    private void ClearColliders()
    {
        for (int i = 0; i < _colliderEntities.Count; i++)
        {
            Entity colliderEntity = _colliderEntities[i];

            if (_loaded)
                colliderEntity.Unload();

            Entity?.RemoveChild(colliderEntity);
        }

        _colliderEntities.Clear();
    }

    private int GetRectWidth(int startX, int startY, bool[,] visited)
    {
        if (_tilemap == null)
            throw new InvalidOperationException("TilemapCollider requires a Tilemap component.");

        int width = 0;

        for (int x = startX; x < _tilemap.Cells.GetLength(1); x++)
        {
            if (_tilemap.Cells[startY, x] <= 0 || visited[startY, x])
                break;

            width++;
        }

        return width;
    }

    private int GetRectHeight(int startX, int startY, int width, bool[,] visited)
    {
        if (_tilemap == null)
            throw new InvalidOperationException("TilemapCollider requires a Tilemap component.");

        int height = 0;

        for (int y = startY; y < _tilemap.Cells.GetLength(0); y++)
        {
            bool canExpand = true;

            for (int x = startX; x < startX + width; x++)
            {
                if (_tilemap.Cells[y, x] <= 0 || visited[y, x])
                {
                    canExpand = false;
                    break;
                }
            }

            if (!canExpand)
                break;

            height++;
        }

        return height;
    }

    private static void MarkVisited(int startX, int startY, int width, int height, bool[,] visited)
    {
        for (int y = startY; y < startY + height; y++)
            for (int x = startX; x < startX + width; x++)
                visited[y, x] = true;
    }

    private Entity CreateColliderEntity(int x, int y, int width, int height)
    {
        if (_tilemap == null)
            throw new InvalidOperationException("TilemapCollider requires a Tilemap component.");

        Entity colliderEntity = new();
        Vector2 colliderSize = new(_tilemap.CellSize.X * width, _tilemap.CellSize.Y * height);
        Vector2 topLeft = _tilemap.GetTopLeft();

        colliderEntity.AddComponent(new Transform(
            new Vector2(
                topLeft.X + (x * _tilemap.CellSize.X) + (colliderSize.X * 0.5f),
                topLeft.Y + (y * _tilemap.CellSize.Y) + (colliderSize.Y * 0.5f)
            ),
            colliderSize
        ));
        colliderEntity.AddComponent(new BoxCollider());

        return colliderEntity;
    }
}
