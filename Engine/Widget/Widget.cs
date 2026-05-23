using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Abstract class which represents a UI widget
/// </summary>
public abstract class Widget
{
    /// <summary>
    /// Position of widget relative to parent or screen
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Z layer of widget
    /// </summary>
    public int ZLayer { get; set; }

    /// <summary>
    /// If widget is displayed
    /// </summary>
    public bool Displayed { get; set; } = true;

    /// <summary>
    /// If widget is active
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    /// If widget consumes mouse when hovered
    /// </summary>
    public bool ConsumesMouse { get; set; } = true;

    /// <summary>
    /// Parent widget
    /// </summary>
    public Widget? Parent { get; set; }

    /// <summary>
    /// Horizontal anchor used to interpret position relative to parent or screen
    /// </summary>
    public HorizontalAlignment HorizontalAnchor { get; set; } = HorizontalAlignment.Left;

    /// <summary>
    /// Vertical anchor used to interpret position relative to parent or screen
    /// </summary>
    public VerticalAlignment VerticalAnchor { get; set; } = VerticalAlignment.Top;

    /// <summary>
    /// Horizontal alignment used to interpret position
    /// </summary>
    public HorizontalAlignment HorizontalAlign { get; set; } = HorizontalAlignment.Center;

    /// <summary>
    /// Vertical alignment used to interpret position
    /// </summary>
    public VerticalAlignment VerticalAlign { get; set; } = VerticalAlignment.Center;

    /// <summary>
    /// How widget must be updated when paused
    /// </summary>
    public PauseState PauseState { get; set; } = PauseState.Normal;

    /// <summary>
    /// Name of widget
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Bounding box of widget in screen space
    /// </summary>
    public Rectangle Bounds { get; protected set; }

    /// <summary>
    /// Real position of widget including parent offset
    /// </summary>
    public Vector2 RealPosition => GetAnchorPoint() + Position;

    /// <summary>
    /// If widget is really displayed including parent state
    /// </summary>
    public bool RealDisplayed => Displayed && (Parent?.RealDisplayed ?? true);

    /// <summary>
    /// If widget is really active including parent state
    /// </summary>
    public bool RealActive => Active && (Parent?.RealActive ?? true);

    /// <summary>
    /// If widget is visible in window bounds
    /// </summary>
    public bool IsVisibleOnScreen
    {
        get
        {
            if (Scene?.Window == null)
                return true;

            Rectangle viewport = new(0, 0, Scene.Window.RenderSize.X, Scene.Window.RenderSize.Y);
            return Bounds.Width <= 0
                || Bounds.Height <= 0
                || Intersects(Bounds, viewport);
        }
    }

    /// <summary>
    /// Get all direct children of widget
    /// </summary>
    public List<Widget> Children { get; } = [];

    /// <summary>
    /// Scene of widget
    /// </summary>
    public Scene? Scene
    {
        get => _scene;
        set
        {
            _scene = value;
            foreach (var child in Children)
                child.Scene = value;
        }
    }

    private Scene? _scene;

    /// <summary>
    /// Create widget
    /// </summary>
    /// <param name="position">Widget position</param>
    /// <param name="zLayer">Widget z layer</param>
    protected Widget(Vector2 position, int zLayer = 0)
    {
        Position = position;
        ZLayer = zLayer;
        Bounds = new Rectangle(position.X, position.Y, 0, 0);
    }

    /// <summary>
    /// Get all direct children of one type
    /// </summary>
    /// <typeparam name="T">Type of child</typeparam>
    /// <returns>Children of type T</returns>
    public List<T> GetChildrenAs<T>()
        where T : Widget => Children.OfType<T>().ToList();

    /// <summary>
    /// Get all recursive children
    /// </summary>
    /// <returns>All children</returns>
    public List<Widget> GetAllChildren()
    {
        var children = new List<Widget>(Children);
        foreach (var child in Children)
            children.AddRange(child.GetAllChildren());
        return children;
    }

    /// <summary>
    /// Get scene as T
    /// </summary>
    /// <typeparam name="T">Scene type</typeparam>
    /// <returns>Scene cast as T</returns>
    public T? GetSceneAs<T>()
        where T : Scene => (T?)Scene;

    /// <summary>
    /// Add child and return it
    /// </summary>
    /// <param name="widget">Widget which will be added</param>
    /// <typeparam name="T">Type of widget</typeparam>
    /// <returns>Child</returns>
    public T AddChild<T>(T widget)
        where T : Widget
    {
        widget.Scene = Scene;
        widget.Parent = this;
        Children.Add(widget);
        return widget;
    }

    /// <summary>
    /// Remove child
    /// </summary>
    /// <param name="widget">Child which will be removed</param>
    public void RemoveChild(Widget widget)
    {
        widget.Scene = null;
        widget.Parent = null;
        Children.Remove(widget);
    }

    /// <summary>
    /// Remove all children
    /// </summary>
    public void RemoveAllChildren()
    {
        foreach (var child in Children)
        {
            child.Scene = null;
            child.Parent = null;
        }

        Children.Clear();
    }

    /// <summary>
    /// Refresh widget bounds
    /// </summary>
    public void RefreshBounds()
    {
        Bounds = CalculateBounds();

        foreach (var child in Children)
            child.RefreshBounds();
    }

    /// <summary>
    /// Calculate widget bounds
    /// </summary>
    /// <returns>Widget bounds in screen space</returns>
    protected virtual Rectangle CalculateBounds()
    {
        return new Rectangle(RealPosition.X, RealPosition.Y, 0, 0);
    }

    /// <summary>
    /// Get the anchor point from which widget position is measured
    /// </summary>
    /// <returns>Anchor point in screen space</returns>
    protected Vector2 GetAnchorPoint()
    {
        Rectangle referenceRect = Parent?.Bounds ?? GetViewportBounds();

        float anchorX = HorizontalAnchor switch
        {
            HorizontalAlignment.Left => referenceRect.X,
            HorizontalAlignment.Center => referenceRect.X + (referenceRect.Width * 0.5f),
            HorizontalAlignment.Right => referenceRect.X + referenceRect.Width,
            _ => referenceRect.X
        };

        float anchorY = VerticalAnchor switch
        {
            VerticalAlignment.Top => referenceRect.Y,
            VerticalAlignment.Center => referenceRect.Y + (referenceRect.Height * 0.5f),
            VerticalAlignment.Bottom => referenceRect.Y + referenceRect.Height,
            _ => referenceRect.Y
        };

        return new Vector2(anchorX, anchorY);
    }

    /// <summary>
    /// Get offset from position to top-left corner using widget alignment
    /// </summary>
    /// <param name="size">Widget size</param>
    /// <returns>Offset from position to top-left corner</returns>
    protected Vector2 GetAlignmentOffset(Vector2 size)
    {
        float offsetX = HorizontalAlign switch
        {
            HorizontalAlignment.Left => 0,
            HorizontalAlignment.Center => size.X * 0.5f,
            HorizontalAlignment.Right => size.X,
            _ => 0
        };

        float offsetY = VerticalAlign switch
        {
            VerticalAlignment.Top => 0,
            VerticalAlignment.Center => size.Y * 0.5f,
            VerticalAlignment.Bottom => size.Y,
            _ => 0
        };

        return new Vector2(offsetX, offsetY);
    }

    /// <summary>
    /// Create bounds from current position, alignment and widget size
    /// </summary>
    /// <param name="size">Widget size</param>
    /// <returns>Aligned widget bounds</returns>
    protected Rectangle GetAlignedBounds(Vector2 size)
    {
        Vector2 position = RealPosition - GetAlignmentOffset(size);
        return new Rectangle(position.X, position.Y, size.X, size.Y);
    }

    private Rectangle GetViewportBounds()
    {
        if (Scene?.Window == null)
            return new Rectangle(0, 0, 0, 0);

        return new Rectangle(0, 0, Scene.Window.RenderSize.X, Scene.Window.RenderSize.Y);
    }

    /// <summary>
    /// Load widget
    /// </summary>
    public virtual void Load()
    {
        foreach (var child in Children)
            child.Load();
    }

    /// <summary>
    /// Unload widget
    /// </summary>
    public virtual void Unload()
    {
        foreach (var child in Children)
            child.Unload();
    }

    /// <summary>
    /// Update widget
    /// </summary>
    /// <param name="delta">Time since last frame</param>
    public virtual void Update(float delta)
    {
        foreach (var child in Children.OrderByDescending(child => child.ZLayer))
            child.Update(delta);
    }

    /// <summary>
    /// Draw widget
    /// </summary>
    public virtual void Draw()
    {
        if (!RealDisplayed)
            return;

        foreach (var child in Children.OrderBy(child => child.ZLayer))
            child.Draw();
    }

    private static bool Intersects(Rectangle first, Rectangle second)
    {
        return first.X < second.X + second.Width
            && first.X + first.Width > second.X
            && first.Y < second.Y + second.Height
            && first.Y + first.Height > second.Y;
    }
}
