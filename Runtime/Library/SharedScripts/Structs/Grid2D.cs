
using UnityEngine;

namespace RealMethod
{
    public struct Grid2D
    {
        private float GridSize;
        private int GridScaleRatio;
        private Vector2Int WorldOffset;


        public Grid2D(float gridsize, int gridscaleratio, Vector2Int worldoffset)
        {
            GridSize = gridsize;
            GridScaleRatio = gridscaleratio;
            WorldOffset = worldoffset;
        }


        // Section Position / ID
        public string GetSectionByPose(Vector2 Targetposition)
        {
            // Calculate the chunk coordinates
            int chunkX = Mathf.FloorToInt(Targetposition.x / GetSize());
            int chunkY = Mathf.FloorToInt(Targetposition.y / GetSize());

            // Generate a unique chunk ID
            string chunkId = $"Chunk_{chunkX}_{chunkY}";
            return chunkId;
        }
        public Vector2Int GetSectionByPose(Vector3 Targetposition)
        {
            // Calculate the chunk coordinates
            int chunkX = Mathf.FloorToInt(Targetposition.x / GetSize());
            int chunkY = Mathf.FloorToInt(Targetposition.z / GetSize());

            // Generate a unique chunk ID
            return new Vector2Int(chunkX, chunkY);
        }


        // Grid Size
        public float GetSize()
        {
            return GridSize * GridScaleRatio;
        }

        // Get GridScaleRatio
        public int GetScaleRatio()
        {
            return GridScaleRatio;
        }


        // GetWorld Position from Sectio
        public Vector2 GetWorldPosition(Vector2Int Section, bool Center = false)
        {
            if (Center)
            {
                return new Vector2(Section.x * GetSize() + GetSize() / 2, Section.y * GetSize() + GetSize() / 2);
            }
            else
            {
                return new Vector2(Section.x * GetSize(), Section.y * GetSize());
            }

        }


        // Gizmo
        public static void DrawGrid(Grid2D Target)
        {
            Gizmos.DrawLine(-Vector3.right * Target.GetSize(), Vector3.right * Target.GetSize());
            for (int x = 0; x <= 10; x++)
            {

            }
            for (int y = 0; y <= 10; y++)
            {
                //Gizmos.DrawLine(new Vector3(0, 0, y * GridSize), new Vector3(GetSize() * GridSize, 0, y * GridSize));
            }

        }

    }
}

