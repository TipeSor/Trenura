namespace Engine;

/// <summary>
/// Class which represents Entity
/// </summary>
public class Entity
{
    /// <summary>
    /// Define if Entity is active
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    /// How Entity must be updated when paused
    /// </summary>
    public PauseState PauseState { get; set; } = PauseState.Normal;

    /// <summary>
    /// Tag of Entity
    /// </summary>
    public string Tag { get; set; } = "";

    /// <summary>
    /// Name of Entity
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Scene of Entity
    /// </summary>
    public Scene? Scene { get; set; }

    /// <summary>
    /// Get All Components of Entity
    /// </summary>
    public List<Component> Components { get; } = [];

    /// <summary>
    /// Get All Children of Entity
    /// </summary>
    public List<Entity> Children { get; } = [];

    /// <summary>
    /// Parent of Entity
    /// </summary>
    public Entity? Parent { get; set; }

    /// <summary>
    /// Get All Components of one Type
    /// </summary>
    /// <typeparam name="T">Type of Component</typeparam>
    /// <returns>Components of type T</returns>
    public List<T> GetComponents<T>()
        where T : Component => Components.OfType<T>().ToList();

    /// <summary>
    /// Get Component of one Type
    /// </summary>
    /// <typeparam name="T">Type of Component</typeparam>
    /// <returns>Component of type T</returns>
    public T? GetComponent<T>()
        where T : Component => Components.OfType<T>().FirstOrDefault();

    /// <summary>
    /// Get Scene as T
    /// </summary>
    /// <typeparam name="T">Scene Type</typeparam>
    /// <returns>Scene cast as T</returns>
    public T? GetSceneAs<T>()
        where T : Scene => (T?)Scene;

    /// <summary>
    /// Check if Entity is active in hierarchy
    /// </summary>
    public bool ActiveInHierarchy => Active && (Parent?.ActiveInHierarchy ?? true);

    /// <summary>
    /// Add Child Entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public T AddChild<T>(T entity)
        where T : Entity
    {
        Children.Add(entity);
        entity.Parent = this;
        entity.Scene = Scene;
        return entity;
    }

    /// <summary>
    /// Add a Component and return it
    /// </summary>
    /// <param name="component">Component which be added</param>
    /// <typeparam name="T">Type of Component</typeparam>
    /// <returns>Component</returns>
    public T AddComponent<T>(T component)
        where T : Component
    {
        Components.Add(component);
        component.Entity = this;
        return component;
    }

    /// <summary>
    /// Remove Child
    /// </summary>
    /// <param name="entity">Child</param>
    public void RemoveChild(Entity entity)
    {
        entity.Parent = null;
        entity.Scene = null;
        Children.Remove(entity);
    }

    /// <summary>
    /// Remove Component
    /// </summary>
    /// <param name="component">Component will be removed</param>
    public void RemoveComponent(Component component)
    {
        component.Entity = null;
        Components.Remove(component);
    }

    /// <summary>
    /// Load Entity
    /// </summary>
    public virtual void Load()
    {
        foreach (var component in Components)
            component.Load();

        foreach (var child in Children)
            child.Load();
    }

    /// <summary>
    /// Unload Entity
    /// </summary>
    public virtual void Unload()
    {
        foreach (var component in Components)
            component.Unload();

        foreach (var child in Children)
            child.Unload();
    }

    /// <summary>
    /// Update Entity
    /// </summary>
    /// <param name="delta">Time since last frame</param>
    public virtual void Update(float delta)
    {
        if (!Active)
            return;

        foreach (var component in Components)
            component.Update(delta);

        foreach (var child in Children)
            child.Update(delta);
    }

    /// <summary>
    /// Draw Entity
    /// </summary>
    public virtual void Draw()
    {
        if (!Active)
            return;

        foreach (var component in Components)
            component.Draw();

        foreach (var child in Children)
            child.Draw();
    }
}
