using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class LevelManager : MonoBehaviour, IGameManager
    {

        [Header("Generation")]
        //[SerializeField]
        //private NlevelGenerator LG;
        [SerializeField]
        private Vector2Int[] Filter = new Vector2Int[16];
        //public ChunkPack[] DifficultyPacks;



        [Header("WelcomStage")]
        [SerializeField]
        private GameObject WelcomChunk;


        //public Action<Chunkinfo> PlayerChunckUpdate;

        private Vector2Int PlayrLocation;
        private float ChunkSize;
        private GameObject Myplayer;




        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        public void InitiateManager(bool AlwaysLoaded)
        {
            // if (!LG)
            // {
            //     Debug.LogError("levelGenerator Field is Empty");
            //     return;
            // }

            //LG.Initialize();
            Myplayer = Game.World.GetPlayerObject();
            //ChunkSize = LG.GetChunkSize();
            PlayrLocation = GetGridLocation(Myplayer.transform);
            if (WelcomChunk == null)
            {
                Filter = new Vector2Int[0];
            }
            StartCoroutine(OnPlayerLocationChange());

        }
        public void InitiateService(Service service)
        {
            throw new System.NotImplementedException();
        }


        private void OnEnable()
        {
            //Game.CastWorld<CCWorld>().OndifficultyChange += OnLevelChange;
        }
        private void OnDisable()
        {
            //Game.CastWorld<CCWorld>().OndifficultyChange -= OnLevelChange;
        }
        private void Update()
        {
            Vector2Int NewLocation = GetGridLocation(Myplayer.transform);
            if (NewLocation != PlayrLocation)
            {
                PlayrLocation = NewLocation;
                StartCoroutine(OnPlayerLocationChange());
            }
        }


        private void OnLevelChange(int newlevel)
        {
            // if ((newlevel - 1) < DifficultyPacks.Length)
            // {
            //     LG.SetNewPakage(DifficultyPacks[newlevel - 1]);
            // }

        }
        private Vector2Int GetGridLocation(Transform WorldLocation)
        {
            float LocX = Mathf.Floor(WorldLocation.position.x / ChunkSize);
            float LocY = Mathf.Floor(WorldLocation.position.z / ChunkSize);
            return new Vector2Int((int)LocX, (int)LocY);
        }

        private IEnumerator OnPlayerLocationChange()
        {
            Hash128 HashLocation = Utility.Vector2ConvertToHash(PlayrLocation);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector2Int offsetlocation = new Vector2Int(i, j);
                    //StartCoroutine(GenerateIndex(offsetlocation + PlayrLocation));
                    Vector2Int Location = offsetlocation + PlayrLocation;

                    bool Result = true;
                    foreach (var filt in Filter)
                    {
                        if (Location == filt)
                        {
                            Result = false;
                        }
                    }

                    if (Result)
                    {
                        //LG.Generate(Location, true);
                        yield return new WaitForEndOfFrame();
                    }
                }
            }


            List<Hash128> Removelist = new List<Hash128>();
            // foreach (var item in LG.GetChunks())
            // {
            //     //GameObject ChunckValue = item.Value.GetLastRequest().gameObject;
            //     Vector2Int ChunkLocation = Utility.Vector3ToVector2Int(item.Value.transform.position / ChunkSize);
            //     ///Vector2Int ChunkLocation = Mustard.Utility.Vector3ToVector2Int(ChunckValue.transform.position / ChunkSize);
            //     float distnce = Vector2Int.Distance(ChunkLocation, PlayrLocation);
            //     if (distnce > 3)
            //     {
            //         Removelist.Add(item.Key);
            //     }
            // }



            //LG.Destroy(Removelist);


            if (WelcomChunk != null)
            {
                if (Vector2Int.Distance(Vector2Int.zero, GetGridLocation(Myplayer.transform)) > 2)
                {
                    WelcomChunk.SetActive(false);
                }
                else
                {
                    WelcomChunk.SetActive(true);
                }
            }


            //Chunkinfo TargetChunk = LG.GetChunks()[HashLocation].GetLastRequest();
            //GameObject TargetChunk;
            // if (LG.GetChunk(HashLocation, out TargetChunk))
            // {
            //     PlayerChunckUpdate?.Invoke(TargetChunk.GetComponent<Chunkinfo>());
            // }



        }

    }
}
