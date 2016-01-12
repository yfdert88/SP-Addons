using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.Networking;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;
using SharpDX;
using Color = System.Drawing.Color;

namespace SP___LeBlanc
{
	class Program
	{
		static Spell.Targeted Q;
		static Spell.Skillshot W;
		static Spell.Active W2;
		static Spell.Skillshot E;
		static Spell.Targeted R;
		public static Menu Lbmenu, HarassMenu, LaneCMenu, KSMenu, ComboMenu, MiscMenu, DrawMenu;
		static AIHeroClient LB { get { return ObjectManager.Player; } }
		static void Main(string[] args)
		{
			Loading.OnLoadingComplete += Loading_OnLoadingComplete;
		}

		static void Game_OnTick(EventArgs args)
		{
			if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Combo();
			if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) Flee();
			if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) LaneClear();
			// KS();
		}

		static void MenuLB()
		{
			Lbmenu = MainMenu.AddMenu("SP-LeBlanc", "SP-LeBlanc");
			Lbmenu.AddGroupLabel("SP-LeBlanc");
			Lbmenu.AddSeparator();
			Lbmenu.AddGroupLabel("Q-R-W-E = Combo || E-R-Q-W Combo 2");
			Lbmenu.AddGroupLabel("Q-W-E = Harass");
			Lbmenu.AddGroupLabel("W-Q = Lane Clear");
			//Lbmenu.AddGroupLabel("Hydra,Tiamat,Youmuu's Item's Usage (Combo, Harass, LaneClear)");
			Lbmenu.AddLabel("SP-LeBlanc v1.0.0.2");
			// COMBO
            ComboMenu = Lbmenu.AddSubMenu("Combo", "combo");
			ComboMenu.AddGroupLabel("Combo Settings");
			ComboMenu.AddSeparator();
			ComboMenu.Add("usecomboq", new CheckBox("Use Q"));
			ComboMenu.Add("usecombow", new CheckBox("Use W"));
			ComboMenu.Add("usecomboe", new CheckBox("Use E"));
			ComboMenu.Add("usecombor", new CheckBox("Use R"));
			ComboMenu.AddSeparator();
			var combo = ComboMenu.Add("combo", new Slider("Combo 1 or 2", 0, 0, 1));
			combo.OnValueChange += delegate
			{
				combo.DisplayName = "Combo 1 or 2: " + new[] { "Combo 1", "Combo 2" }[combo.CurrentValue];
			};
			combo.DisplayName = "Combo 1 or 2: " + new[] { "Combo 1", "Combo 2" }[combo.CurrentValue];
			// HARASS
            /*HarassMenu = Lbmenu.AddSubMenu("Harass", "harass");
			HarassMenu.AddGroupLabel("Harass Settings");
			HarassMenu.AddSeparator();
			HarassMenu.Add("useharassq", new CheckBox("Use Q"));
			HarassMenu.Add("useharassw", new CheckBox("Use W"));
			HarassMenu.Add("useharasse", new CheckBox("Use E"));*/
			// KİLLSTEAL
            KSMenu = Lbmenu.AddSubMenu("Killsteal", "ks");
			KSMenu.AddGroupLabel("KillSteal Settings");
			KSMenu.AddSeparator();
			KSMenu.Add("ksq", new CheckBox("Use Q"));
			KSMenu.Add("ksw", new CheckBox("Use W"));
			KSMenu.Add("kse", new CheckBox("Use E"));
			// LANE
            LaneCMenu = Lbmenu.AddSubMenu("Lane Clear", "laneclear");
			LaneCMenu.AddGroupLabel("Lane Clear Settings");
			LaneCMenu.AddSeparator();
			LaneCMenu.Add("uselcq", new CheckBox("Use Q"));
			LaneCMenu.Add("uselcw", new CheckBox("Use W"));
			LaneCMenu.Add("wcnt", new Slider("Use W if Hit >= :", 3, 0, 10));
			LaneCMenu.Add("LMANA", new Slider("Min. mana for laneclear :", 0, 0, 100));
			// MİSC
            MiscMenu = Lbmenu.AddSubMenu("Misc", "misc");
			MiscMenu.AddGroupLabel("Misc Settings");
			MiscMenu.AddSeparator();
			MiscMenu.AddLabel("Skin Changer");
			var skin = MiscMenu.Add("sID", new Slider("Skin", 0, 0, 5));
			var sId = new[] { "Classic", "Wicked", "Prestigious", "Mistletoe", "Ravenborn" };
			skin.DisplayName = sId[skin.CurrentValue];

			skin.OnValueChange +=
                delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
			{
				sender.DisplayName = sId[changeArgs.NewValue];
			};
			var Style = MiscMenu.Add("style", new Slider("Min Prediction", 1, 0, 2));
			Style.OnValueChange += delegate
			{
				Style.DisplayName = "Min Prediction: " + new[] { "Low", "Medium", "High" }[Style.CurrentValue];
			};
			Style.DisplayName = "Min Prediction: " + new[] { "Low", "Medium", "High" }[Style.CurrentValue];
			// DRAW
            DrawMenu = Lbmenu.AddSubMenu("Drawing", "draw");
			DrawMenu.AddGroupLabel("Drawing Settings");
			DrawMenu.AddSeparator();
			DrawMenu.Add("drawq", new CheckBox("Draw Q"));
			DrawMenu.Add("drawst", new CheckBox("Draw Combo Status"));
		}

		static void Flee()
		{
			foreach (AIHeroClient enemy in EntityManager.Heroes.Enemies)
			{
			    if (enemy != null && E.IsReady() && enemy.IsValid())
                {
                    E.Cast(enemy); 
                }
                if (enemy != null && !E.IsReady() && W.IsReady() && enemy.IsValid())
                {
                    W.Cast(Game.CursorPos); 
                }
                if (enemy.GetAutoAttackRange() < W.Range && Player.Instance.HasBuff("LeBlancDisplacement"))
                {
                    W2.Cast();
                }
            }
            
        }

        public static double QDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.Q).IsLearned) return 0;
            return LB.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new double[] { 55 , 80 , 105 , 130 , 155 }[Program.Q.Level - 1] + 0.4 * LB.FlatMagicDamageMod));
        }
        public static double EDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.E).IsLearned) return 0;
            return LB.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new double[] { 40 , 65 , 90 , 115 , 140 }[Program.E.Level - 1] + 0.2 * LB.FlatMagicDamageMod));
        }
        public static double WDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.W).IsLearned) return 0;
            return LB.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new double[] { 85 , 125 , 165 , 205 , 245 }[Program.W.Level - 1] + 0.2 * LB.FlatMagicDamageMod));
        }
        public static double RDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.R).IsLearned) return 0;
            return LB.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new double[] { 100 , 200 , 300 }[Program.R.Level - 1] + 0.2 * LB.FlatMagicDamageMod));
        }

        private static void SChoose()
        {
            var style = MiscMenu["sID"].Cast<Slider>().CurrentValue;

            if (style == 0)
            {
                Player.SetSkinId(0);
            }
            if (style == 1)
            {
                Player.SetSkinId(1);
            }
            if (style == 2)
            {
                Player.SetSkinId(2);
            }
            if (style == 3)
            {
                Player.SetSkinId(3);
            }
            if (style == 4)
            {
                Player.SetSkinId(4);
            }
            if (style == 5)
            {
                Player.SetSkinId(5);
            }

        }

        static void KS()
        {
            var UsageQ = KSMenu["ksq"].Cast<CheckBox>().CurrentValue;
            var UsageW = KSMenu["ksw"].Cast<CheckBox>().CurrentValue;
            var UsageE = KSMenu["kse"].Cast<CheckBox>().CurrentValue;
            var QDMG = LB.GetSpellDamage(LB, SpellSlot.Q);
            var WDMG = LB.GetSpellDamage(LB, SpellSlot.W);
            var EDMG = LB.GetSpellDamage(LB, SpellSlot.E);
            var Style = MiscMenu["style"].Cast<Slider>().CurrentValue;
            var targetE = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            var targetW = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            var targetQ = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var ePred = E.GetPrediction(targetE);
            foreach (AIHeroClient enemy in EntityManager.Heroes.Enemies)
            {
                if (UsageQ && Q.IsReady() && targetQ.IsValidTarget() && (QDamage(enemy) >= enemy.Health))
                {
                    Q.Cast(enemy);
                }
                if (UsageW && W.IsReady() && targetW.IsValidTarget() && (WDamage(enemy) >= enemy.Health))
                {
                    W.Cast(enemy);
                }
                if (UsageE && E.IsReady() && targetE.IsValidTarget() && (EDamage(enemy) >= enemy.Health))
                {
                    if (Style == 0)
                    {
                        if (ePred.HitChance >= HitChance.Low)
                        {
                            E.Cast(enemy);
                        }
                    }
                    if (Style == 1)
                    {
                        if (ePred.HitChance >= HitChance.Medium)
                        {
                            E.Cast(enemy);
                        }
                    }
                    if (Style == 2)
                    {
                        if (ePred.HitChance >= HitChance.High)
                        {
                            E.Cast(enemy);
                        }
                    }
                }
            }
        }

        static void LaneClear()
        {
            var minion = (Obj_AI_Minion)GetEnemy(Program.Q.Range, GameObjectType.obj_AI_Minion);
            var UsageQ = LaneCMenu["uselcq"].Cast<CheckBox>().CurrentValue;
            var UsageW = LaneCMenu["uselcw"].Cast<CheckBox>().CurrentValue;
            var WFarm = LaneCMenu["wcnt"].Cast<Slider>().CurrentValue;
            var QDMG = LB.GetSpellDamage(LB, SpellSlot.Q);
            var WDMG = LB.GetSpellDamage(LB, SpellSlot.W);
            if (minion == null) return;
            var laneclearMinMana = LaneCMenu["LMANA"].Cast<Slider>().CurrentValue;
            if (Player.Instance.ManaPercent >= laneclearMinMana)
            {
                if ((QDamage(minion) >= minion.Health) && UsageQ && Q.IsReady() && minion.IsValidTarget())
                {
                    Q.Cast(minion);
                }
                if ((WDamage(minion) >= minion.Health) && UsageW && W.IsReady() && minion.IsValidTarget() && WFarm <=
                        EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, LB.Position,
                            W.Range).Count())
                {
                    W.Cast(minion);
                }
                 if (Player.Instance.HasBuff("LeBlancDisplacement"))
                {
                    W2.Cast(); 
                }
            }
        }

        private static Obj_AI_Base GetEnemy(float range, GameObjectType t)
        {
            switch (t)
            {
                case GameObjectType.AIHeroClient:
                    return EntityManager.Heroes.Enemies.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) < range && !a.IsDead && !a.IsInvulnerable);
                default:
                    return EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) < range && !a.IsDead && !a.IsInvulnerable);
            }
        }

        static void Combo()
        {
            var targetW = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var targetE = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            var ePred = E.GetPrediction(targetE);
            var Style = MiscMenu["style"].Cast<Slider>().CurrentValue;
            var UsageQ = ComboMenu["usecomboq"].Cast<CheckBox>().CurrentValue;
            var UsageW = ComboMenu["usecombow"].Cast<CheckBox>().CurrentValue;
            var UsageE = ComboMenu["usecomboe"].Cast<CheckBox>().CurrentValue;
            var UsageR = ComboMenu["usecombor"].Cast<CheckBox>().CurrentValue;
            var Combo = ComboMenu["combo"].Cast<Slider>().CurrentValue;
                if (Combo == 0)
                {
                    if (targetW.IsValidTarget() && UsageQ && Q.IsReady())
                    {
                        Q.Cast(targetW);
                    }
                    if (targetW.IsValidTarget() && UsageR && R.IsReady())
                    {
                        R.Cast(targetW);
                    }
                    if (targetW.IsValidTarget() && UsageW && W.IsReady())
                    {
                        W.Cast(targetW);
                    }
                if (Player.Instance.HasBuff("LeBlancDisplacement"))
                {
                    W2.Cast(); 
                }
                if (!Q.IsReady() && !W.IsReady() && targetE.IsValidTarget() && UsageE && E.IsReady())
                    {
                        if (Style == 0)
                        {
                            if (ePred.HitChance >= HitChance.Low)
                            {
                                E.Cast(targetE);
                            }
                        }
                        if (Style == 1)
                        {
                            if (ePred.HitChance >= HitChance.Medium)
                            {
                                E.Cast(targetE);
                            }
                        }
                        if (Style == 2)
                        {
                            if (ePred.HitChance >= HitChance.High)
                            {
                                E.Cast(targetE);
                            }
                        }
                    }
                }
                if (Combo == 1)
                {
                    if (targetE.IsValidTarget() && UsageE && E.IsReady())
                    {
                        if (Style == 0)
                        {
                            if (ePred.HitChance >= HitChance.Low)
                            {
                                E.Cast(targetE);
                            }
                        }
                        if (Style == 1)
                        {
                            if (ePred.HitChance >= HitChance.Medium)
                            {
                                E.Cast(targetE);
                            }
                        }
                        if (Style == 2)
                        {
                            if (ePred.HitChance >= HitChance.High)
                            {
                                E.Cast(targetE);
                            }
                        }
                    }

                    if (!E.IsReady() && targetE.IsValidTarget() && UsageR && R.IsReady())
                    {
                        R.Cast(targetE);
                    }
                    if (targetW.IsValidTarget() && UsageQ && Q.IsReady())
                    {
                        Q.Cast(targetW);
                    }
                    if (targetW.IsValidTarget() && UsageW && W.IsReady())
                    {
                        W.Cast(targetW);
                    }
                }

        }

        static void Loading_OnLoadingComplete(EventArgs args)
        {
            Chat.Print("5.25 - SP LeBlanc Loaded");
            Q = new Spell.Targeted(SpellSlot.Q, 700);
            W = new Spell.Skillshot(SpellSlot.W, 600, SkillShotType.Circular);
            W2 = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Linear);
            R = new Spell.Targeted(SpellSlot.R, 700);
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += OnDraw;
            MenuLB();
            SChoose();
        }

        private static void OnDraw(EventArgs args)
        {
            var Combo = ComboMenu["combo"].Cast<Slider>().CurrentValue;
            if (DrawMenu["drawst"].Cast<CheckBox>().CurrentValue) {
                if (Combo == 1)
                {
                    Drawing.DrawText(Drawing.WorldToScreen(Player.Instance.Position) - new Vector2(0, 34), Color.Green, "Combo 2: Open", 2);
                }
                if (Combo == 0)
                {
                    Drawing.DrawText(Drawing.WorldToScreen(Player.Instance.Position) - new Vector2(0, 34), Color.Green, "Combo : Open", 2);
                }
                if (Q.IsReady() && W.IsReady() && E.IsReady() && R.IsReady())
                {
                    Drawing.DrawText(Drawing.WorldToScreen(Player.Instance.Position) - new Vector2(0, 17), Color.Green, "Full Combo is Ready", 2);
                } else
                {
                    Drawing.DrawText(Drawing.WorldToScreen(Player.Instance.Position) - new Vector2(0, 17), Color.Red, "Full Combo is not Ready", 2);
                }
            }
            if (DrawMenu["drawq"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(Player.Instance.Position, Q.Range, Color.Blue);
            }

        }
    }
}
