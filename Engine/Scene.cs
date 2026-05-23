namespace Engine;

/// <summary>
/// Class which represents a Scene
/// </summary>
public class Scene
{
    /// <summary>
    /// Define if Scene is paused
    /// </summary>
    public bool Paused { get; set; }

    /// <summary>
    /// Window that has this scene
    /// </summary>
    public Window? Window { get; set; }

    /// <summary>
    /// All Entities of Scene
    /// </summary>
    public List<Entity> Entities { get; } = [];

    /// <summary>
    /// All widgets of scene
    /// </summary>
    public List<Widget> Widgets { get; } = [];

    private readonly Queue<Entity> _addEntities = [];
    private readonly Queue<Entity> _removeEntities = [];
    private readonly Queue<Widget> _addWidgets = [];
    private readonly Queue<Widget> _removeWidgets = [];

    /// <summary>
    /// Create Scene
    /// </summary>
    public Scene() { }

    /// <summary>
    /// Add Entity To Scene
    /// </summary>
    /// <param name="entity">Entity to be added</param>
    /// <param name="delay">If adding must be delayed</param>
    /// <typeparam name="T">Type of Widgets</typeparam>
    /// <returns>Entity</returns>
    public T AddEntity<T>(T entity, bool delay = true)
        where T : Entity
    {
        if (delay)
            _addEntities.Enqueue(entity);
        else
        {
            entity.Scene = this;
            Entities.Add(entity);
        }

        return entity;
    }

    /// <summary>
    /// Remove Entity From Scene
    /// </summary>
    /// <param name="entity">Entity to be removed</param>
    /// <param name="delay">If remove must be delayed</param>
    public void RemoveEntity(Entity entity, bool delay = false)
    {
        if (delay)
            _removeEntities.Enqueue(entity);
        else
        {
            entity.Scene = null;
            Entities.Remove(entity);
        }
    }

    /// <summary>
    /// Remove all Entities
    /// </summary>
    public void RemoveAllEntities(bool delay = true)
    {
        if (delay)
            foreach (var entity in Entities)
                RemoveEntity(entity);
        else
        {
            foreach (var entity in Entities)
                entity.Scene = null;
            Entities.Clear();
        }
    }

    /// <summary>
    /// Add widget to scene
    /// </summary>
    /// <param name="widget">Widget to be added</param>
    /// <param name="delay">If adding must be delayed</param>
    /// <typeparam name="T">Type of widget</typeparam>
    /// <returns>Widget</returns>
    public T AddWidget<T>(T widget, bool delay = true)
        where T : Widget
    {
        if (delay)
            _addWidgets.Enqueue(widget);
        else
        {
            widget.Scene = this;
            Widgets.Add(widget);
        }

        return widget;
    }

    /// <summary>
    /// Remove widget from scene
    /// </summary>
    /// <param name="widget">Widget to remove</param>
    /// <param name="delay">If remove must be delayed</param>
    public void RemoveWidget(Widget widget, bool delay = false)
    {
        if (delay)
            _removeWidgets.Enqueue(widget);
        else
        {
            widget.Scene = null;
            Widgets.Remove(widget);
        }
    }

    /// <summary>
    /// Load Scene
    /// </summary>
    public virtual void Load()
    {
        foreach (var entity in Entities)
            entity.Load();

        foreach (var widget in Widgets)
        {
            widget.RefreshBounds();
            widget.Load();
        }
    }

    /// <summary>
    /// Unload Scene
    /// </summary>
    public virtual void Unload()
    {
        foreach (var entity in Entities)
            entity.Unload();

        foreach (var widget in Widgets)
            widget.Unload();
    }

    /// <summary>
    /// Update Scene
    /// </summary>
    /// <param name="delta">Time since last update</param>
    public virtual void Update(float delta)
    {
        ProcessAddingAndRemoving();

        for (var i = Entities.Count - 1; i > -1; i--)
            if (
                Entities[i].ActiveInHierarchy
                && (
                Entities[i].PauseState is PauseState.Enabled
                || !Paused && Entities[i].PauseState is PauseState.Normal
                || Paused && Entities[i].PauseState is PauseState.WhenPaused
                )
            )
                Entities[i].Update(delta);

        PhysicsManager.Update(this, delta);

        foreach (var widget in Widgets.OrderByDescending(widget => widget.ZLayer))
        {
            widget.RefreshBounds();

            if (
                widget.RealActive
                && (
                    widget.PauseState is PauseState.Enabled
                    || !Paused && widget.PauseState is PauseState.Normal
                    || Paused && widget.PauseState is PauseState.WhenPaused
                )
            )
                widget.Update(delta);
        }
    }

    private void ProcessAddingAndRemoving()
    {
        while (_removeEntities.TryDequeue(out Entity? entity))
            RemoveEntity(entity, false);

        while (_removeWidgets.TryDequeue(out Widget? widget))
            RemoveWidget(widget, false);
        
        while (_addEntities.TryDequeue(out Entity? entity))
            AddEntity(entity, false).Load();

        while (_addWidgets.TryDequeue(out Widget? widget))
        {
            Widget addedWidget = AddWidget(widget, false);
            addedWidget.RefreshBounds();
            addedWidget.Load();
        }
            
    }

    /// <summary>
    /// Draw Scene
    /// </summary>
    public virtual void Draw()
    {
        foreach (var entity in Entities)
            if (entity.ActiveInHierarchy)
                entity.Draw();

        foreach (var widget in Widgets.OrderBy(widget => widget.ZLayer))
            if (widget.RealDisplayed && widget.IsVisibleOnScreen)
                widget.Draw();
    }

    /// <summary>
    /// Function call when Scene is opened
    /// </summary>
    public virtual void OpenScene() { }

    /// <summary>
    /// Function call when Scene is closed
    /// </summary>
    public virtual void CloseScene() { }
}
