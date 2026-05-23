using System.Numerics;
using Engine;

public class CameraHandler : Entity
{
    private readonly Transform _target;
    private readonly Tilemap _tilemap;

    public CameraHandler(Transform target, Tilemap tilemap)
    {
        _target = target;
        _tilemap = tilemap;
    }

    public override void Update(float delta)
    {
        base.Update(delta);

        Vector2 targetPosition = _target.Position;
        Vector2 halfViewSize = CameraManager.HalfViewSize;
        Vector2 mapHalfSize = _tilemap.Size * 0.5f;
        Vector2 mapCenter = _tilemap.Transform?.Position ?? Vector2.Zero;

        float minX = mapCenter.X - mapHalfSize.X + halfViewSize.X;
        float maxX = mapCenter.X + mapHalfSize.X - halfViewSize.X;
        float minY = mapCenter.Y - mapHalfSize.Y + halfViewSize.Y;
        float maxY = mapCenter.Y + mapHalfSize.Y - halfViewSize.Y;

        float cameraX = minX > maxX
            ? mapCenter.X
            : Math.Clamp(targetPosition.X, minX, maxX);

        float cameraY = minY > maxY
            ? mapCenter.Y
            : Math.Clamp(targetPosition.Y, minY, maxY);

        CameraManager.Position = new Vector2(cameraX, cameraY);
    }
}
