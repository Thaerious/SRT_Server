using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace frar.JSONServer;

/// <summary>
/// Listens for incoming packets on it's own thread.
/// Extend this class and add annotations to dictate behaviour.
/// The accepted method annotations are: OnConnect, OnDisconnect, Route.
/// </summary>
public class ThreadedAttributeRouter : ThreadedConnHnd {
    private List<RouteEntry> routes;

    /// <summary>
    /// When set to true, no further routes will be processed.
    /// This field is reset to false whenever a new packet is received.
    /// </summary>
    public bool TerminateRoute {
        get; set;
    } = false;

    /// <summary>
    /// Retrieve a non-reflective list of route entries.
    /// </summary>
    public List<RouteEntry> Routes {
        get { return new List<RouteEntry>(routes); }
    }

    /// <summary>
    /// Default no-arg constructor.
    /// </summary>
    public ThreadedAttributeRouter() {
        this.routes = AttributeParser.SeekRoutes(this);
    }

    /// <summary>
    /// Called when a connection occurs, will invoke all mtehods with the 
    /// [OnConnection] annotation.
    /// </summary>
    /// <param name="connection">The new conneciton object</param>
    public override void OnConnect(Connection connection) {
        base.OnConnect(connection);
        
        foreach(MethodInfo method in AttributeParser.SeekOnConnect(this)){
            method.Invoke(this, new object[]{connection});
        }
    }

    /// <summary>
    /// Called when a disconnect occurs.
    /// A 'gracefull' disconnect occurs when the server originates the disconnect.
    /// A 'broken' disconnect occurs when the socket terminates unexpectedly.
    /// </summary>
    /// <param name="reason">A description of what triggered the disconnect.</param>
    public override void OnDisconnect(DISCONNECT_REASON reason) {
        base.OnDisconnect(reason);
        foreach(MethodInfo method in AttributeParser.SeekOnDisconnect(this)){
            method.Invoke(this, new object[]{reason});
        }        
    }

    /// <summary>
    /// Process a packet as a json object.
    /// Triggers all methods annotated [Route] whose 'Rule' regex
    /// matches the action field of the packet.
    /// </summary>
    /// <param name="isString">JSON object representing a packet</param>
    public override void Process(Packet packet) {
        this.TerminateRoute = false;
        foreach (RouteEntry routeEntry in routes) {
            if (this.TerminateRoute) break;

            Regex rx = new Regex(routeEntry.Rule);
            if (rx.Matches(packet.Action).Count > 0) {
                MethodInfo method = routeEntry.MethodInfo;

                List<Object> parameters = new List<Object>();

                // Perform checks on the parameters.
                // Call each check method, if it returns true then
                // that method was added to the parameter list.
                foreach (ParameterInfo parameterInfo in method.GetParameters()) {
                    if (SeekReqAnnotation(parameterInfo, packet, parameters)) continue;
                    if (TypeCheckParameters(parameterInfo, packet, parameters)) continue;
                }

                try{
                    method.Invoke(this, parameters.ToArray());
                } catch (TargetParameterCountException ex){
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

        object value = packet[parameterInfo.Name];

        switch (parameterInfo.ParameterType.ToString()) {
            case "System.Boolean":
                value = Convert.ToBoolean(value);
                break;
            case "System.Byte":
                value = Convert.ToByte(value);
                break;
            case "System.SByte":
                value = Convert.ToSByte(value);
                break;
            case "System.Char":
                value = Convert.ToChar(value);
                break;
            case "System.Decimal":
                value = Convert.ToDecimal(value);
                break;
            case "System.Double":
                value = Convert.ToDouble(value);
                break;
            case "System.Single":
                value = Convert.ToSingle(value);
                break;
            case "System.Int16":
                value = Convert.ToInt16(value);
                break;
            case "System.UInt16":
                value = Convert.ToUInt16(value);
                break;
            case "System.Int32":
                value = Convert.ToInt32(value);
                break;
            case "System.UInt32":
                value = Convert.ToUInt32(value);
                break;
            case "System.Int64":
                value = Convert.ToInt64(value);
                break;
            case "System.UInt64":
                value = Convert.ToUInt64(value);
                break;
            default:
                Convert.ChangeType(value, parameterInfo.ParameterType);
                break;
        }

        parameters.Add(value);
        return true;
    }
}