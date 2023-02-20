using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2BepInExPack.VanillaFixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.Networking;

namespace TemplarSkins
{
    // stolen from RMB
    public class RiskyMonkeyAchievements
    {
        public static void Patch()
        {
            Main.Harmony.PatchAll(typeof(PatchAchievementDefs));
        }

        [HarmonyPatch(typeof(SaferAchievementManager), nameof(SaferAchievementManager.SaferCollectAchievementDefs))]
        public class PatchAchievementDefs
        {
            public static void ILManipulator(ILContext il) 
            {
                ILCursor c = new(il);
                c.Index = 0;
                c.EmitDelegate(() => { Main.Log.LogDebug("Hook Planted"); });
                c.GotoNext(x => x.MatchCastclass<RegisterAchievementAttribute>(), x => x.MatchStloc(11));
                c.Index += 2;
                c.Emit(OpCodes.Ldloc, 10);
                c.Emit(OpCodes.Ldloc, 11);
                c.EmitDelegate<Func<Type, RegisterAchievementAttribute, RegisterAchievementAttribute>>((type, achievementAttribute) =>
                {
                    if (achievementAttribute != null) return achievementAttribute;    
                    RegisterModdedAchievementAttribute moddedAttribute = (RegisterModdedAchievementAttribute)Enumerable.FirstOrDefault(type.GetCustomAttributes(false), v => v is RegisterModdedAchievementAttribute);
                    if (moddedAttribute == null || !Main.Mods(moddedAttribute.mods) || !Main.EnableUnlocks.Value) return null;
                    MethodInfo m = type.GetMethod("OnlyRegisterIf");
                    if (m != null && !(bool)m.Invoke(null, null)) return null;
                    return new RegisterAchievementAttribute(moddedAttribute.identifier, moddedAttribute.unlockableRewardIdentifier, moddedAttribute.prerequisiteAchievementIdentifier, moddedAttribute.serverTrackerType);
                });
                c.Emit(OpCodes.Stloc, 11);
            }
        }
    }
}
