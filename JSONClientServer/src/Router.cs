namespace frar.JSONServer;
using Newtonsoft.Json.Linq;

// TODO DELETE ME
public class Router {
    public static readonly String TRIGGER_FIELD = "action";
    public delegate void Next();
    public delegate void Handler(JObject jObject, Next next);

    private Dictionary<string, LinkedList<Handler>> handlers = new Dictionary<string, LinkedList<Handler>>();

    public void Use(String triggerValue, params Handler[] handlers){
        if (!this.handlers.ContainsKey(triggerValue)){
            this.handlers.Add(triggerValue, new LinkedList<Handler>());            
        }

        LinkedList<Handler> list = this.handlers[triggerValue];
        
        foreach(Handler handler in handlers){
           list.AddLast(handler);
        }
    }

    public void Process(JObject jObject){
        String triggerValue = (string)jObject[TRIGGER_FIELD]!;
        if (!this.handlers.ContainsKey(triggerValue)) return;
        this.Process(jObject, new LinkedList<Handler>(this.handlers[triggerValue]));
    }

    void Process(JObject jObject, LinkedList<Handler> list){
        if (list.Count == 0) return;
        var handler = list.First();
        list.RemoveFirst();
        handler(jObject, ()=>this.Process(jObject, list));
    }
}