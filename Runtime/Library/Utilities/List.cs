namespace RealMethod
{
    static class List
    {
        // Method to check if an index is valid
        public static bool IsValidIndex<T>(System.Collections.Generic.List<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }
    }
}