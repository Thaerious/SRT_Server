namespace frar.clientserver;

/// <summary>
/// Thrown by the Connection class.
/// </summary>
public class MissingParameterException : Exception {

    public MissingParameterException(string method, string parameter)
        : base($"Missing parameter '{parameter}' expected in method '{method}'.") {}

    public MissingParameterException(string method, string parameter, Exception inner)
        : base($"Missing parameter '{parameter}' expected in method '{method}'.", inner) {}
}