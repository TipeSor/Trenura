using System.Numerics;
using Engine;
using Raylib_cs;

public class SpriteRenderer : Component
{
    public string TextureName { get; set; }
    public Vector2 Size { get; set; }
    public Color Tint { get; set; }
    public int ZLayerOffset { get; set; }

    private Transform? _transform;

    public SpriteRenderer(string textureName, Vector2? size = null, Color? tint = null, int zLayerOffset = 0)
    {
        TextureName = textureName;
        Size = size ?? Vector2.One;
        Tint = tint ?? Color.White;
        ZLayerOffset = zLayerOffset;
    }

    public override void Load()
    {
        _transform = Entity?.GetComponent<Transform>();
    }

    public override void Draw()
    {
        if (_transform == null || !TextureManager.HasTexture(TextureName))
            return;

        Texture2D texture = TextureManager.GetTexture(TextureName);
        Vector2 size = Size * _transform.Scale;
        Rectangle destination = new(
            _transform.Position.X - (size.X * 0.5f),
            _transform.Position.Y - (size.Y * 0.5f),
            size.X,
            size.Y
        );

        Renderer.DrawTexture(
            texture,
            new Rectangle(0, 0, texture.Width, texture.Height),
            destination,
            Vector2.Zero,
            _transform.Rotation,
            Tint,
            InstructionSource.Entity,
            _transform.ZLayer + ZLayerOffset
        );
    }
}
