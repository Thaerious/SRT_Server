
public class Foo{
    public int pub = 1;
    private int pri = 1;

    public Foo(int v){
        pub = v;
        pri = 2 * v;
    }

    public override string ToString(){
        return "I am foo " + pub + ", " + pri;
    }
}
