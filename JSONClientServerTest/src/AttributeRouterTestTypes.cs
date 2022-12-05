using Microsoft.VisualStudio.TestTools.UnitTesting;
using frar.JSONServer;
using System.Net;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace frar.JSONClientServerTest;

[TestClass]
public class TestTypes : AttributeRouter {
    public string isString = "";
    public bool isBool = false;
    public byte isByte = 0;
    public sbyte isSByte = 0;
    public char isChar = 'a';
    public decimal isDecimal = 0.0M;
    public double isDouble = 0.0;
    public float isFloat = 0.0F;
    public int isInt = 0;
    public uint isUInt = 0;
    public nint isNInt = 0;
    public nuint isNUInt = 0;
    public long isLong = 0;
    public ulong isULong = 0;
    public short isShort = 0;
    public ushort isUShort = 0;

    public TestTypes() {
        this.Initialize();
    }

    // Invokes an action with a single string parameter.
    // The method uses a default route, case-insenstive full match.
    [TestMethod]
    public void String_Parameter() {
        this.Process(@"{
            'action' : 'setstring',
            'parameters' : {
                'value' : 'the_value'
            }
        }");

        Assert.AreEqual("the_value", this.isString);
    }

    // Invokes an action with a single byte parameter.
    // The method uses a default route, case-insenstive full match.
    [TestMethod]
    public void Byte_Parameter() {
        this.isByte = 0;
        this.Process(@"{
            'action' : 'setbyte',
            'parameters' : {
                'value' : 7
            }
        }");

        Assert.AreEqual(7, this.isByte);
    }

    // Invokes an action with a single byte parameter.
    // The method uses a default route, case-insenstive full match.
    // The json value is represented by a string.
    [TestMethod]
    public void String_as_Byte_Parameter() {
        this.isByte = 0;
        this.Process(@"{
            'action' : 'setbyte',
            'parameters' : {
                'value' : '7'
            }
        }");

        Assert.AreEqual(7, this.isByte);
    }


    // Invokes an action with a single signed byte parameter.
    // The method uses a default route, case-insenstive full match.
    [TestMethod]
    public void SByte_Parameter() {
        this.isSByte = 0;
        this.Process(@"{
            'action' : 'setsbyte',
            'parameters' : {
                'value' : -7
            }
        }");

        Assert.AreEqual(-7, this.isSByte);
    }

    // Invokes an action with a single signed byte parameter.
    // The method uses a default route, case-insenstive full match.
    // The json value is represented by a string.
    [TestMethod]
    public void String_as_SByte_Parameter() {
        this.isSByte = 0;
        this.Process(@"{
            'action' : 'setsbyte',
            'parameters' : {
                'value' : '-7'
            }
        }");

        Assert.AreEqual(-7, this.isSByte);
    }

    // Invokes an action with a single char parameter.
    // The method uses a default route, case-insenstive full match.
    [TestMethod]
    public void Char_Parameter() {
        this.isChar = 'a';
        this.Process(@"{
            'action' : 'setchar',
            'parameters' : {
                'value' : 'b'
            }
        }");

        Assert.AreEqual('b', this.isChar);
    }

    // Invokes an action with a single decimal parameter.
    // The method uses a default route, case-insenstive full match.
    [TestMethod]
    public void Decimal_Parameter() {
        this.isDecimal = 0.0M;
        this.Process(@"{
            'action' : 'setdecimal',
            'parameters' : {
                'value' : 5.6
            }
        }");

        Assert.AreEqual(5.6M, this.isDecimal);
    }

    // Invokes an action with a single decimal parameter.
    // The method uses a default route, case-insenstive full match.
    // The json value is represented by a string.
    [TestMethod]
    public void String_as_Decimal_Parameter() {
        this.isDecimal = 0.0M;
        this.Process(@"{
            'action' : 'setdecimal',
            'parameters' : {
                'value' : '5.6'
            }
        }");

        Assert.AreEqual(5.6M, this.isDecimal);
    }

    // Invokes an action with a single float parameter.
    // The method uses a default route, case-insenstive full match.
    [TestMethod]
    public void Float_Parameter() {
        this.isFloat = 0.0F;
        this.Process(@"{
            'action' : 'setFloat',
            'parameters' : {
                'value' : 5.6
            }
        }");

        Assert.AreEqual(5.6F, this.isFloat);
    }

    // Invokes an action with a single float parameter.
    // The method uses a default route, case-insenstive full match.
    // The json value is represented by a string.
    [TestMethod]
    public void String_as_Float_Parameter() {
        this.isFloat = 0.0F;
        this.Process(@"{
            'action' : 'setFloat',
            'parameters' : {
                'value' : '5.6'
            }
        }");

        Assert.AreEqual(5.6F, this.isFloat);
    }

    // Invokes an action with a single integer (int32) parameter.
    // The method uses a default route, case-insenstive full match.
    [TestMethod]
    public void Integer_Parameter() {
        this.isInt = 0;
        this.Process(@"{
            'action' : 'setint',
            'parameters' : {
                'value' : 3
            }
        }");

        Assert.AreEqual(3, this.isInt);
    }

    // Invokes an action with a single integer (int32) parameter.
    // The method uses a default route, case-insenstive full match.
    // The json value is represented by a string.
    [TestMethod]
    public void String_as_Integer_Parameter() {
        this.isInt = 0;
        this.Process(@"{
            'action' : 'setint',
            'parameters' : {
                'value' : '3'
            }
        }");

        Assert.AreEqual(3, this.isInt);
    }

    // Invokes an action with a single unsigned integer (int32) parameter.
    // The method uses a default route, case-insenstive full match.
    [TestMethod]
    public void Unsigned_Integer_Parameter() {
        this.isUInt = 0;
        this.Process(@"{
            'action' : 'setuint',
            'parameters' : {
                'value' : 7
            }
        }");

        Assert.AreEqual((uint)7, this.isUInt);
    }

    // Invokes an action with a single unsigned integer (int32) parameter.
    // The method uses a default route, case-insenstive full match.
    // The json value is represented by a string.
    [TestMethod]
    public void String_as_Unsigned_Integer_Parameter() {
        this.isUInt = 0;
        this.Process(@"{
            'action' : 'setuint',
            'parameters' : {
                'value' : '7'
            }
        }");

        Assert.AreEqual((uint)7, this.isUInt);
    }


    // Invokes an action with a single long (int64) parameter.
    // The method uses a default route, case-insenstive full match.
    [TestMethod]
    public void Long_Parameter() {
        this.isLong = 0;
        this.Process(@"{
            'action' : 'setlong',
            'parameters' : {
                'value' : 7
            }
        }");

        Assert.AreEqual(7, this.isLong);
    }

    // Invokes an action with a single long (int64) parameter.
    // The method uses a default route, case-insenstive full match.
    // The json value is represented by a string.
    [TestMethod]
    public void String_as_Long_Parameter() {
        this.isLong = 0;
        this.Process(@"{
            'action' : 'setlong',
            'parameters' : {
                'value' : '7'
            }
        }");

        Assert.AreEqual(7, this.isLong);
    }

    // Invokes an action with a single unsigned long (int64) parameter.
    // The method uses a default route, case-insenstive full match.
    [TestMethod]
    public void Unsigned_Long_Parameter() {
        this.isULong = 0;
        this.Process(@"{
            'action' : 'setulong',
            'parameters' : {
                'value' : 9
            }
        }");

        Assert.AreEqual((ulong)9, this.isULong);
    }

    // Invokes an action with a single unsigned long (int64) parameter.
    // The method uses a default route, case-insenstive full match.
    // The json value is represented by a string.
    [TestMethod]
    public void String_as_Unsigned_Long_Parameter() {
        this.isULong = 0;
        this.Process(@"{
            'action' : 'setulong',
            'parameters' : {
                'value' : '9'
            }
        }");

        Assert.AreEqual((ulong)9, this.isULong);
    }

    // Invokes an action with a single long (int64) parameter.
    // The method uses a default route, case-insenstive full match.
    [TestMethod]
    public void Bool_Parameter() {
        this.isBool = false;
        this.Process(@"{
            'action' : 'setbool',
            'parameters' : {
                'value' : true
            }
        }");
        Assert.AreEqual(true, this.isBool);

        this.Process(@"{
            'action' : 'setbool',
            'parameters' : {
                'value' : false
            }
        }");
        Assert.AreEqual(false, this.isBool);
    }

    // Invokes an action with a single long (int64) parameter.
    // The method uses a default route, case-insenstive full match.
    // The json value is represented by a string
    [TestMethod]
    public void String_as_Bool_Parameter() {
        this.isBool = false;
        this.Process(@"{
            'action' : 'setbool',
            'parameters' : {
                'value' : 'true'
            }
        }");
        Assert.AreEqual(true, this.isBool);
    }

    // Invokes an action with a single Short (int64) parameter.
    // The method uses a default route, case-insenstive full match.
    [TestMethod]
    public void Short_Parameter() {
        this.isShort = 0;
        this.Process(@"{
            'action' : 'setShort',
            'parameters' : {
                'value' : 7
            }
        }");

        Assert.AreEqual((short)7, this.isShort);
    }

    // Invokes an action with a single Short (int64) parameter.
    // The method uses a default route, case-insenstive full match.
    // The json value is represented by a string.
    [TestMethod]
    public void String_as_Short_Parameter() {
        this.isShort = 0;
        this.Process(@"{
            'action' : 'setShort',
            'parameters' : {
                'value' : '7'
            }
        }");

        Assert.AreEqual((short)7, this.isShort);
    }

    // Invokes an action with a single unsigned Short (int64) parameter.
    // The method uses a default route, case-insenstive full match.
    [TestMethod]
    public void Unsigned_Short_Parameter() {
        this.isUShort = 0;
        this.Process(@"{
            'action' : 'setuShort',
            'parameters' : {
                'value' : 9
            }
        }");

        Assert.AreEqual((ushort)9, this.isUShort);
    }

    // Invokes an action with a single unsigned Short (int64) parameter.
    // The method uses a default route, case-insenstive full match.
    // The json value is represented by a string.
    [TestMethod]
    public void String_as_Unsigned_Short_Parameter() {
        this.isUShort = 0;
        this.Process(@"{
            'action' : 'setuShort',
            'parameters' : {
                'value' : '9'
            }
        }");

        Assert.AreEqual((ushort)9, this.isUShort);
    }

    [Route]
    public void SetString(string value) {
        this.isString = value;
    }

    [Route]
    public void SetInt(int value) {
        this.isInt = value;
    }

    [Route]
    public void SetUInt(uint value) {
        this.isUInt = value;
    }

    [Route]
    public void SetShort(short value) {
        this.isShort = value;
    }

    [Route]
    public void SetUShort(ushort value) {
        this.isUShort = value;
    }

    [Route]
    public void SetNInt(nint value) {
        this.isNInt = value;
    }

    [Route]
    public void SetNUInt(nuint value) {
        this.isNUInt = value;
    }

    [Route]
    public void SetLong(long value) {
        this.isLong = value;
    }

    [Route]
    public void SetULong(ulong value) {
        this.isULong = value;
    }

    [Route]
    public void SetBool(bool value) {
        this.isBool = value;
    }

    [Route]
    public void SetByte(byte value) {
        this.isByte = value;
    }

    [Route]
    public void SetSByte(sbyte value) {
        this.isSByte = value;
    }

    [Route]
    public void SetChar(char value) {
        this.isChar = value;
    }

    [Route]
    public void SetDecimal(decimal value) {
        this.isDecimal = value;
    }

    [Route]
    public void SetDouble(double value) {
        this.isDouble = value;
    }

    [Route]
    public void SetFloat(float value) {
        this.isFloat = value;
    }
}