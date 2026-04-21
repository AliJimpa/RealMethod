namespace RealMethod
{
    public static class MessageNames
    {
        // Lifecycle
        public const string Spawn = "OnSpawn";
        public const string Despawn = "OnDespawn";
        public const string Die = "OnDie";

        // Combat
        public const string ApplyDamage = "OnTakeDamage";

        // Attachment
        public const string Attach = "OnAttach";
        public const string Detach = "OnDetach";

        // Save
        public const string Save = "OnSave";
        public const string Load = "Onload";
    }

    public static class FunctionNames
    {
        public const string AssetPermission = "EnsureAssetPermission";
        public const string Reset = "Reset";
    }
}