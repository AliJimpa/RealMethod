
namespace RealMethod
{
    public static class DoOnce
    {
        private static readonly NameTable<bool> Flags = new NameTable<bool>();
        public static bool Check(string name)
        {
            if (!Flags.ContainsKey(name))
                Flags.Add(name, false);

            if (Flags[name]) return false;

            Flags[name] = true;
            return true;
        }
        public static void Reset(string name)
        {
            if (Flags.ContainsKey(name))
            {
                Flags[name] = false;
            }
            else
            {
                Flags.Add(name, false);
            }
        }
    }

}