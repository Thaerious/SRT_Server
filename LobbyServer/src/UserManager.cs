using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace frar.LobbyServer;

public class UserManager {
    public static int SALT_LENGTH = 64;
    public static int REHASH_TIME_MS = 1000;

    MySqlConnection sqlConnnection = null!;

    // Connect to the mysql database.
    // configPath: path to JSON configuration file
    // Json configuration file must contain the username, password,
    // ip, and database to use.
    public void Connect(string configPath) {
        string config = File.ReadAllText(configPath);
        JObject json = JObject.Parse(config);

        if (json["sql"] == null) throw new Exception("missing configuration field sql");
        if (json["sql"]!["ip"] == null) throw new Exception("missing configuration field sql.ip");
        if (json["sql"]!["username"] == null) throw new Exception("missing configuration field sql.username");
        if (json["sql"]!["password"] == null) throw new Exception("missing configuration field sql.password");
        if (json["sql"]!["database"] == null) throw new Exception("missing configuration field sql.database");

        string ip = json["sql"]!["ip"]!.Value<string>()!;
        string username = json["sql"]!["username"]!.Value<string>()!;
        string password = json["sql"]!["password"]!.Value<string>()!;
        string database = json["sql"]!["database"]!.Value<string>()!;

        string cs = $"server={ip};username={username};password={password};database={database}";
        this.sqlConnnection = new MySqlConnection(cs);
        sqlConnnection.Open();
    }

    public void Disconnect() {
        this.sqlConnnection.Close();
    }

    public bool UpdatePassword(string username, string password) {
        if (!this.HasUser(username)) return false;
        string email = this.GetEmail(username);
        string status = this.GetStatus(username);

        this.RemoveUser(username);
        this.AddUser(username, password, email);
        this.UpdateStatus(username, status);
        return true;
    }

    public bool UpdateEmail(string username, string email) {
        if (!this.HasUser(username)) return false;

        string sql = "UPDATE users SET email = @email WHERE username = @username";
        var cmd = new MySqlCommand(sql, this.sqlConnnection);

        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@email", email);

        cmd.Prepare();
        cmd.ExecuteNonQuery();
        return true;
    }

    public bool UpdateStatus(string username, string status) {
        if (!this.HasUser(username)) return false;

        string sql = "UPDATE users SET status = @status WHERE username = @username";
        var cmd = new MySqlCommand(sql, this.sqlConnnection);

        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@status", status);

        cmd.Prepare();
        cmd.ExecuteNonQuery();
        return true;
    }

    public bool AddUser(string username, string password, string email) {
        if (this.HasUser(username)) return false;
        if (!ValidateUsername(username)) throw new InvalidUsernameException(username);

        string sql = "INSERT INTO users(username, salt, password, iterations, email,status) values (@username, @salt, @password, @iterations, @email, @status)";
        var cmd = new MySqlCommand(sql, this.sqlConnnection);

        int iterations = 0;
        byte[] plainPW = Encoding.UTF8.GetBytes(password);
        byte[] salt = GenerateSalt();
        byte[] saltedPW = ApplySalt(plainPW, salt);
        byte[] hashed = Hash(saltedPW, ref iterations);

        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@salt", Convert.ToBase64String(salt));
        cmd.Parameters.AddWithValue("@password", Convert.ToBase64String(hashed));
        cmd.Parameters.AddWithValue("@iterations", iterations);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@status", "new");

        cmd.Prepare();
        cmd.ExecuteNonQuery();
        return true;
    }

    public bool VerifyUser(string username, string password) {
        byte[] plainPW = Encoding.UTF8.GetBytes(password);
        byte[] salt = GetSalt(username);
        byte[] saltedPW = ApplySalt(plainPW, salt);
        byte[] hashed = ReHash(saltedPW, GetIterations(username));

        return IsEqual(GetHash(username), hashed);
    }

    private bool IsEqual(byte[] expected, byte[] actual) {
        if (expected.Length != actual.Length) return false;
        for (int i = 0; i < expected.Length; i++) {
            if (expected[i] != actual[i]) return false;
        }
        return true;
    }

    private byte[] GetSalt(string username) {
        byte[] bytes = new byte[0];

        string sql = "SELECT salt FROM users where username = @username";
        var cmd = new MySqlCommand(sql, this.sqlConnnection);
        cmd.Parameters.AddWithValue("@username", username);
        MySqlDataReader rdr = cmd.ExecuteReader();

        if (rdr.HasRows) {
            rdr.Read();
            string salt = rdr.GetString(0);
            bytes = Convert.FromBase64String(salt);
        }

        rdr.Close();
        return bytes;
    }

    public string GetEmail(string username) {
        string r = "";

        string sql = "SELECT email FROM users where username = @username";
        var cmd = new MySqlCommand(sql, this.sqlConnnection);
        cmd.Parameters.AddWithValue("@username", username);
        MySqlDataReader rdr = cmd.ExecuteReader();

        if (rdr.HasRows) {
            rdr.Read();
            r = rdr.GetString(0);            
        }

        rdr.Close();
        return r;
    }    

    public string GetStatus(string username) {
        string r = "";

        string sql = "SELECT status FROM users where username = @username";
        var cmd = new MySqlCommand(sql, this.sqlConnnection);
        cmd.Parameters.AddWithValue("@username", username);
        MySqlDataReader rdr = cmd.ExecuteReader();

        if (rdr.HasRows) {
            rdr.Read();
            r = rdr.GetString(0);            
        }

        rdr.Close();
        return r;
    }   

    private byte[] GetHash(string username) {
        byte[] bytes = new byte[0];

        string sql = "SELECT password FROM users where username = @username";
        var cmd = new MySqlCommand(sql, this.sqlConnnection);
        cmd.Parameters.AddWithValue("@username", username);
        MySqlDataReader rdr = cmd.ExecuteReader();

        if (rdr.HasRows) {
            rdr.Read();
            string hash = rdr.GetString(0);
            bytes = Convert.FromBase64String(hash);
        }

        rdr.Close();
        return bytes;
    }

    private int GetIterations(string username) {
        int r = 0;

        string sql = "SELECT iterations FROM users where username = @username";
        var cmd = new MySqlCommand(sql, this.sqlConnnection);
        cmd.Parameters.AddWithValue("@username", username);
        MySqlDataReader rdr = cmd.ExecuteReader();

        if (rdr.HasRows) {
            rdr.Read();
            r = rdr.GetInt32(0);
        }

        rdr.Close();
        return r;
    }

    public int RemoveUser(string username) {
        string sql = "DELETE FROM users WHERE username = @username";
        var cmd = new MySqlCommand(sql, this.sqlConnnection);

        cmd.Parameters.AddWithValue("@username", username);

        cmd.Prepare();
        return cmd.ExecuteNonQuery();
    }

    public bool HasUser(string username) {
        string sql = "SELECT * FROM users where username = @username";
        var cmd = new MySqlCommand(sql, this.sqlConnnection);
        cmd.Parameters.AddWithValue("@username", username);
        MySqlDataReader rdr = cmd.ExecuteReader();
        bool r = rdr.HasRows;
        rdr.Close();
        return r;
    }

    private byte[] GenerateSalt() {
        byte[] bytes = new byte[SALT_LENGTH];

        using (var rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(bytes);
            return bytes;
        }
    }

    // https://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp
    public static byte[] ApplySalt(byte[] plainText, byte[] salt) {
        byte[] salted = new byte[plainText.Length + salt.Length];

        for (int i = 0; i < plainText.Length; i++) salted[i] = plainText[i];
        for (int i = 0; i < salt.Length; i++) salted[plainText.Length + i] = salt[i];

        return salted;
    }

    private static byte[] Hash(byte[] unhashed, ref int iterations) {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        iterations = 1;

        using (SHA512 mySHA512 = SHA512.Create()) {
            byte[] hashed = mySHA512.ComputeHash(unhashed);

            while (stopwatch.ElapsedMilliseconds < REHASH_TIME_MS) {
                iterations++;
                hashed = mySHA512.ComputeHash(hashed);
            }

            return hashed;
        }
    }

    private static byte[] ReHash(byte[] unhashed, int iterations) {
        using (SHA512 mySHA512 = SHA512.Create()) {
            byte[] hashed = mySHA512.ComputeHash(unhashed);

            for (int i = 1; i < iterations; i++) {
                hashed = mySHA512.ComputeHash(hashed);
            }

            return hashed;
        }
    }

    public static bool ValidateUsername(string username){
        if (username.Length < 2) return false;
        if (username.Length > 16) return false;

        Regex regex = new Regex("^[A-Za-z0-9]+$");
        Match match = regex.Match(username);

        return match.Success;
    }

}