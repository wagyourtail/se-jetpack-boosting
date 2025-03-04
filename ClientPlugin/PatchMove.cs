using System;
using System.Threading;
using HarmonyLib;
using Sandbox.Engine.Utils;
using Sandbox.Game;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.World;
using VRage.Game;
using VRage.Input;
using VRageMath;

namespace wagyourtail.JetpackBoosting
{
    
    [HarmonyPatch(typeof(MyCharacter), "MoveAndRotate")]
    public class PatchMove
    {
        private static float SQRT_3 = (float) Math.Sqrt(3);
        
        static void Prefix(MyCharacter __instance, ref Vector3 moveIndicator)
        {
            try
            {
                if (__instance.CurrentMovementState != MyCharacterMovementEnum.Flying) return;

                var worldMatrixInv = __instance.PositionComp.WorldMatrixNormalizedInv;
                var velocity = __instance.Physics.LinearVelocity;

                var sprint = MyControllerHelper.IsControl(
                    MySpaceBindingCreator.CX_JETPACK,
                    MyControlsSpace.SPRINT,
                    MyControlStateType.PRESSED
                );
                
                var dampenEntity = __instance.RelativeDampeningEntity;
                
                if (dampenEntity != null)
                {
                    velocity -= dampenEntity.Physics.LinearVelocity;
                }
                
                if (!(velocity.LengthSquared() > Config.Current.NoBoostSpeedSq) || sprint) return;
                
                var velPlayer = Vector3.TransformNormal(velocity, worldMatrixInv);

                var originalLength = moveIndicator.Length();
                
                if (originalLength > .5f)
                {
                    var along = velPlayer.Project(moveIndicator);
                    var perpendicular = moveIndicator - along;

                    if (velPlayer.Dot(along) > 0)
                    {
                        // component in same direction as movement.
                        // thrust opposite with same magnitude as perpendicular
                        moveIndicator = (along.Normalized() * -perpendicular.Length()) + perpendicular;
                        
                        // if (prevVelocity > 0 && __instance.DampenersEnabled)
                        // {
                        //     if (velPlayer.LengthSquared() > prevVelocity)
                        //     {
                        //         prevMult -= .001f;
                        //         Console.WriteLine($"{prevMult}");
                        //     }
                        //     else if (velPlayer.LengthSquared() < prevVelocity)
                        //     {
                        //         prevMult += .001f;
                        //         Console.WriteLine($"{prevMult}");
                        //     }
                        // }
                    }

                    var velocityMult = (float)Math.Sqrt(originalLength) * Config.Current.HitSpeedMultiplier;
                    if (moveIndicator.LengthSquared() < .0001f && __instance.DampenersEnabled)
                    {
                        moveIndicator = velPlayer.Normalized() * velocityMult;
                    }
                    
                    if (moveIndicator.Length() < velocityMult && __instance.DampenersEnabled)
                    {
                        moveIndicator = moveIndicator.Normalized() * velocityMult;
                    }
                }

                // slow down to no boost speed
                if ((__instance.DampenersEnabled || Config.Current.SlowWhenDampenersOff) && 
                    velocity.LengthSquared() > Config.Current.SlowDownSpeedSq)
                {
                    moveIndicator += velPlayer.Normalized() * -SQRT_3;
                }

                // make sure magnitude is not too high
                if (moveIndicator.LengthSquared() > 2)
                {
                    moveIndicator = moveIndicator.Normalized() * SQRT_3;
                }
            }
            catch (NullReferenceException e)
            {
                
            }
        }
    }

    [HarmonyPatch(typeof(MyScriptManager), "AddAssembly")]
    public class PatchAddAssembly
    {
        private static Mutex mutex = new Mutex();
        
        static void Prefix()
        {
            mutex.WaitOne();
        }
    
        static void Finalizer()
        {
            mutex.ReleaseMutex();
        }
        
    }
    
    [HarmonyPatch("Shared.Patches.MyScriptCompilerPatch", "RecallFromCache")]
    public class PatchRecallFromCache
    {
        private static Mutex mutex = new Mutex();
        
        static void Prefix()
        {
            mutex.WaitOne();
        }
    
        static void Finalizer()
        {
            mutex.ReleaseMutex();
        }
        
    }
}