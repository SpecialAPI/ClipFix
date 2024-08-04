using BepInEx;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ClipFix
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.clipoverflow";
        public const string NAME = "Clip Overflow";
        public const string VERSION = "1.0.0";

        public static MethodInfo f1cm_f = AccessTools.Method(typeof(Plugin), nameof(Fix1ClipMods_Fix));

        public void Awake()
        {
            new Harmony(GUID).PatchAll();
        }

        [HarmonyPatch(typeof(ProjectileModule), nameof(ProjectileModule.GetModNumberOfShotsInClip))]
        [HarmonyILManipulator]
        public static void Fix1ClipMods_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if(!crs.TryGotoNext(x => x.MatchBneUn(out _)))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, f1cm_f);
        }

        public static int Fix1ClipMods_Fix(int current, ProjectileModule mod)
        {
            if (mod == null)
                return current;

            var shots = mod.numberOfShotsInClip;

            if (shots == current)
                return current + 1;

            return current;
        }
    }
}
