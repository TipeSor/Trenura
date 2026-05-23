using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Component that controls a player using platformer-style movement
/// </summary>
public class PlayerController : Component
{
    private readonly Dictionary<ControlKey, KeyboardKey> _keys;

    private Transform? _transform;
    private Rigidbody? _rigidbody;
    private Collider? _collider;
    private float _verticalVelocity;
    private float _coyoteTimer;
    private float _jumpBufferTimer;
    private bool _jumpHeld;
    private bool _jumpWasHeld;
    private bool _jumpConsumed;

    /// <summary>
    /// Horizontal movement speed
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// Upward jump force
    /// </summary>
    public float JumpForce { get; set; }

    /// <summary>
    /// Gravity used when no rigidbody is attached
    /// </summary>
    public float Gravity { get; set; }

    /// <summary>
    /// Time after leaving ground where jump is still allowed
    /// </summary>
    public float CoyoteTime { get; set; }

    /// <summary>
    /// Time a jump press is buffered before landing
    /// </summary>
    public float JumpBufferTime { get; set; }

    /// <summary>
    /// Multiplier applied when jump is released early
    /// </summary>
    public float JumpCutMultiplier { get; set; }

    /// <summary>
    /// Check if the player is moving horizontally
    /// </summary>
    public bool IsMoving { get; protected set; }

    /// <summary>
    /// Horizontal input direction
    /// </summary>
    public Vector2 Direction { get; protected set; }

    /// <summary>
    /// Check if the player is on the ground
    /// </summary>
    public bool IsGrounded { get; protected set; }

    /// <summary>
    /// Create PlayerController
    /// </summary>
    /// <param name="speed">Horizontal speed</param>
    /// <param name="jumpForce">Jump force</param>
    public PlayerController(int speed)
        : this(speed, 425.0f) { }

    /// <summary>
    /// Create PlayerController
    /// </summary>
    /// <param name="speed">Horizontal speed</param>
    /// <param name="jumpForce">Jump force</param>
    public PlayerController(int speed, float jumpForce)
    {
        Speed = speed;
        JumpForce = jumpForce;
        Gravity = 980.0f;
        CoyoteTime = 0.1f;
        JumpBufferTime = 0.12f;
        JumpCutMultiplier = 0.5f;

        _keys = new()
        {
            { ControlKey.Up, KeyboardKey.W },
            { ControlKey.Down, KeyboardKey.S },
            { ControlKey.Left, KeyboardKey.A },
            { ControlKey.Right, KeyboardKey.D },
            { ControlKey.Jump, KeyboardKey.Space },
        };
    }

    /// <summary>
    /// Get bound key for control
    /// </summary>
    /// <param name="key">Control key</param>
    /// <returns>Keyboard key</returns>
    public KeyboardKey GetKey(ControlKey key) => _keys[key];

    /// <summary>
    /// Set bound key for control
    /// </summary>
    /// <param name="key">Control key</param>
    /// <param name="value">Keyboard key</param>
    public void SetKey(ControlKey key, KeyboardKey value) => _keys[key] = value;

    /// <summary>
    /// Load PlayerController
    /// </summary>
    public override void Load()
    {
        _transform = Entity?.GetComponent<Transform>();
        _rigidbody = Entity?.GetComponent<Rigidbody>();
        _collider = Entity?.GetComponent<Collider>();
    }

    /// <summary>
    /// Update PlayerController
    /// </summary>
    /// <param name="delta">Time since last frame</param>
    public override void Update(float delta)
    {
        base.Update(delta);

        if (_transform == null)
            return;

        float movement = GetHorizontalMovement();
        UpdateJumpInput(delta);

        IsMoving = movement != 0;
        Direction = movement == 0 ? Vector2.Zero : new Vector2(MathF.Sign(movement), 0);
        IsGrounded = _collider != null && PhysicsManager.IsGrounded(_collider);
        UpdateGroundState();

        if (_rigidbody != null && _rigidbody.Simulated)
        {
            Vector2 velocity = _rigidbody.Velocity;
            velocity.X = movement * Speed;

            if (CanJump())
            {
                velocity.Y = -JumpForce;
                ConsumeJump();
            }

            if (ShouldCutJump(velocity.Y))
                velocity.Y *= JumpCutMultiplier;

            _rigidbody.Velocity = velocity;
            return;
        }

        if (CanJump())
        {
            _verticalVelocity = -JumpForce;
            ConsumeJump();
        }
        else if (IsGrounded)
        {
            if (_verticalVelocity > 0)
                _verticalVelocity = 0;
        }
        else
            _verticalVelocity += Gravity * delta;

        if (ShouldCutJump(_verticalVelocity))
            _verticalVelocity *= JumpCutMultiplier;

        Vector2 movementDelta = new(movement * Speed * delta, _verticalVelocity * delta);

        if (_collider == null)
        {
            _transform.LocalPosition += movementDelta;
            return;
        }

        PhysicsMoveResult moveResult = PhysicsManager.Move(_collider, movementDelta);
        if (moveResult.HasFlag(PhysicsMoveResult.Down) || moveResult.HasFlag(PhysicsMoveResult.Up))
            _verticalVelocity = 0;
    }

    public void ResetState()
    {
        _verticalVelocity = 0;
        _coyoteTimer = 0;
        _jumpBufferTimer = 0;
        _jumpHeld = false;
        _jumpWasHeld = false;
        _jumpConsumed = false;
        IsMoving = false;
        Direction = Vector2.Zero;
        IsGrounded = false;
    }

    private float GetHorizontalMovement()
    {
        int direction = 0;

        if (InputManager.IsKeyDown(_keys[ControlKey.Left]))
            direction--;

        if (InputManager.IsKeyDown(_keys[ControlKey.Right]))
            direction++;

        return direction;
    }

    private bool IsJumpPressed()
    {
        return InputManager.IsKeyPressed(_keys[ControlKey.Jump])
            || InputManager.IsKeyPressed(_keys[ControlKey.Up]);
    }

    private bool IsJumpDown()
    {
        return InputManager.IsKeyDown(_keys[ControlKey.Jump])
            || InputManager.IsKeyDown(_keys[ControlKey.Up]);
    }

    private void UpdateJumpInput(float delta)
    {
        if (IsJumpPressed())
            _jumpBufferTimer = JumpBufferTime;
        else if (_jumpBufferTimer > 0)
            _jumpBufferTimer -= delta;

        if (!IsGrounded && _coyoteTimer > 0)
            _coyoteTimer -= delta;

        _jumpWasHeld = _jumpHeld;
        _jumpHeld = IsJumpDown();
    }

    private void UpdateGroundState()
    {
        if (IsGrounded)
        {
            _coyoteTimer = CoyoteTime;
            _jumpConsumed = false;
        }
    }

    private bool CanJump()
    {
        return _jumpBufferTimer > 0
            && _coyoteTimer > 0
            && !_jumpConsumed;
    }

    private void ConsumeJump()
    {
        _jumpBufferTimer = 0;
        _coyoteTimer = 0;
        _jumpConsumed = true;
    }

    private bool ShouldCutJump(float verticalVelocity)
    {
        return _jumpWasHeld
            && !_jumpHeld
            && verticalVelocity < 0;
    }
}

public enum ControlKey
{
    Up,
    Down,
    Left,
    Right,
    Jump,
}
