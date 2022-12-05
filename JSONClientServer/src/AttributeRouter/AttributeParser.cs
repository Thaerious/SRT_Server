using System.Reflection;

namespace frar.JSONServer;

public class RouteEntry {
    public readonly MethodInfo MethodInfo;
    public readonly string Rule;
    public readonly int Index;

    public RouteEntry(MethodInfo methodInfo, string rule, int index) {
        this.MethodInfo = methodInfo;
        this.Rule = rule;
        this.Index = index;
    }

    public override string ToString() {
        return $"{MethodInfo.Name} {Rule} {Index}";
    }
}

public class AttributeParser {
    public static List<RouteEntry> SeekAttributes(Object target) {
        List<RouteEntry> routeList = new List<RouteEntry>();

        Type type = target.GetType();
        MethodInfo[] methodInfos = type.GetMethods();

        foreach (MethodInfo info in methodInfos) {
            Route? route = info.GetCustomAttribute<Route>();
            if (route == null) continue;

            string action = route.Rule;
            if (action == "") action = $"(?i)^{info.Name.ToLower()}$";

            routeList.Add(new RouteEntry(info, action, route.Index));
        }

        routeList.Sort(delegate (RouteEntry a, RouteEntry b) {
            return a.Index.CompareTo(b.Index);
        });

        return routeList;
    }
}