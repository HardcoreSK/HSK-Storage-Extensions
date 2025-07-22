using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace HSK_Storage_Extensions{
    public class HskStorageExtensions : Mod {
        
        public HskStorageExtensions(ModContentPack content) : base(content) {
            Harmony harmony = new Harmony("hsk.extensions.storage");
            harmony.PatchAll();

        }
    }
}
