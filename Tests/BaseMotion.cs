using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using RealMethod;

// public class MotionPlayer : MonoBehaviour
// {
//     public enum MotionPlayMode
//     {
//         Pause = 0,
//         Forward = 1,
//         Backward = -1,
//     }
//     public enum MotionMethod
//     {
//         None = 0,
//         LateUpdate = 2,
//         Update = 1,
//         FixedUpdate = 3,
//         Coroutine = 4
//     };


//     [Header("Asset")]
//     [SerializeField]
//     private bool InstantiateAsset = true;
//     [SerializeField]
//     private MotionAsset motion;
//     [SerializeField, ConditionalHide("Optimize", true, true)]
//     private float startTime = 0;

//     [Header("Setting")]
//     [SerializeField]
//     private bool Optimize = true;
//     [SerializeField]
//     private MotionMethod method = MotionMethod.Update;
//     [SerializeField]
//     private MotionPlayMode PlayMode = MotionPlayMode.Pause;
//     [SerializeField, ConditionalHide("Optimize", true, true)]
//     private float Speed = 1;
//     [SerializeField]
//     private float AdditiveRandomTime = 0;

//     [Header("Advance")]
//     [SerializeField, ShowOnly]
//     private float elapsedTime = 0;

//     [Header("Events")]
//     public UnityEvent<float> FloatMotion;
//     public UnityEvent<Vector3> VectorMotion;
//     public UnityEvent<Quaternion> RotationMotion;
//     public UnityEvent<Transform> TransformMotion;
//     public UnityEvent<Color> ColorMotion;


//     private bool IsInCoroutine = false;
//     private MotionAsset TargetMotion;
//     private Action CallBackFunctionArchive;


//     private void Awake()
//     {
//         if (InstantiateAsset)
//         {
//             TargetMotion = Instantiate(motion);
//         }
//         else
//         {
//             if (motion)
//             {
//                 TargetMotion = motion;
//             }
//             else
//             {
//                 Debug.LogError("MotionPlayer Can't Play Motion Without Asset");
//                 PlayMode = MotionPlayMode.Pause;
//             }
//         }

//         if (TargetMotion == null)
//         {
//             Debug.LogError("MotionPlayer Can't Play Motion Without Asset");
//             PlayMode = MotionPlayMode.Pause;
//             return;
//         }

//         if (!TargetMotion.Prepare(this))
//         {
//             Debug.LogError("MotionPlayer Can't Play Motion Without Asset");
//             motion = null;
//             TargetMotion = null;
//             PlayMode = MotionPlayMode.Pause;
//         }

//         AdditiveRandomTime = UnityEngine.Random.Range(0, AdditiveRandomTime);
//     }
//     private void Start()
//     {
//         if (PlayMode != MotionPlayMode.Pause)
//         {
//             PlayFB(PlayMode == MotionPlayMode.Forward);
//         }
//     }
//     private void LateUpdate()
//     {
//         if (method == MotionMethod.LateUpdate && IsPlaying())
//         {
//             UpdateTimeline();
//         }
//     }
//     private void Update()
//     {
//         if (method == MotionMethod.Update && IsPlaying())
//         {
//             UpdateTimeline();
//         }
//         if (method == MotionMethod.Coroutine && !IsInCoroutine && IsPlaying())
//         {
//             IsInCoroutine = true;
//             StartCoroutine(LerpUpdate());
//         }
//     }
//     private void FixedUpdate()
//     {
//         if (method == MotionMethod.FixedUpdate && IsPlaying())
//         {
//             UpdateTimeline();
//         }
//     }
//     private void OnDestroy()
//     {
//         if (TargetMotion != null)
//         {
//             TargetMotion.StopMotion();
//             TargetMotion.CleanMotion();
//         }
//     }

//     public bool IsPlaying()
//     {
//         return PlayMode != MotionPlayMode.Pause;
//     }
//     public MotionPlayMode GetCurrentMode()
//     {
//         return PlayMode;
//     }
//     public MotionAsset GetMotionAsset()
//     {
//         return TargetMotion;
//     }
//     public bool IsInstantiated()
//     {
//         return InstantiateAsset;
//     }

//     public bool SetMotion(MotionAsset Asset, float BeginTime = 0)
//     {
//         if (IsPlaying())
//             Pause();

//         if (TargetMotion)
//             TargetMotion.CleanMotion();

//         if (TargetMotion.Prepare(this))
//         {
//             if (InstantiateAsset)
//             {
//                 TargetMotion = Instantiate(Asset);
//             }
//             else
//             {
//                 TargetMotion = Asset;
//             }
//             motion = Asset;
//             return true;
//         }
//         else
//         {
//             motion = null;
//             TargetMotion = null;
//             PlayMode = MotionPlayMode.Pause;
//             return false;
//         }
//     }
//     public void Play(bool Condition = true)
//     {
//         if (Condition)
//         {
//             PlayForward();
//         }
//         else
//         {
//             Pause();
//         }
//     }
//     public void PlayFB(bool forward = true)
//     {
//         if (forward)
//         {
//             PlayForward();
//         }
//         else
//         {
//             PlayBackward();
//         }
//     }
//     public void PlayForward()
//     {
//         TargetMotion.PreStartMotion(true);
//         TargetMotion.StartMotion();
//         TargetMotion.OnMotionActivate?.Invoke(true);
//         if (Optimize)
//         {
//             startTime = Time.time;
//         }
//         else
//         {
//             elapsedTime = startTime;
//         }
//         PlayMode = MotionPlayMode.Forward;
//     }
//     public void PlayBackward()
//     {
//         TargetMotion.PreStartMotion(false);
//         TargetMotion.StartMotion();
//         TargetMotion.OnMotionActivate?.Invoke(true);
//         if (Optimize)
//         {
//             startTime = Time.time;
//         }
//         else
//         {
//             elapsedTime = startTime;
//         }
//         PlayMode = MotionPlayMode.Backward;
//     }
//     public void Play(bool forward, Action callback)
//     {
//         CallBackFunctionArchive = callback;
//         PlayFB(forward);
//     }
//     public void Play(bool forward, float time)
//     {
//         if (!Optimize)
//         {
//             startTime = time;
//         }
//         else
//         {
//             Debug.LogWarning("Time is not used in Optimize Mode, Use StartTime instead");
//         }
//         PlayFB(forward);
//     }
//     public void ResetPlay(MotionAsset Asset, float BeginTime = 0, bool forward = true)
//     {
//         SetMotion(Asset, BeginTime);
//         PlayFB(forward);
//     }
//     public void Pause()
//     {
//         PlayMode = MotionPlayMode.Pause;
//         TargetMotion.StopMotion();
//         TargetMotion.OnMotionActivate?.Invoke(false);
//         CallBackFunctionArchive?.Invoke();
//     }
//     public void InvertPlayeMode()
//     {
//         switch (PlayMode)
//         {
//             case MotionPlayMode.Forward:
//                 TargetMotion.PreStartMotion(false);
//                 PlayMode = MotionPlayMode.Backward;
//                 break;
//             case MotionPlayMode.Backward:
//                 TargetMotion.PreStartMotion(true);
//                 PlayMode = MotionPlayMode.Forward;
//                 break;
//             default:
//                 Debug.LogWarning("PlayMode is not in Currect Mode for Inverting.");
//                 break;
//         }
//     }


//     private void UpdateTimeline()
//     {
//         switch (PlayMode)
//         {
//             case MotionPlayMode.Pause:
//                 Debug.LogError("The Component Should not Run Timeline in Pause Mode!");
//                 break;
//             case MotionPlayMode.Forward:
//                 if (Optimize)
//                 {
//                     elapsedTime = Time.time - startTime;
//                 }
//                 else
//                 {
//                     elapsedTime += Time.deltaTime * Speed;
//                 }
//                 break;
//             case MotionPlayMode.Backward:
//                 if (Optimize)
//                 {
//                     elapsedTime = startTime - Time.time;
//                 }
//                 else
//                 {
//                     elapsedTime -= Time.deltaTime * Speed;
//                 }
//                 break;
//         }
//         TargetMotion.UpdateMotion(elapsedTime + AdditiveRandomTime);
//     }

//     private IEnumerator LerpUpdate()
//     {
//         while (PlayMode != MotionPlayMode.Pause)
//         {
//             UpdateTimeline();
//             yield return null;
//         }
//         IsInCoroutine = false;
//     }



// }



// [Serializable]
// public abstract class MotionAsset : ScriptableObject
// {
//     public Action OnMotionPrepare;
//     public Action<bool> OnMotionActivate;
//     public abstract bool Prepare(MotionPlayer Owner);
//     public abstract void PreStartMotion(bool IsForward);
//     public abstract void StartMotion();
//     public abstract void UpdateMotion(float Time);
//     public abstract void StopMotion();
//     public abstract void CleanMotion();
// }

// public abstract class MotionAsset<T> : MotionAsset
// {
//     [Serializable]
//     public class MotionNotify
//     {
//         public float Percentage;
//         public string MethodName;
//         private bool Called;
//         public SendMessageOptions CheckMessageSended;

//         public bool IsCalled()
//         {
//             return Called;
//         }
//         public void NotifyCall(MotionPlayer Target)
//         {
//             if (!Called)
//             {
//                 Called = true;
//                 switch (MethodName)
//                 {
//                     case "PlayForward":
//                         Target.Play();
//                         break;
//                     case "Invert":
//                         Target.InvertPlayeMode();
//                         break;
//                     case "Pause":
//                         Target.Pause();
//                         break;
//                     case "PlayBackward":
//                         Target.Play(false);
//                         break;
//                     case "Active":
//                         Target.gameObject.SetActive(true);
//                         break;
//                     case "Deactive":
//                         Target.gameObject.SetActive(false);
//                         break;
//                     case "Print":
//                         Debug.Log($"Percentage = {Percentage}");
//                         break;
//                     case "Reset":
//                         Target.SetMotion(Target.GetMotionAsset(), 0);
//                         break;
//                     default:
//                         Target.gameObject.SendMessage(MethodName, CheckMessageSended);
//                         break;
//                 }
//             }
//             else
//             {
//                 Debug.LogWarning("$ The Notify with this  - {tag} -  called befor");
//             }
//         }
//         public void NotifyReset()
//         {
//             Called = false;
//         }
//     }

//     [Header("Motion")]
//     [SerializeField]
//     protected T StartPose;
//     [SerializeField]
//     protected T EndPose;
//     [SerializeField, ConditionalHide("infinite", false, true)]
//     private float Duration = 1f;
//     [SerializeField]
//     private float TimeOffset = 0;
//     [Header("Options")]
//     [SerializeField]
//     protected bool infinite;
//     [SerializeField, ConditionalHide("AutoInvert", false, true)]
//     protected bool AutoPause = false;
//     [SerializeField, ConditionalHide("AutoPause", false, true)]
//     protected bool AutoInvert = false;
//     [SerializeField, ConditionalHide("AutoInvert", true, false)]
//     protected bool Once = false;
//     [SerializeField, ConditionalHide("Once", true, true)]
//     protected bool Reversible = false;
//     [SerializeField]
//     protected bool Absolute = true;
//     [SerializeField, ConditionalHide("Reversible", true, true)]
//     protected bool MinesOne = false;
//     [SerializeField]
//     protected bool Clamp = false;
//     [SerializeField]
//     protected bool Sin = false;
//     [SerializeField]
//     protected bool Cos = false;
//     [SerializeField]
//     protected bool UseCurve = false;
//     [SerializeField, ConditionalHide("UseCurve", true, false)]
//     protected AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
//     [SerializeField, Tooltip("Some Tag Is Implemented by default 'PlayForward'-'Invert'-'Pause'-'PlayBackward'-'Active'-'Deactive'-'Reset'-'Print' ")]
//     protected MotionNotify[] notifies;
//     [Space, SerializeField]
//     protected bool ShowPreview = false;
//     [SerializeField, ConditionalHide("ShowPreview", true, false), ShowOnly]
//     private float Alpha = 0;
//     [SerializeField, ConditionalHide("ShowPreview", true, false), ShowOnly]
//     private T result;


//     public Action<T> OnMotionUpdated;

//     protected MotionPlayer MyOwner { get; private set; }


//     protected abstract T Interpolation(float Alpha);


//     public override bool Prepare(MotionPlayer Owner)
//     {
//         if (MyOwner != null)
//         {
//             Debug.LogError("MotionAsset is already prepared by another MotionPlayer");
//             return false;
//         }

//         MyOwner = Owner;
//         result = StartPose;
//         OnMotionPrepare?.Invoke();
//         return true;
//     }
//     public override void UpdateMotion(float Time)
//     {
//         Alpha = (Time + TimeOffset) / Duration;
//         CheckOptions(Time);
//         result = Interpolation(Alpha);
//         OnMotionUpdated?.Invoke(result);
//     }
//     public override void PreStartMotion(bool IsForward)
//     {
//         foreach (var item in notifies)
//         {
//             item.NotifyReset();
//         }
//     }
//     public override void CleanMotion()
//     {
//         MyOwner = null;
//         Alpha = 0;
//     }

//     public T GetResult()
//     {
//         return result;
//     }
//     public float GetDuration()
//     {
//         return Duration;
//     }
//     public void ForceStop()
//     {
//         MyOwner.Pause();
//     }

//     private void CheckOptions(float time)
//     {
//         if (infinite)
//         {
//             Alpha = time;
//         }

//         if (AutoPause && Math.Abs(time) >= Duration)
//         {
//             ForceStop();
//             return;
//         }

//         if (AutoInvert && Math.Abs(time) >= Duration)
//         {
//             MyOwner.InvertPlayeMode();
//         }

//         if (Once && time < 0)
//         {
//             ForceStop();
//             return;
//         }

//         if (Reversible && time < 0)
//         {
//             Alpha = 1 + Alpha;
//         }

//         if (Absolute)
//         {
//             Alpha = Math.Abs(Alpha);
//         }

//         if (MinesOne)
//         {
//             Alpha = 1 - Alpha;
//         }

//         if (Clamp)
//         {
//             Alpha = Mathf.Clamp(Alpha, 0, 1);
//         }

//         if (Sin)
//         {
//             Alpha = Mathf.Abs(MathF.Sin(Alpha * 90 * Mathf.Deg2Rad));
//         }

//         if (Cos)
//         {
//             Alpha = Mathf.Abs(MathF.Cos(Alpha * 90 * Mathf.Deg2Rad));
//         }

//         if (UseCurve)
//         {
//             Alpha = Curve.Evaluate(Alpha);
//         }

//         if (notifies.Length > 0)
//         {
//             SendNotify(Math.Abs(Alpha));
//         }
//     }
//     private void SendNotify(float alpha)
//     {
//         foreach (var item in notifies)
//         {
//             if (alpha >= (Math.Abs(item.Percentage) / 100) && !item.IsCalled())
//             {
//                 if (item.Percentage < 0 && MyOwner.GetCurrentMode() == MotionPlayer.MotionPlayMode.Backward)
//                 {
//                     item.NotifyCall(MyOwner);
//                 }

//                 if (item.Percentage > 0 && MyOwner.GetCurrentMode() == MotionPlayer.MotionPlayMode.Forward)
//                 {
//                     item.NotifyCall(MyOwner);
//                 }

//             }
//         }
//     }

// }





