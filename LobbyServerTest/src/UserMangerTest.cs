using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using frar.LobbyServer;

namespace LobbyServerTest;

[TestClass]
public class UserManagerTest {
    [ClassInitialize]
    public static void before_all(TestContext testContext){
        UserManager.REHASH_TIME_MS = 1;
    }

    [TestMethod]
    public void sanity_test() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        userManager.Disconnect();
    }

    [TestMethod]
    public void add_remove_user() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        userManager.AddUser("username", "password", "email");
        Assert.IsTrue(userManager.HasUser("username"));
        userManager.RemoveUser("username");
        Assert.IsFalse(userManager.HasUser("username"));
        userManager.Disconnect();
    }

    // RemoveUser method returns number of rows affected.
    [TestMethod]
    public void add_remove_user_removes_1_row() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        userManager.AddUser("username", "password", "email");
        Assert.IsTrue(userManager.HasUser("username"));
        int r = userManager.RemoveUser("username");
        Assert.AreEqual(1, r);
        userManager.Disconnect();
    }

    // RemoveUser method returns number of rows affected.
    [TestMethod]
    public void add_user_twice_removes_1_row() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        userManager.AddUser("username", "password", "email");
        userManager.AddUser("username", "password", "email");
        Assert.IsTrue(userManager.HasUser("username"));
        int r = userManager.RemoveUser("username");
        Assert.AreEqual(1, r);
        userManager.Disconnect();
    }

    // Adduser returns true / false
    [TestMethod]
    public void add_user_twice_returns_true_when_new() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        bool r1 = userManager.AddUser("username", "password", "email");
        bool r2 = userManager.AddUser("username", "password", "email");
        Assert.IsTrue(r1);
        Assert.IsFalse(r2);
        userManager.Disconnect();
    }    

    [TestMethod]
    public void not_has_user() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        Assert.IsFalse(userManager.HasUser("unknown"));
        userManager.Disconnect();
    }

    [TestMethod]
    public void verify_user_pass() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        userManager.AddUser("bilbo", "the_one_ring", "billy@the_shire.com");
        var actual = userManager.VerifyUser("bilbo", "the_one_ring");
        Assert.IsTrue(actual);
        userManager.RemoveUser("bilbo");
        userManager.Disconnect();
    }        

    [TestMethod]
    public void verify_user_fail() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        userManager.AddUser("bilbo", "the_one_ring", "billy@the_shire.com");
        var actual = userManager.VerifyUser("bilbo", "the_tower");
        Assert.IsFalse(actual);
        userManager.RemoveUser("bilbo");
        userManager.Disconnect();
    }       

    [TestMethod]
    public void verify_user_unknown_user() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        userManager.AddUser("bilbo", "the_one_ring", "billy@the_shire.com");
        var actual = userManager.VerifyUser("blargo", "the_one_ring");
        Assert.IsFalse(actual);
        userManager.RemoveUser("bilbo");
        userManager.Disconnect();
    }  

    [TestMethod]
    public void update_email() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        userManager.AddUser("bilbo", "the_one_ring", "billy@the_shire.com");        
        bool r = userManager.UpdateEmail("bilbo", "b@gmail.com");
        var actual = userManager.GetEmail("bilbo");

        Assert.IsTrue(r);
        Assert.AreEqual(actual, "b@gmail.com");
        
        userManager.RemoveUser("bilbo");
        userManager.Disconnect();
    }       

    [TestMethod]
    public void update_status() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        userManager.AddUser("bilbo", "the_one_ring", "billy@the_shire.com");        
        bool r = userManager.UpdateStatus("bilbo", "verified");
        var actual = userManager.GetStatus("bilbo");

        Assert.IsTrue(r);
        Assert.AreEqual(actual, "verified");

        userManager.RemoveUser("bilbo");
        userManager.Disconnect();
    }     
        
    [TestMethod]
    public void update_pass() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        userManager.AddUser("bilbo", "the_one_ring", "billy@the_shire.com");

        bool r = userManager.UpdatePassword("bilbo", "second_breakfast");        
        var actual = userManager.VerifyUser("bilbo", "second_breakfast");

        Assert.IsTrue(r);
        Assert.IsTrue(actual);

        userManager.RemoveUser("bilbo");
        userManager.Disconnect();
    }           

    [TestMethod]
    public void update_email_unknown_user() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        userManager.AddUser("bilbo", "the_one_ring", "billy@the_shire.com");        
        bool r = userManager.UpdateEmail("balbu", "b@gmail.com");
        
        Assert.IsFalse(r);
        
        userManager.RemoveUser("bilbo");
        userManager.Disconnect();
    }       

    [TestMethod]
    public void update_status_unknown_user() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        userManager.AddUser("bilbo", "the_one_ring", "billy@the_shire.com");        
        bool r = userManager.UpdateStatus("balbu", "verified");

        Assert.IsFalse(r);

        userManager.RemoveUser("bilbo");
        userManager.Disconnect();
    }     
        
    [TestMethod]
    public void update_pass_unknown_user() {
        UserManager userManager = new UserManager();
        userManager.Connect("../../../config.json");
        userManager.AddUser("bilbo", "the_one_ring", "billy@the_shire.com");

        bool r = userManager.UpdatePassword("balbu", "second_breakfast");        

        Assert.IsFalse(r);
        
        userManager.RemoveUser("bilbo");
        userManager.Disconnect();
    }      
}