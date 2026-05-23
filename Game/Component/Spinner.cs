using System.Numerics;
using Engine;
using Raylib_cs;

public class Spinner : Component
{
    public string TextureName { get; set; }
    public Vector2 FrameSize { get; set; }
    public int FrameCount { get; set; }
    public float FramesPerSecond { get; set; }
    public int FaceFrameStep { get; set; }
    public int FaceFrameOffset { get; set; }
    public bool Loop { get; set; }
    public Color Tint { get; set; }
    public int ZLayerOffset { get; set; }
    public int CurrentFrame { get; private set; }
    public int CurrentFaceIndex => GetFaceIndex(CurrentFrame);
    public float CurrentFramesPerSecond => _spinFramesPerSecond ?? FramesPerSecond;
    public bool Playing { get; private set; } = true;
    public bool SpinningToTarget { get; private set; }
    public bool HoldingAfterSpin { get; private set; }

    /// <summary>
    /// Event triggered when a targeted spin lands, giving raw frame index and logical face index
    /// </summary>
    public event Action<int, int>? OnLanded;

    private float _frameTimer;
    private float _holdTimer;
    private float _holdDuration;
    private float? _spinFramesPerSecond;
    private int _spinTargetFrame;
    private int _remainingSpinFrames;
    private bool _restartAfterHold;
    private Transform? _transform;

    public Spinner(
        string textureName,
        Vector2 frameSize,
        int frameCount,
        float framesPerSecond = 12,
        int faceFrameStep = 1,
        int faceFrameOffset = 0,
        bool loop = true,
        Color? tint = null,
        int zLayerOffset = 0)
    {
        TextureName = textureName;
        FrameSize = frameSize;
        FrameCount = frameCount;
        FramesPerSecond = framesPerSecond;
        FaceFrameStep = Math.Max(1, faceFrameStep);
        FaceFrameOffset = faceFrameOffset;
        Loop = loop;
        Tint = tint ?? Color.White;
        ZLayerOffset = zLayerOffset;
    }

    public override void Load()
    {
        _transform = Entity?.GetComponent<Transform>();
    }

    public override void Update(float delta)
    {
        if (FrameCount <= 1)
            return;

        if (SpinningToTarget)
        {
            UpdateFrameTimer(delta, true);
            return;
        }

        if (HoldingAfterSpin)
        {
            UpdateHold(delta);
            return;
        }

        if (!Playing || FramesPerSecond <= 0)
            return;

        UpdateFrameTimer(delta, false);
    }

    public override void Draw()
    {
        if (_transform == null || !TextureManager.HasTexture(TextureName))
            return;

        Texture2D texture = TextureManager.GetTexture(TextureName);
        Rectangle source = GetSourceRectangle(texture);
        Vector2 size = FrameSize * _transform.Scale;
        Rectangle destination = new(
            _transform.Position.X - (size.X * 0.5f),
            _transform.Position.Y - (size.Y * 0.5f),
            size.X,
            size.Y
        );

        Renderer.DrawTexture(
            texture,
            source,
            destination,
            Vector2.Zero,
            _transform.Rotation,
            Tint,
            InstructionSource.Entity,
            _transform.ZLayer + ZLayerOffset
        );
    }

    public void Reset()
    {
        CurrentFrame = 0;
        Playing = true;
        _frameTimer = 0;
        _holdTimer = 0;
        _holdDuration = 0;
        _spinFramesPerSecond = null;
        _remainingSpinFrames = 0;
        _restartAfterHold = false;
        SpinningToTarget = false;
        HoldingAfterSpin = false;
    }

    public void Play()
    {
        Playing = true;
        SpinningToTarget = false;
        HoldingAfterSpin = false;
        _spinFramesPerSecond = null;
    }

    public void Stop()
    {
        Playing = false;
        SpinningToTarget = false;
        HoldingAfterSpin = false;
        _frameTimer = 0;
        _holdTimer = 0;
        _spinFramesPerSecond = null;
    }

    public void SetFrame(int frame)
    {
        CurrentFrame = NormalizeFrame(frame);
        _frameTimer = 0;
    }

    public void SpinToFrame(
        int targetFrame,
        int extraLoops = 2,
        float holdTime = 0,
        bool restartAfterHold = false,
        float? spinFramesPerSecond = null)
    {
        if (FrameCount <= 0)
            return;

        _spinTargetFrame = NormalizeFrame(targetFrame);
        _frameTimer = 0;
        _holdTimer = 0;
        _holdDuration = Math.Max(0, holdTime);
        _spinFramesPerSecond = spinFramesPerSecond;
        _restartAfterHold = restartAfterHold;
        Playing = true;
        SpinningToTarget = true;
        HoldingAfterSpin = false;

        int forwardDistance = _spinTargetFrame - CurrentFrame;
        if (forwardDistance < 0)
            forwardDistance += FrameCount;

        _remainingSpinFrames = forwardDistance + (Math.Max(0, extraLoops) * FrameCount);
        if (_remainingSpinFrames <= 0)
            _remainingSpinFrames = FrameCount;
    }

    public void SpinToFace(
        int faceIndex,
        int extraLoops = 0,
        float holdTime = 0,
        bool restartAfterHold = false,
        float? spinFramesPerSecond = null)
    {
        SpinToFace(faceIndex, FaceFrameStep, FaceFrameOffset, extraLoops, holdTime, restartAfterHold, spinFramesPerSecond);
    }

    public void SpinToFace(
        int faceIndex,
        int faceFrameStep,
        int faceFrameOffset,
        int extraLoops = 2,
        float holdTime = 0,
        bool restartAfterHold = false,
        float? spinFramesPerSecond = null)
    {
        int step = Math.Max(1, faceFrameStep);
        SpinToFrame(
            faceFrameOffset + (faceIndex * step),
            extraLoops,
            holdTime,
            restartAfterHold,
            spinFramesPerSecond
        );
    }

    private void AdvanceFrame()
    {
        if (CurrentFrame < FrameCount - 1)
        {
            CurrentFrame++;
            return;
        }

        if (Loop)
            CurrentFrame = 0;
    }

    private void UpdateFrameTimer(float delta, bool targetedSpin)
    {
        float fps = targetedSpin ? (_spinFramesPerSecond ?? FramesPerSecond) : FramesPerSecond;
        if (fps <= 0)
            return;

        _frameTimer += delta;
        float frameDuration = 1f / fps;

        while (_frameTimer >= frameDuration)
        {
            _frameTimer -= frameDuration;

            if (targetedSpin)
            {
                AdvanceSpinFrame();
                continue;
            }

            AdvanceFrame();
        }
    }

    private void AdvanceSpinFrame()
    {
        if (_remainingSpinFrames <= 0)
        {
            FinishSpin();
            return;
        }

        AdvanceFrame();
        _remainingSpinFrames--;

        if (_remainingSpinFrames <= 0)
            FinishSpin();
    }

    private void FinishSpin()
    {
        CurrentFrame = _spinTargetFrame;
        _remainingSpinFrames = 0;
        SpinningToTarget = false;
        HoldingAfterSpin = _holdDuration > 0;
        Playing = _restartAfterHold && !HoldingAfterSpin;
        OnLanded?.Invoke(CurrentFrame, GetFaceIndex(CurrentFrame));

        if (!HoldingAfterSpin)
            _spinFramesPerSecond = null;

        if (!HoldingAfterSpin && !_restartAfterHold)
            Playing = false;
    }

    private void UpdateHold(float delta)
    {
        _holdTimer += delta;
        if (_holdTimer < _holdDuration)
            return;

        HoldingAfterSpin = false;
        _holdTimer = 0;

        if (_restartAfterHold)
        {
            _spinFramesPerSecond = null;
            Playing = true;
            return;
        }

        _spinFramesPerSecond = null;
        Playing = false;
    }

    private int NormalizeFrame(int frame)
    {
        if (FrameCount <= 0)
            return 0;

        int normalized = frame % FrameCount;
        return normalized < 0 ? normalized + FrameCount : normalized;
    }

    private int GetFaceIndex(int frame)
    {
        int normalizedFrame = NormalizeFrame(frame);
        int normalizedOffset = NormalizeFrame(FaceFrameOffset);
        int relativeFrame = normalizedFrame - normalizedOffset;

        if (relativeFrame < 0)
            relativeFrame += FrameCount;

        return relativeFrame / Math.Max(1, FaceFrameStep);
    }

    private Rectangle GetSourceRectangle(Texture2D texture)
    {
        int columns = Math.Max(1, texture.Width / (int)FrameSize.X);
        int frameX = CurrentFrame % columns;
        int frameY = CurrentFrame / columns;

        return new Rectangle(
            frameX * FrameSize.X,
            frameY * FrameSize.Y,
            FrameSize.X,
            FrameSize.Y
        );
    }
}
