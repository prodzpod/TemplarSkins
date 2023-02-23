using R2API;
using RoR2;
using RoR2.Achievements;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TemplarSkins
{
    public class Unlocks
    {
        public static Dictionary<string, UnlockableDef> unlockables;
        public static void Patch()
        {
            unlockables = new();
            MakeUnlockable("Characters.Templar");
            MakeUnlockable("Skills.TEMPLAR_PRIMARY_RAILGUN_NAME");
            MakeUnlockable("Skills.TEMPLAR_PRIMARY_FLAMETHROWER_NAME");
            if (Templar.Templar.bazookaGoBoom.Value) MakeUnlockable("Skills.TEMPLAR_PRIMARY_BAZOOKA_NAME");
            MakeUnlockable("Skills.TEMPLAR_SECONDARY_SHOTGUN_NAME");
            MakeUnlockable("Skills.TEMPLAR_UTILITY_DODGE_NAME");
            MakeUnlockable("Skills.TEMPLAR_SPECIAL_SWAP_NAME");
            MakeUnlockable("Skins.Templar.Alt1");
            if (Main.Mods("com.themysticsword.bulwarkshaunt")) MakeUnlockable("Skins.Templar.BulwarksHaunt_Alt");
            MakeUnlockable("Skins.Templar.Enemy");
            if (Main.Mods("prodzpod.Downpour"))
            {
                if (Main.EnableEngiVoidSkin.Value) MakeUnlockable("Skins.Engineer.Simulated");
                MakeUnlockable("Skins.Templar.Simulated");
            }
            MakeUnlockable("Skins.Templar.E8");
            if (Main.Mods("HIFU.Inferno")) MakeUnlockable("Skins.Templar.Inferno");
            AchievementManager.onAchievementsRegistered += PostPatch;
        }
        public static void PostPatch()
        {
            AddUnlockableSurvivor("Templar_Survivor", "Templar");
            AddUnlockableSkill("TEMPLAR_PRIMARY_RAILGUN_NAME");
            AddUnlockableSkill("TEMPLAR_PRIMARY_FLAMETHROWER_NAME");
            if (Templar.Templar.bazookaGoBoom.Value) AddUnlockableSkill("TEMPLAR_PRIMARY_BAZOOKA_NAME");
            AddUnlockableSkill("TEMPLAR_SECONDARY_SHOTGUN_NAME");
            AddUnlockableSkill("TEMPLAR_UTILITY_DODGE_NAME");
            AddUnlockableSkill("TEMPLAR_SPECIAL_SWAP_NAME");
            AddUnlockableSkin("skinTemplarAlt", "Templar.Alt1");
            if (Main.Mods("com.themysticsword.bulwarkshaunt")) AddUnlockableSkin("skinTemplarBulwarksHauntAlt", "Templar.BulwarksHaunt_Alt");
            AddUnlockableSkin("skinTemplarGoldAlt", "Templar.Enemy");
            if (Main.Mods("prodzpod.Downpour"))
            {
                if (Main.EnableEngiVoidSkin.Value) AddUnlockableSkin("skinEngiVoidAlt", "Engineer.Simulated");
                AddUnlockableSkin("skinTemplarVoidAlt", "Templar.Simulated");
            }
            AddUnlockableSkin("TemplarNeo", "Templar.E8");
            if (Main.Mods("HIFU.Inferno")) AddUnlockableSkin("skinTemplarInfernoAlt", "Templar.Inferno");
        }
        public static void MakeUnlockable(string name)
        {
            UnlockableDef unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            unlockableDef.cachedName = name;
            ContentAddition.AddUnlockableDef(unlockableDef);
            unlockables.Add(name, unlockableDef);
            Main.Log.LogDebug("Registered Unlockable " + name);
        }
        public static void AddUnlockableSurvivor(string bodyName, string name)
        {
            Sprite icon = Main.AssetBundle.LoadAsset<Sprite>("Assets/unlocks/texSurvivor" + name + ".png");
            UnlockableDef unlockableDef = unlockables["Characters." + name];
            Main.Log.LogDebug("Fetched Survivor Unlockable Characters." + name);
            SurvivorDef def = SurvivorCatalog.GetSurvivorDef(SurvivorCatalog.GetSurvivorIndexFromBodyIndex(BodyCatalog.FindBodyIndex(bodyName)));
            unlockableDef.nameToken = def.displayNameToken;
            unlockableDef.achievementIcon = icon;
            def.unlockableDef = unlockableDef;
            AchievementManager.GetAchievementDefFromUnlockable(unlockableDef.cachedName).achievedIcon = icon;
        }
        public static void AddUnlockableSkill(string skillName, string name = null)
        {
            if (name == null) name = skillName;
            SkillDef def = null;
            UnlockableDef unlockableDef = unlockables["Skills." + name];
            Main.Log.LogDebug("Fetched Skill Unlockable Skills." + name);
            foreach (var skill in SkillCatalog.allSkillDefs) if (skill.skillNameToken == skillName) { def = skill; break; }
            if (def == null) return;
            unlockableDef.nameToken = def.skillNameToken;
            unlockableDef.achievementIcon = def.icon;
            AchievementManager.GetAchievementDefFromUnlockable(unlockableDef.cachedName).achievedIcon = def.icon;
            IEnumerator<SkillFamily> families = SkillCatalog.allSkillFamilies.GetEnumerator();
            while (families.MoveNext()) for (var i = 0; i < families.Current.variants.Length; i++) if (families.Current.variants[i].skillDef == def) families.Current.variants[i].unlockableDef = unlockableDef;
        }
        public static void AddUnlockableSkin(string skinName, string name)
        {
            SkinDef def = null;
            foreach (var skin in SkinCatalog.allSkinDefs) if (skin.name == skinName) def = skin;
            UnlockableDef unlockableDef = unlockables["Skins." + name];
            Main.Log.LogDebug("Fetched Skin Unlockable Skins." + name);
            unlockableDef.nameToken = def.nameToken;
            unlockableDef.achievementIcon = def.icon;
            def.unlockableDef = unlockableDef;
            AchievementManager.GetAchievementDefFromUnlockable(unlockableDef.cachedName).achievedIcon = def.icon;
        }

        [RegisterModdedAchievement("TemplarSkins_Character", "Characters.Templar", null, null)]
        public class TemplarAchievement : BaseAchievement
        {
            public override void OnInstall() { base.OnInstall(); On.EntityStates.Missions.Goldshores.Exit.OnEnter += OnEnter; }
            public override void OnUninstall() { On.EntityStates.Missions.Goldshores.Exit.OnEnter -= OnEnter; base.OnUninstall(); }

            private void OnEnter(On.EntityStates.Missions.Goldshores.Exit.orig_OnEnter orig, EntityStates.Missions.Goldshores.Exit self)
            {
                if (localUser != null && localUser.cachedBody != null) Grant();
                orig(self);
            }
        }
        [RegisterModdedAchievement("TemplarSkins_Skills_TEMPLAR_PRIMARY_RAILGUN_NAME", "Skills.TEMPLAR_PRIMARY_RAILGUN_NAME", null, null)]
        public class TEMPLAR_PRIMARY_RAILGUN_NAMEAchievement : BaseAchievement
        {
            public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex("Templar_Survivor");
            public override void OnBodyRequirementMet() { base.OnBodyRequirementMet(); GlobalEventManager.onClientDamageNotified += OnDamage; }
            public override void OnBodyRequirementBroken() { GlobalEventManager.onClientDamageNotified -= OnDamage; base.OnBodyRequirementBroken(); }
            public void OnDamage(DamageDealtMessage damageDealtMessage) { if (damageDealtMessage.attacker == null || damageDealtMessage.attacker != localUser.cachedBody) return; if (damageDealtMessage.damage >= 10000) Grant(); }
        }

        [RegisterModdedAchievement("TemplarSkins_Skills_TEMPLAR_PRIMARY_FLAMETHROWER_NAME", "Skills.TEMPLAR_PRIMARY_FLAMETHROWER_NAME", null, null)]
        public class TEMPLAR_PRIMARY_FLAMETHROWER_NAMEAchievement : BaseAchievement
        {
            public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex("Templar_Survivor");
            public override void OnBodyRequirementMet() { base.OnBodyRequirementMet(); RoR2Application.onUpdate += OnUpdate; }
            public override void OnBodyRequirementBroken() { RoR2Application.onUpdate -= OnUpdate; base.OnBodyRequirementBroken(); }
            public void OnUpdate()
            {
                if (localUser == null || localUser.cachedBody == null) return;
                int num = 0;
                foreach (HurtBox hurtBox in new SphereSearch() { origin = localUser.cachedBody.footPosition, radius = 50000f, mask = LayerIndex.entityPrecise.mask }.RefreshCandidates()
                    .FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(localUser.cachedBody.teamComponent.teamIndex)).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes())
                {
                    CharacterBody body = hurtBox.healthComponent.body;
                    body.MarkAllStatsDirty();
                    if (body.HasBuff(RoR2Content.Buffs.ClayGoo)) ++num;
                }
                if (num >= 20) Grant();
            }
        }

        [RegisterModdedAchievement("TemplarSkins_Skills_TEMPLAR_PRIMARY_BAZOOKA_NAME", "Skills.TEMPLAR_PRIMARY_BAZOOKA_NAME", null, null)]
        public class TEMPLAR_PRIMARY_BAZOOKA_NAMEAchievement : BaseAchievement
        {
            public static bool OnlyRegisterIf() { return Templar.Templar.bazookaGoBoom.Value; }
            public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex("Templar_Survivor");
            public override void OnBodyRequirementMet() { base.OnBodyRequirementMet(); GlobalEventManager.onCharacterDeathGlobal += OnCharacterDeath; }
            public override void OnBodyRequirementBroken() { GlobalEventManager.onCharacterDeathGlobal -= OnCharacterDeath; base.OnBodyRequirementBroken(); }
            public void OnCharacterDeath(DamageReport damageReport)
            {
                if (localUser == null || localUser.cachedBody == null || Stage.instance.sceneDef.cachedName != "goolake") return;
                foreach (HurtBox hurtBox in new SphereSearch() { origin = localUser.cachedBody.footPosition, radius = 50000f, mask = LayerIndex.entityPrecise.mask }.RefreshCandidates().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes())
                {
                    CharacterBody body = hurtBox.healthComponent.body;
                    if (body.name.Contains("ExplosivePotDestructibleBody")) return;
                }
                Grant();
            }
        }

        [RegisterModdedAchievement("TemplarSkins_Skills_TEMPLAR_SECONDARY_SHOTGUN_NAME", "Skills.TEMPLAR_SECONDARY_SHOTGUN_NAME", null, null)]
        public class TEMPLAR_SECONDARY_SHOTGUN_NAMEAchievement : BaseAchievement
        {
            public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex("Templar_Survivor");
            public override void OnBodyRequirementMet() { base.OnBodyRequirementMet(); RoR2Application.onUpdate += OnUpdate; }
            public override void OnBodyRequirementBroken() { RoR2Application.onUpdate -= OnUpdate; base.OnBodyRequirementBroken(); }
            public void OnUpdate() { if (localUser == null || localUser.cachedBody == null) return; if (localUser.cachedBody.skillLocator.primary.cooldownScale <= 0.2f) Grant(); }
        }

        [RegisterModdedAchievement("TemplarSkins_Skills_TEMPLAR_UTILITY_DODGE_NAME", "Skills.TEMPLAR_UTILITY_DODGE_NAME", null, null)]
        public class TEMPLAR_UTILITY_DODGE_NAMEAchievement : BaseAchievement
        {
            public bool enabled = false;
            public float stopwatch = 0;
            public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex("Templar_Survivor");
            public override void OnBodyRequirementMet()
            {
                base.OnBodyRequirementMet();
                On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += OnEncounterStart;
                RoR2Application.onFixedUpdate += OnUpdate;
                On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += OnEncounterFinished;
            }
            public override void OnBodyRequirementBroken()
            {
                On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter -= OnEncounterStart;
                RoR2Application.onFixedUpdate -= OnUpdate;
                On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter -= OnEncounterFinished;
                base.OnBodyRequirementBroken();
            }
            public void OnUpdate()
            {
                if (localUser == null || localUser.cachedBody == null || !enabled) return;
                stopwatch += Time.deltaTime;
            }

            public void OnEncounterStart(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
            {
                stopwatch = 0;
                enabled = true;
                orig(self);
            }

            public void OnEncounterFinished(On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
            {
                if (stopwatch < 180f)
                {
                    Grant();
                    enabled = false;
                }
                orig(self);
            }
        }

        [RegisterModdedAchievement("TemplarSkins_Skills_TEMPLAR_SPECIAL_SWAP_NAME", "Skills.TEMPLAR_SPECIAL_SWAP_NAME", null, null)]
        public class TEMPLAR_SPECIAL_SWAP_NAMEAchievement : BaseAchievement
        {
            public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex("Templar_Survivor");
            public override void OnBodyRequirementMet() { base.OnBodyRequirementMet(); RoR2Application.onUpdate += OnUpdate; }
            public override void OnBodyRequirementBroken() { RoR2Application.onUpdate -= OnUpdate; base.OnBodyRequirementBroken(); }
            public void OnUpdate() { if (localUser == null || localUser.cachedBody == null) return; if (localUser.cachedBody.healthComponent.health >= 3000) Grant(); }
        }

        [RegisterModdedAchievement("TemplarClearGameMonsoon", "Skins.Templar.Alt1", null, null)] public class TemplarClearGameMonsoonAchievement : BasePerSurvivorClearGameMonsoonAchievement { public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex("Templar_Survivor"); }
        [RegisterModdedAchievement("BulwarksHaunt_TemplarWinGhostWave", "Skins.Templar.BulwarksHaunt_Alt", null, null, "com.themysticsword.bulwarkshaunt")] public class TemplarWinGhostWave : BulwarksHaunt.Achievements.BaseWinGhostWavePerSurvivor { public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex("Templar_Survivor"); }

        [RegisterModdedAchievement("TemplarSkins_Skin_Enemy", "Skins.Templar.Enemy", null, typeof(TemplarEnemySkinServerAchievement))]
        public class TemplarEnemySkinAchievement : BasePerSurvivorEnemySkinAchievement
        {
            public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex("Templar_Survivor");
            public class TemplarEnemySkinServerAchievement : BasePerSurvivorEnemySkinServerAchievement
            { public override BodyIndex body => BodyCatalog.FindBodyIndex("TitanGoldBody"); public override int reqKills => 2; }
        }
        public class BasePerSurvivorEnemySkinAchievement : BaseAchievement
        {

            public override void OnBodyRequirementMet() { base.OnBodyRequirementMet(); SetServerTracked(true); }
            public override void OnBodyRequirementBroken() { SetServerTracked(false); base.OnBodyRequirementBroken(); }

            public class BasePerSurvivorEnemySkinServerAchievement : BaseServerAchievement
            {
                private int kills;
                public virtual BodyIndex body => BodyIndex.None;
                public virtual int reqKills => 100;

                public override void OnInstall()
                {
                    base.OnInstall();
                    Run.onRunStartGlobal += OnRunStart;
                    GlobalEventManager.onCharacterDeathGlobal += onKill;
                    if (Run.instance != null) kills = 0;
                }

                public override void OnUninstall()
                {
                    Run.onRunStartGlobal -= OnRunStart;
                    GlobalEventManager.onCharacterDeathGlobal -= onKill;
                    base.OnUninstall();
                }
                private void OnRunStart(Run _) => kills = 0;

                private void onKill(DamageReport damageReport)
                {
                    if (damageReport.victimBodyIndex == body)
                    {
                        kills++;
                        if (kills >= reqKills)
                        {
                            Grant();
                            Run.onRunStartGlobal -= OnRunStart;
                            GlobalEventManager.onCharacterDeathGlobal -= onKill;
                        }
                    }
                }
            }
        }
        [RegisterModdedAchievement("EngineerClearGameSimulacrum", "Skins.Engineer.Simulated", "CompleteMainEnding", typeof(BasePerSurvivorClearGameSimulacrumServerAchievement), "prodzpod.Downpour")] public class EngineerClearGameSimulacrumAchievement : BasePerSurvivorClearGameSimulacrumAchievement { [SystemInitializer(new Type[] { typeof(HG.Reflection.SearchableAttribute.OptInAttribute) })] public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex("EngiBody"); public static bool OnlyRegisterIf() { return Main.EnableEngiVoidSkin.Value; } }
        [RegisterModdedAchievement("TemplarClearGameSimulacrum", "Skins.Templar.Simulated", "CompleteMainEnding", typeof(BasePerSurvivorClearGameSimulacrumServerAchievement), "prodzpod.Downpour")] public class TemplarClearGameSimulacrumAchievement : BasePerSurvivorClearGameSimulacrumAchievement { [SystemInitializer(new Type[] { typeof(HG.Reflection.SearchableAttribute.OptInAttribute) })] public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex("Templar_Survivor"); }
        public class BasePerSurvivorClearGameSimulacrumAchievement : BaseEndingAchievement
        {
            public override void OnBodyRequirementMet() { base.OnBodyRequirementMet(); SetServerTracked(true); }

            public override void OnBodyRequirementBroken() { SetServerTracked(false); base.OnBodyRequirementBroken(); }

            public override bool ShouldGrant(RunReport runReport) => false;

            public class BasePerSurvivorClearGameSimulacrumServerAchievement : BaseServerAchievement
            {

                public override void OnInstall()
                {
                    base.OnInstall(); InfiniteTowerRun.onAllEnemiesDefeatedServer += new Action<InfiniteTowerWaveController>(OnAllEnemiesDefeatedServer);
                }

                public override void OnUninstall()
                {
                    InfiniteTowerRun.onAllEnemiesDefeatedServer -= new Action<InfiniteTowerWaveController>(OnAllEnemiesDefeatedServer); base.OnUninstall();
                }

                private void OnAllEnemiesDefeatedServer(InfiniteTowerWaveController waveController)
                {
                    InfiniteTowerRun instance = Run.instance as InfiniteTowerRun;
                    if (instance != null && Downpour.DownpourPlugin.DownpourList.Contains(DifficultyCatalog.GetDifficultyDef(Run.instance.selectedDifficulty)) && instance.waveIndex >= 50) Grant();
                }
            }
        }
        [RegisterModdedAchievement("TemplarClearGameE8", "Skins.Templar.E8", "CompleteMainEnding", null)] public class TemplarClearGameE8Achievement : BasePerSurvivorClearGameE8Achievement { [SystemInitializer(new Type[] { typeof(HG.Reflection.SearchableAttribute.OptInAttribute) })] public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex("Templar_Survivor"); }
        public class BasePerSurvivorClearGameE8Achievement : BaseAchievement
        {
            [SystemInitializer(new Type[] { typeof(HG.Reflection.SearchableAttribute.OptInAttribute) })]
            public override void OnBodyRequirementMet()
            {
                base.OnBodyRequirementMet();
                Run.onClientGameOverGlobal += OnClientGameOverGlobal;
            }

            [SystemInitializer(new Type[] { typeof(HG.Reflection.SearchableAttribute.OptInAttribute) })]
            public override void OnBodyRequirementBroken()
            {
                Run.onClientGameOverGlobal -= OnClientGameOverGlobal;
                base.OnBodyRequirementBroken();
            }

            [SystemInitializer(new Type[] { typeof(HG.Reflection.SearchableAttribute.OptInAttribute) })]
            private void OnClientGameOverGlobal(Run run, RunReport runReport)
            {
                if ((bool)runReport.gameEnding && runReport.gameEnding.isWin)
                {
                    if (run.selectedDifficulty == DifficultyIndex.Eclipse8 || (Main.Mods("com.TPDespair.ZetArtifacts") && eclifactEnabled()))
                    {
                        runReport.gameEnding.lunarCoinReward = 15u;
                        runReport.gameEnding.showCredits = false;
                        Grant();
                    }
                }
            }
        }
        public static bool eclifactEnabled()
        {
            return TPDespair.ZetArtifacts.ZetEclifact.Enabled;
        }
        [RegisterModdedAchievement("TemplarClearGameInferno", "Skins.Templar.Inferno", "CompleteMainEnding", null, "HIFU.Inferno")] public class TemplarClearGameInfernoAchievement : Inferno.Unlocks.BasePerSurvivorClearGameInfernoAchievement { [SystemInitializer(new Type[] { typeof(HG.Reflection.SearchableAttribute.OptInAttribute) })] public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex("Templar_Survivor"); }
    }
}
