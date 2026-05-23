using System.Numerics;
using Engine;
using Raylib_cs;

class Player : Entity
{
    public Transform transform;
    private readonly Rigidbody _rigidbody;
    private readonly PlayerController _controller;

    public Player()
    {
        transform = AddComponent(new Transform(Vector2.Zero, Vector2.One * 32, zLayer: 20));

        _rigidbody = AddComponent(new Rigidbody()
        {
            Interpolation = RigidbodyInterpolation.Interpolate,
        });

        BoxCollider collider = AddComponent(new BoxCollider());

        _controller = AddComponent(new PlayerController(200, 600f));

        AddComponent(new RectRenderer(Color.Blue));
    }

    public override void Update(float delta)
    {
        base.Update(delta);

        if (!Active)
            return;

        int? sceneIndex = Scene?.Window?.Scenes.IndexOf(Scene);
        if (sceneIndex == null || !GameManager.RunActive || !GameManager.CanSubmitRunInScene(sceneIndex.Value))
            return;

        if (InputManager.IsKeyPressed(GameManager.SubmitRunKey))
            GameManager.EndRun();
    }

    public void ResetState(Vector2 position)
    {
        Active = true;
        transform.LocalPosition = position;
        _rigidbody.Velocity = Vector2.Zero;
        _controller.ResetState();
    }
}
