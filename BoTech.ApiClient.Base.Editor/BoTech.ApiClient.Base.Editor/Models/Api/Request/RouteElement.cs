using System.Collections.Generic;
using BoTech.ApiClient.Base.Editor.Models.Api.Request.Parameters;
using ReactiveUI.Primitives.Advanced;

namespace BoTech.ApiClient.Base.Editor.Models.Api.Request;

public class RouteElement
{
    private RouteElement? _next = null;
    
    public RouteParameter? Parameter { get; private set; } = null;

    public static RouteElement CreateRouteElementFromRoutParameter(RouteParameter param) => new RouteElement() { Parameter = param };

    /// <summary>
    /// Adds a new element to the end of the route
    /// </summary>
    /// <param name="next">the new element to add.</param>
    public void Add(RouteElement next)
    {
        if (Parameter is null)
        {
            Parameter = next.Parameter;
            return;
        }
        if (_next is not null)
            _next.Add(next);
        else
            _next = next;
    }
    /// <summary>
    /// Deletes the next element from the route
    /// </summary>
    public void DeleteNext()
    {
        _next = null;
    }
    /// <summary>
    /// Converts the route structure into a list of RouteElement objects.
    /// </summary>
    /// <returns>A list containing all RouteElement objects in the current route structure, starting from the current instance.</returns>
    public List<RouteElement> ToList()
    {
        List<RouteElement> list = new();
        AddAllToList(list);
        return list;
    }
    private void AddAllToList(List<RouteElement> list)
    {
        list.Add(this);
        if(_next is not null)
            _next.AddAllToList(list);
    }

    public bool IsEmpty()
    {
        return Parameter is null;
    }
    public override string ToString()
    {
        if (Parameter is null)
            return "EMPTY";
        return Parameter + (_next is null ? "" : _next.ToString());
    }
}