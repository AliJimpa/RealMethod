namespace RealMethod
{
    public static class Method_Extension
    {
        public static bool IsGameScope(this Scope target)
        {
            return target is Game;
        }
        public static bool IsWorldScope(this Scope target)
        {
            return target is World;
        }

    }
}