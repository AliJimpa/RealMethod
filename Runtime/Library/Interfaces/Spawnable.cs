using UnityEngine;

namespace RealMethod
{

    public interface ISpawn
    {
        void OnSpawn();
    }
    public interface ISpawnWithAuthor
    {
        void OnSpawn(Object author);
    }
    public interface ISpawnWithArgument<TArgument>
    {
        void OnSpawn(TArgument argument);
    }
    public interface ISpawnWithTwoArgument<TArgumentA, TArgumentB>
    {
        void OnSpawn(TArgumentA argumentA, TArgumentB argumentB);
    }
    public interface IDespawn
    {
        void OnDespawn();
    }
    public interface IDespawnWithAuthor
    {
        void OnDespawn(Object author);
    }



}


