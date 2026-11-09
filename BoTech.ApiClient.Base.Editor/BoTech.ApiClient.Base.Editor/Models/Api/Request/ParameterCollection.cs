using System;
using System.Collections.ObjectModel;
using BoTech.ApiClient.Base.Editor.Models.Api.Request.Parameters;

namespace BoTech.ApiClient.Base.Editor.Models.Api.Request;

/// <summary>
/// This collection must not contain <see cref="RouteParameter"/> instances.
/// Therefore, this class is only used in special cases where the <see cref="RouteParameter"/> instances are not needed or stored somewhere else.
/// See <see cref="EndpointRequest"/> for instance.
/// </summary>
public class ParameterCollection : Collection<IParameterBase>
{
    protected override void InsertItem(int index, IParameterBase item)
    {
        Validate(item);
        base.InsertItem(index, item);
    }

    protected override void SetItem(int index, IParameterBase item)
    {
        Validate(item);
        base.SetItem(index, item);
    }

    private static void Validate(IParameterBase item)
    {
        if (item is RouteParameter)
            throw new ArgumentException(
                $"{nameof(RouteParameter)} is not allowed in this collection.", nameof(item));
    }
}