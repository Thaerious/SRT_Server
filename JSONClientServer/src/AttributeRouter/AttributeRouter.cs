using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace frar.JSONServer;

public class AttributeRouter : ThreadedConnHnd {
    public static readonly String ACTION_FIELD = "action";
    private bool initialized = false;
    private List<RouteEntry> routes;

    public bool TerminateRoute {
        get; set;
    } = false;

    public List<RouteEntry> Routes {
        get { return new List<RouteEntry>(routes); }
    }

    public AttributeRouter() {
        this.routes = new List<RouteEntry>();
    }

    public override void OnConnect(Connection connection) {
        base.OnConnect(connection);
        this.Initialize();
        
        foreach(MethodInfo method in AttributeParser.SeekOnConnect(this)){
            method.Invoke(this, new object[]{connection});
        }
    }

    public void Initialize() {
        if (initialized) return;
        this.routes = AttributeParser.SeekRoutes(this);
        this.initialized = true;
    }

    public void Process(string isString) {
        this.Process(JObject.Parse(isString));
    }

    public override void Process(JObject jsonReq) {
        String triggerValue = (string)jsonReq[ACTION_FIELD]!;
        if (triggerValue == null) throw new Exception();       

        this.TerminateRoute = false;
        foreach (RouteEntry routeEntry in routes) {
            if (this.TerminateRoute) break;

            Regex rx = new Regex(routeEntry.Rule);
            if (rx.Matches(triggerValue).Count > 0) {
                System.Console.WriteLine(routeEntry.Rule);
                System.Console.WriteLine(jsonReq);

                MethodInfo method = routeEntry.MethodInfo;

                List<Object> parameters = new List<Object>();

                // Perform checks on the parameters.
                // Call each check method, if it returns true then
                // that method was added to the parameter list.
                foreach (ParameterInfo parameterInfo in method.GetParameters()) {
                    if (ReqParam(parameterInfo, jsonReq, parameters)) continue;
                    if (ConvertParam(parameterInfo, jsonReq, parameters)) continue;
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

    // Check if the parameter is the request object.
    private bool ReqParam(ParameterInfo parameterInfo, JObject jsonReq, List<Object> parameters) {
        Req? req = parameterInfo.GetCustomAttribute<Req>();
        if (req == null) return false;
        parameters.Add(jsonReq);
        return true;
    }

    // Convert the json value to the parameter value.
    private bool ConvertParam(ParameterInfo parameterInfo, JObject jsonReq, List<Object> parameters) {
        ArgumentNullException.ThrowIfNull(parameterInfo);
        ArgumentNullException.ThrowIfNull(jsonReq);
        ArgumentNullException.ThrowIfNull(parameters);
        if (parameterInfo.Name == null) return true;

        JToken jParam = jsonReq["parameters"]!;
        ArgumentNullException.ThrowIfNull(jParam);

        JToken jToken = jParam[parameterInfo.Name]!;
        ArgumentNullException.ThrowIfNull(jToken);

        JValue jValue = (JValue)jToken;
        object value = jValue.Value!;

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