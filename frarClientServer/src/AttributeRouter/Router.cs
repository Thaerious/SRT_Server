using System.Reflection;
using System.Text.RegularExpressions;

namespace frar.clientserver;

public class Router {
    private readonly List<object> Handlers = new List<Object>();
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

    /// <summary>
    /// When set to true, no further routes will be processed.
    /// This field is reset to false whenever a new packet is received.
    /// </summary>
    public bool TerminateRoute {
        get; set;
    } = false;

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
    protected virtual void OnConnect(IConnection connection) {
        System.Console.WriteLine("ON CONNECT ROUTER");
        this.Connection = connection;
    }

    /// <summary>
    /// Called when a disconnect occurs.
    /// A 'gracefull' disconnect occurs when the server originates the disconnect.
    /// A 'broken' disconnect occurs when the socket terminates unexpectedly.
    /// </summary>
    /// <param name="reason">A description of what triggered the disconnect.</param>
    public void OnDisconnect(DISCONNECT_REASON reason) {
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
        this.TerminateRoute = false;
        foreach (RouteEntry routeEntry in routes) {
            if (this.TerminateRoute) break;

            Regex rx = new Regex(routeEntry.Rule);
            if (rx.Matches(packet.Action).Count > 0) {
                MethodInfo method = routeEntry.MethodInfo;

                List<Object> parameters = new List<Object>();

                // Perform checks on the parameters.
                // Call each check method, if it returns true do not continue processing.                
                foreach (ParameterInfo parameterInfo in method.GetParameters()) {
                    if (CheckDefault(method, parameterInfo, packet, parameters)) continue;
                    if (SeekReqAnnotation(parameterInfo, packet, parameters)) continue;
                    CheckParameterExists(method, parameterInfo, packet);
                    if (TypeCheckParameters(parameterInfo, packet, parameters)) continue;
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
    /// Ensures that if the parameter exists on the method that it is present in the packet.
    /// If the it's not in the packet then check if there is a default value.
    /// If there is no default value, throw a missing parameter exception.
    /// </summary>
    /// <param name="parameterInfo"></param>
    /// <returns>true to terminate the parameter processing</returns>            
    public bool CheckDefault(MethodInfo method, ParameterInfo parameterInfo, Packet packet, List<Object> parameters) {
        if (parameterInfo.Name == null) return true;
        if (packet.Has(parameterInfo.Name)) return false;
        if (parameterInfo.HasDefaultValue) {
            parameters.Add(parameterInfo.DefaultValue!);
            return true;
        }
        return false;
    }

    public void CheckParameterExists(MethodInfo method, ParameterInfo parameterInfo, Packet packet) {
        if (parameterInfo.Name == null) return;
        if (packet.Has(parameterInfo.Name)) return;
        throw new MissingParameterException(method.Name, parameterInfo.Name);
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

    /// <summary>
    /// Convert the json value to the parameter value.
    /// </summary>
    /// <param name="parameterInfo"></param>
    /// <param name="packet"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private bool TypeCheckParameters(ParameterInfo parameterInfo, Packet packet, List<Object> parameters) {
        ArgumentNullException.ThrowIfNull(parameterInfo);
        ArgumentNullException.ThrowIfNull(packet);
        ArgumentNullException.ThrowIfNull(parameters);
        if (parameterInfo.Name == null) return true;

        var arg = packet.Get(parameterInfo.ParameterType, parameterInfo.Name);
        parameters.Add(arg);

        return true;
    }
}