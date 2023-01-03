using System.Reflection;
namespace frar.clientserver;

/// <summary>
/// Annotated methods get invoked when a disconnect occurs.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class OnDisconnect : Attribute {}

/// <summary>
/// Annotated methods get invoked when a new connection occurs.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class OnConnect : Attribute {}

/// <summary>
/// Annotating a parameter in a Route marks it for the incoming
/// request packet.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class Req : Attribute {}

/// <summary>
/// Annotating a parameter in a Route marks it as a controller object.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class Ctrl : Attribute {}

/// <summary>
/// Annotated methods will be considered when processing incoming packets.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class Route : Attribute {
    private string rule;
    public virtual string Rule {
        get { return rule; }
        set { rule = value; }
    }

    private int index;
    public virtual int Index {
        get { return index; }
        set { index = value; }
    }

    public virtual MethodInfo? Method {
        get; set;
    }

    public Route(string action) {
        this.rule = action;
        this.index = 0;
    }

    public Route() {
        this.rule = "";
        this.index = 0;
    }    
}