using System.Reflection;
namespace frar.clientserver;

/// <summary>
/// Data class for recording methods marked by the Route annotation.
/// Used by AttributeParser.
/// </summary>
public class RouteEntry {
    public readonly MethodInfo MethodInfo;
    public readonly string Rule;
    public readonly int Index;
    public readonly object Handler;

    public RouteEntry(MethodInfo methodInfo, string rule, int index, object Handler) {
        this.MethodInfo = methodInfo;
        this.Rule = rule;
        this.Index = index;
        this.Handler = Handler;
    }

    public override string ToString() {
        return $"{MethodInfo.Name} {Rule} {Index}";
    }
}

/// <summary>
/// Parses attributes from a class for use in routers.
/// </summary>
class AttributeParser {
    public static List<MethodInfo> SeekOnConnect(Object target) {
        List<MethodInfo> methodList = new List<MethodInfo>();
        Type type = target.GetType();
        MethodInfo[] methodInfos = type.GetMethods();

        foreach (MethodInfo methodInfo in methodInfos) {
            OnConnect? onConnect = methodInfo.GetCustomAttribute<OnConnect>();
            if (onConnect != null) methodList.Add(methodInfo);
        }

        return methodList;
    }

    public static List<MethodInfo> SeekOnDisconnect(Object target) {
        List<MethodInfo> methodList = new List<MethodInfo>();
        Type type = target.GetType();
        MethodInfo[] methodInfos = type.GetMethods();

        foreach (MethodInfo methodInfo in methodInfos) {
            OnDisconnect? onDisconnect = methodInfo.GetCustomAttribute<OnDisconnect>();
            if (onDisconnect != null) methodList.Add(methodInfo);
        }

        return methodList;
    }

    /// <summary>
    /// Look for methods marked [Route] on handler object.
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static List<RouteEntry> SeekRoutes(Object handler) {
        List<RouteEntry> routeList = new List<RouteEntry>();
        Type type = handler.GetType();
        MethodInfo[] methodInfos = type.GetMethods();

        foreach (MethodInfo info in methodInfos) {
            Route? route = info.GetCustomAttribute<Route>();
            if (route == null) continue;

            string action = route.Rule;
            if (action == "") action = $"(?i)^{info.Name.ToLower()}$";

            routeList.Add(new RouteEntry(info, action, route.Index, handler));
        }

        return routeList;
    }
}