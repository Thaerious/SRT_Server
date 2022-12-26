using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace frar.clientserver.test;

/// <summary>
/// Methods in parent classes get called from the child instance.
/// </summary>
[TestClass]
public class StaticRouterTest {
    [TestMethod]
    public void Set_J(){
        var router = new StaticChild();
        router.Process(
            new Packet("setj")
            .Set("value", 3)
        );
        Assert.AreEqual(6, StaticChild.j);
    }
}
// Parent in RouterTest.cs
public class StaticChild : RouterImpl {
    public static int j = 0;

    // This overrides the parent SetInt
    [Route]
    public static void SetJ(int value) {
        StaticChild.j = value * 2;
    }    
}