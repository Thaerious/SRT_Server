using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace frar.clientserver.test;

/// <summary>
/// Methods in parent classes get called from the child instance.
/// </summary>
[TestClass]
public class InheritedRouterTest {

    [TestMethod]
    public void Set_Int(){
        var router = new Child();
        router.Process(
            new Packet("setint")
            .Set("value", 3)
        );
        Assert.AreEqual(6, router.j);
        Assert.AreEqual(0, router.i);
    }

    [TestMethod]
    public void Set_J(){
        var router = new Child();
        router.Process(
            new Packet("setj")
            .Set("value", 3)
        );
        Assert.AreEqual(6, router.j);
    }

    [TestMethod]
    public void Set_Object(){
        var router = new Child();
        router.Process(
            new Packet("setsimple")
            .Set("value", new SimpleObject(2, 5))
        );
        Assert.AreEqual(2, router?.simple?.a);
    }

    [TestMethod]
    public void Set_Array(){
        var router = new Child();
        router.Process(
            new Packet("setarray")
            .Set("value", new int[]{3, 5, 7})
        );
        Assert.AreEqual(5, router?.array?[1]);
    }
}
// Parent in RouterTest.cs
public class Child : RouterImpl {
    public int j = 0;

    // This overrides the parent SetInt
    [Route]
    public override void SetInt(int value) {
        this.j = value * 2;
    }    

    [Route]
    private void SetJ(int value) {
        this.j = value * 2;
    }    
}