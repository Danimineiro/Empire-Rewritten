using Verse;

namespace Empire_Rewritten
{
    [StaticConstructorOnStartup]
    public class TestClass
    {
        static TestClass()
        {
            Log.Message($"<color=orange>[Empire] Test succeeded ^-^</color>");
        }
    }
}