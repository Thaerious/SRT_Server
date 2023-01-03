using System.Reflection;
using System.Text.RegularExpressions;

namespace frar.clientserver;

public class RouterController {
    /// <summary>
    /// When set to true, no further routes will be processed.
    /// This field is reset to false whenever a new packet is received.
    /// </summary>
    public bool TerminateRoute {
        get; set;
    } = false;    
}

public class Router {
    private readonly List<RouteEntry> routes = new List<RouteEntry>();

    private IConnection? _connection;
    public IConnection Connection {
        get {
            if (_connection == null) throw new NullReferenceException();
            return _connection;
        }
        set {
            this._connection = value;
        }
    }

    public Router() {
        this.AddHandler(this);
    }

    public void AddHandler(Object handler){
        foreach (RouteEntry routeEntry in AttributeParser.SeekRoutes(handler)){
            this.routes.Add(routeEntry);
        }

        this.routes.Sort(delegate (RouteEntry a, RouteEntry b) {
            return a.Index.CompareTo(b.Index);
        });
    }

    /// <summary>
    /// Retrieve a non-reflective list of route entries.
    /// </summary>
    public List<RouteEntry> Routes {
        get { return new List<RouteEntry>(routes); }
    }

    [OnConnect]
    protected virtual void InvokeConnect(IConnection connection) {
        this.Connection = connection;
    }

    /// <summary>
    /// Called when a disconnect occurs.
    /// A 'gracefull' disconnect occurs when the server originates the disconnect.
    /// A 'broken' disconnect occurs when the socket terminates unexpectedly.
    /// </summary>
    /// <param name="reason">A description of what triggered the disconnect.</param>
    public void InvokeDisconnect(DISCONNECT_REASON reason) {
        foreach (MethodInfo method in AttributeParser.SeekOnDisconnect(this)) {
            method.Invoke(this, new object[] { reason });
        }
    }

    /// <summary>
    /// Process a packet applying values from the packet to the method parameters.
    /// Triggers all methods annotated [Route] whose 'Rule' regex
    /// matches the action field of the packet.
    /// </summary>
    /// <param name="isString">JSON object representing a packet</param>
    public void Process(Packet packet) {
        var ctrl = new RouterController();

        foreach (RouteEntry routeEntry in routes) {
            if (ctrl.TerminateRoute) break;
            packet.Reset();

            Regex rx = new Regex(routeEntry.Rule);
            if (rx.Matches(packet.Action).Count > 0) {
                MethodInfo method = routeEntry.MethodInfo;
                List<Object> parameters = new List<Object>();

                // For each parameter in the method check the packet for a match.                
                foreach (ParameterInfo parameterInfo in method.GetParameters()) {
                    if (parameterInfo.Name == null) continue; // [1]

                    // If the parameter is annotated with [Req] assign packet to value.
                    if (SeekReqAnnotation(parameterInfo, packet, parameters)) continue;

                    // If the parameter is annotated with [Ctrl] assign controller to value.
                    if (SeekCtrlAnnotation(parameterInfo, packet, parameters, ctrl)) continue;

                    // If the packet has the parameter, assign it to value.
                    if (packet.Has(parameterInfo.Name)) {
                        var arg = packet.Get(parameterInfo.ParameterType, parameterInfo.Name);
                        parameters.Add(arg);
                        continue;
                    }

                    // Attempt to retrieve an anonymous argument.
                    if (packet.HasAnon()) {
                        parameters.Add(packet.NextAnon());
                        continue;
                    }

                    // If the parameter provides a default value, use it.
                    if (parameterInfo.HasDefaultValue) {
                        parameters.Add(parameterInfo.DefaultValue!);
                        continue;
                    }

                    throw new MissingParameterException(method.Name, parameterInfo.Name);
                }

                try {
                    method.Invoke(routeEntry.Handler, parameters.ToArray());
                }
                catch (TargetParameterCountException ex) {
                    string msg = $"Parameter count mismatch for method '{routeEntry.MethodInfo.Name}'. ";
                    msg += $"Found {parameters.Count} expected {method.GetParameters().Length}.";
                    throw new TargetParameterCountException(msg, ex);
                }
            }
        }
    }

    /// <summary>
    /// Check if the parameter is the request object.
    /// </summary>
    /// <param name="parameterInfo"></param>
    /// <param name="packet"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private bool SeekReqAnnotation(ParameterInfo parameterInfo, Packet packet, List<Object> parameters) {
        Req? req = parameterInfo.GetCustomAttribute<Req>();
        if (req == null) return false;
        parameters.Add(packet);
        return true;
    }

    private bool SeekCtrlAnnotation(ParameterInfo parameterInfo, Packet packet, List<Object> parameters, RouterController routerController) {
        Ctrl? ctrl = parameterInfo.GetCustomAttribute<Ctrl>();
        if (ctrl == null) return false;
        parameters.Add(routerController);
        return true;
    }    
}

//[1] https://learn.microsoft.com/en-us/dotnet/api/system.reflection.parameterinfo.name?view=net-7.0#system-reflection-parameterinfo-name