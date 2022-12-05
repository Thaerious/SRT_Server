using System.Reflection;

namespace frar.JSONServer;

[AttributeUsage(AttributeTargets.Method)]
public class OnConnect : Attribute {}

[AttributeUsage(AttributeTargets.Parameter)]
public class Req : Attribute {}

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