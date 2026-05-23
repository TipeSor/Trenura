using System.Numerics;
using Engine;
using Raylib_cs;

class Floor : Entity
{
    public Floor()
    {
        Transform transform = AddComponent(new Transform(new Vector2(0, 64), new Vector2(128, 16), zLayer: 13));

        BoxCollider collider = AddComponent(new BoxCollider());

        AddComponent(new RectRenderer(Color.Green));
    }
}
