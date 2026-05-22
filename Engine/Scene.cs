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

    private readonly Queue<Entity> _addEntities = [];
    private readonly Queue<Entity> _removeEntities = [];

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
    /// Load Scene
    /// </summary>
    public virtual void Load()
    {
        foreach (var entity in Entities)
            entity.Load();
    }

    /// <summary>
    /// Unload Scene
    /// </summary>
    public virtual void Unload()
    {
        foreach (var entity in Entities)
            entity.Unload();
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
                Entities[i].PauseState is PauseState.Enabled
                || !Paused && Entities[i].PauseState is PauseState.Normal
                || Paused && Entities[i].PauseState is PauseState.WhenPaused
            )
                Entities[i].Update(delta);
    }

    private void ProcessAddingAndRemoving()
    {
        while (_removeEntities.TryDequeue(out Entity? entity))
            RemoveEntity(entity, false);
        
        while (_addEntities.TryDequeue(out Entity? entity))
            AddEntity(entity, false).Load();
            
    }

    /// <summary>
    /// Draw Scene
    /// </summary>
    public virtual void Draw()
    {
        foreach (var entity in Entities)
            entity.Draw();
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

