using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class JSONExt{

    public static bool EnsureKeys(this JObject jObject, params string[] keys){
        foreach (string key in keys){
            if (jObject[key] == null) return false;
        }
        return true;
    }
}
