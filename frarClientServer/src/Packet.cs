using Newtonsoft.Json;
namespace frar.clientserver;

/// <summary>
/// Communication packet for the Connection class.<br>
/// Generates a JSON (JObject) packet.
/// </summary>
public class Packet {
    public static readonly String ACTION_FIELD = "Action";
    public static readonly String PARAMETER_FIELD = "Parameters";

    private string action;
    private Dictionary<string, object> parameters = new Dictionary<string, object>();
    private Dictionary<string, object> data = new Dictionary<string, object>();

    public String Action{
        get { return action; }
    }

    public object this[string key]{
        get { return parameters[key]; }
        set { parameters[key] = value; }
    }

    public Dictionary<string, object> Parameters {
        get { return parameters; }
    }

    [JsonIgnore]
    public Dictionary<string, object> Data {
        get { return data; }
    }

    /// <summary>
    /// Create a new packet with the specified action and parameters.
    /// </summary>
    /// <param name="action">String that triggers routes</param>
    public Packet(string action) {
        this.action = action;
    }

    /// <summary>
    /// Create a new packet from a JObject.
    /// </summary>
    /// <param name="action">A JObject with at least an 'action' field.</param>
    static public Packet FromString(string jsonString) {
        ArgumentNullException.ThrowIfNull(jsonString);
        Packet? packet = JsonConvert.DeserializeObject<Packet>(jsonString);
        ArgumentNullException.ThrowIfNull(packet);
        return packet;
    }

    /// <summary>
    /// Add a parameter to this packet.
    /// Parameters must be of a primitive type or string.
    /// Setting a parameter multiple times will overwrite it.
    /// </summary>
    /// <param name="name">Name of the parameter</param>
    /// <param name="value">Parameter value (primitive)</param>
    public Packet Set(string name, IConvertible value) {
        if (this.parameters.ContainsKey(name)) this.parameters.Remove(name);
        this.parameters.Add(name, value);
        return this;
    }

    /// <summary>
    /// Determine if the packet has the given parameter.
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns>true if the packet has parameter</returns>
    public bool Has(string parameter){
        return this.parameters.ContainsKey(parameter);
    }

    /// <summary>
    /// Generate a JObject string from this packet.
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
        return JsonConvert.SerializeObject(this);
    }

    /// <summary>
    /// Generate a JObject string from this packet.
    /// </summary>
    /// <returns></returns>
    public string ToString(Formatting formatting) {
        return JsonConvert.SerializeObject(this, formatting);
    }    
}
