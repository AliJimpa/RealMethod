using UnityEngine;

namespace RealMethod
{
	public abstract class GizmoManager : MonoBehaviour, IGameManager
	{
		protected abstract class GUIRenderer
		{
			public abstract void Start(GizmoManager Manager);
			public abstract bool CanRender();
			public abstract void Draw();
		}

		[Header("GUI")]
		[SerializeField] private Vector2 defualtposition = new Vector2(10, 10);
		public Vector2 DefualtPosition => defualtposition;


		private GUIRenderer[] RenderBox;

		//IGameManager Interface
		MonoBehaviour IGameManager.GetManagerClass()
		{
			return this;
		}
		void IGameManager.InitiateManager(bool AlwaysLoaded)
		{
			if (!AlwaysLoaded)
			{
				Debug.LogError("You can't use GUIManager in [World] Scope");
				Destroy(this);
			}

			RenderBox = GetRenderSlots();
			foreach (var Renderer in RenderBox)
			{
				Renderer.Start(this);
			}
		}


		private void OnGUI()
		{
			foreach (var Renderer in RenderBox)
			{
				if (Renderer.CanRender())
				{
					Renderer.Draw();
				}
			}
		}


		// Abstract Methods
		public abstract void ResolveService(Service service, bool active);
		protected abstract GUIRenderer[] GetRenderSlots();


	}
}

