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

namespace SP___Khazix
{
    class Program
    {
        static Spell.Targeted Q;
        static Spell.Skillshot W;
        static Spell.Skillshot E;
        static Spell.Active R;
        public static Menu KhaMenu, HarassMenu, LaneCMenu, ComboMenu, MiscMenu, DrawMenu;
        static AIHeroClient Kha { get { return ObjectManager.Player; } }
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
             
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                HarassKha();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneKha();
            }
        }

        static void MenuKha()
        {
            KhaMenu = MainMenu.AddMenu("SP-Khazix", "SP-Khazix");
            KhaMenu.AddGroupLabel("SP-Khazix");
            KhaMenu.AddSeparator();
            KhaMenu.AddGroupLabel("R-W-E-Q = Combo");
            KhaMenu.AddGroupLabel("W-E-Q = Harass");
            KhaMenu.AddGroupLabel("W-Q = Lane Clear");
            KhaMenu.AddGroupLabel("Hydra,Tiamat,Youmuu's Item's Usage (Combo, Harass, LaneClear)");
            KhaMenu.AddLabel("SP-Khazix v1.0.0.2");
            // COMBO
            ComboMenu = KhaMenu.AddSubMenu("Combo", "combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.AddSeparator();
            ComboMenu.Add("usecomboq", new CheckBox("Use Q"));
            ComboMenu.Add("usecombow", new CheckBox("Use W"));
            ComboMenu.Add("usecomboe", new CheckBox("Use E"));
            ComboMenu.Add("usecombor", new CheckBox("Use R"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("useitems", new CheckBox("Use Items"));
            // HARASS
            HarassMenu = KhaMenu.AddSubMenu("Harass", "harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.AddSeparator();
            HarassMenu.Add("useharassq", new CheckBox("Use Q"));
            HarassMenu.Add("useharassw", new CheckBox("Use W"));
            HarassMenu.Add("useharasse", new CheckBox("Use E"));
            // LANE
            LaneCMenu = KhaMenu.AddSubMenu("Lane Clear", "laneclear");
            LaneCMenu.AddGroupLabel("Lane Clear Settings");
            LaneCMenu.AddSeparator();
            LaneCMenu.Add("uselcq", new CheckBox("Use Q"));
            LaneCMenu.Add("uselcw", new CheckBox("Use W"));
            LaneCMenu.Add("LMANA", new Slider("Min. mana for laneclear :", 0, 0, 100));
            // MİSC
            MiscMenu = KhaMenu.AddSubMenu("Misc", "misc");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.AddSeparator();
            MiscMenu.Add("skin.", new Slider("Skin ID", 0, 0, 2));
            var Style = MiscMenu.Add("style", new Slider("Min Prediction", 1, 0, 2));
            Style.OnValueChange += delegate
            {
                Style.DisplayName = "Min Prediction: " + new[] { "Low", "Medium", "High" }[Style.CurrentValue];
            };
            Style.DisplayName = "Min Prediction: " + new[] { "Low", "Medium", "High" }[Style.CurrentValue];
            // DRAW
            DrawMenu = KhaMenu.AddSubMenu("Drawing", "draw");
            DrawMenu.AddGroupLabel("Drawing Settings");
            DrawMenu.AddSeparator();
            DrawMenu.Add("drawq", new CheckBox("Draw Q"));
            DrawMenu.Add("draww", new CheckBox("Draw W"));
            DrawMenu.Add("drawe", new CheckBox("Draw E"));
        }


        static void Items()
        {
            if (Item.HasItem(3074) && Item.CanUseItem(3074)) // Hydra
                Item.UseItem(3074);
            if (Item.HasItem(3077) && Item.CanUseItem(3077)) // Tiamat
                Item.UseItem(3077);
            if (Item.HasItem(3142) && Item.CanUseItem(3142)) // Youmuu's
                Item.UseItem(3142);
            if (Item.HasItem(3748) && Item.CanUseItem(3748)) // Titanic Hydra
                Item.UseItem(3748);
        }

        static void Itemsminion()
        {
            if (Item.HasItem(3074) && Item.CanUseItem(3074)) // Hydra
                Item.UseItem(3074);
            if (Item.HasItem(3077) && Item.CanUseItem(3077)) //Tiamat
                Item.UseItem(3077);
        }

        static void Combo()
        {

            var khaq = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
           // var khaw = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var targetW = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var wPred = W.GetPrediction(targetW);
            var khae = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var UsageItems = ComboMenu["useitems"].Cast<CheckBox>().CurrentValue;
            var Style = MiscMenu["style"].Cast<Slider>().CurrentValue;
            var UsageQ = ComboMenu["usecomboq"].Cast<CheckBox>().CurrentValue;
            var UsageW = ComboMenu["usecombow"].Cast<CheckBox>().CurrentValue;
            var UsageE = ComboMenu["usecomboe"].Cast<CheckBox>().CurrentValue;
            var UsageR = ComboMenu["usecombor"].Cast<CheckBox>().CurrentValue;
            
            if (targetW.IsValidTarget(800) && UsageR && R.IsReady())
                {
                    R.Cast();
                }
            if (UsageItems)
                {
                    Items();
                }
            if (targetW.IsValidTarget(600) && Q.IsReady() && E.IsReady() && W.IsReady())
            {


                if (UsageE)
                {
                    E.Cast(W.GetPrediction(targetW).CastPosition);
                }
                if (UsageQ)
                {
                    Q.Cast(W.GetPrediction(targetW).CastPosition);
                }
                if (Style == 0)
                {
                    if (wPred.HitChance >= HitChance.Low)
                    {
                        if (UsageW)
                        {
                            W.Cast(targetW);
                        }
                        Core.DelayAction(() => E.Cast(targetW), W.CastDelay + Game.Ping);

                        var EDelay = (int)((Kha.Distance(targetW) / Kha.Spellbook.GetSpell(SpellSlot.E).SData.MissileSpeed * 1000) + E.CastDelay);

                        Core.DelayAction(() => Q.Cast(targetW), EDelay);
                    }
                }
                if (Style == 1) { 
                if (wPred.HitChance >= HitChance.Medium)
                {
                    if (UsageW)
                    {
                        W.Cast(targetW);
                    }
                    Core.DelayAction(() => E.Cast(targetW), W.CastDelay + Game.Ping);

                    var EDelay = (int)((Kha.Distance(targetW) / Kha.Spellbook.GetSpell(SpellSlot.E).SData.MissileSpeed * 1000) + E.CastDelay);

                    Core.DelayAction(() => Q.Cast(targetW), EDelay);
                }
            }
                if (Style == 2)
                {
                    if (wPred.HitChance >= HitChance.High)
                    {
                        if (UsageW)
                        {
                            W.Cast(targetW);
                        }
                        Core.DelayAction(() => E.Cast(targetW), W.CastDelay + Game.Ping);

                        var EDelay = (int)((Kha.Distance(targetW) / Kha.Spellbook.GetSpell(SpellSlot.E).SData.MissileSpeed * 1000) + E.CastDelay);

                        Core.DelayAction(() => Q.Cast(targetW), EDelay);
                    }
                }
            }
            else
            {
                if (khaq.IsValidTarget(Kha.GetAutoAttackRange()) && Q.IsReady())
                {
                    if (UsageQ)
                    {
                        Q.Cast(khaq);
                    }

                    if (targetW.IsValidTarget() && W.IsReady())
                    {
                        if (wPred.HitChance >= HitChance.Medium)
                        {
                            if (UsageW) {
                                W.Cast(targetW);
                            }
                        }
                    }
                    if (khae.IsValidTarget() && E.IsReady())
                    {
                        if (UsageE) {
                            E.Cast(khae);
                        }
                    }
                }
                return;
            }
        }


        static void HarassKha()
        {
            var hkhaq = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var hkhaw = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var minionkhaw = TargetSelector.GetTarget(300, DamageType.Physical);
            var hkhae = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var HWPred = W.GetPrediction(minionkhaw);
            var UsageQ = HarassMenu["useharassq"].Cast<CheckBox>().CurrentValue;
            var UsageW = HarassMenu["useharassw"].Cast<CheckBox>().CurrentValue;
            var UsageE = HarassMenu["useharasse"].Cast<CheckBox>().CurrentValue;
            var UsageItems = ComboMenu["useitems"].Cast<CheckBox>().CurrentValue;
            if (UsageQ && hkhaq.IsValidTarget(Kha.GetAutoAttackRange()) && Q.IsReady())
            {
                Q.Cast(hkhaq);
                if (UsageItems)
                {
                    Items();
                }
            }
            if (HWPred.HitChance >= HitChance.Medium && !HWPred.CollisionObjects.Any(it => it.Name.ToLower().Contains("minion")) && UsageW)
            {
                W.Cast(hkhaw);
            }
            if (UsageE && hkhae.IsValidTarget() && E.IsReady())
            {
                E.Cast(hkhae);
            }
        }

        static void LaneKha()
        {
            var minion = (Obj_AI_Minion)GetEnemy(Program.Q.Range, GameObjectType.obj_AI_Minion);
            var UsageQ = LaneCMenu["uselcq"].Cast<CheckBox>().CurrentValue;
            var UsageW = LaneCMenu["uselcw"].Cast<CheckBox>().CurrentValue;
            if (minion == null) return;
            Itemsminion();
            var laneclearMinMana = LaneCMenu["LMANA"].Cast<Slider>().CurrentValue;
            if (Player.Instance.ManaPercent >= laneclearMinMana) {
                if (UsageQ && Q.IsReady() && minion.IsValidTarget()) 
                {
                    Q.Cast(minion); 
                }
                if (UsageW && W.IsReady() && minion.IsValidTarget()) 
                {
                    W.Cast(minion); 
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

        static void Loading_OnLoadingComplete(EventArgs args)
        {
            Chat.Print("5.25 - SP Khazix Loaded");
            Q = new Spell.Targeted(SpellSlot.Q, 325);
            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear);
            E = new Spell.Skillshot(SpellSlot.E, 600, SkillShotType.Linear);
            R = new Spell.Active(SpellSlot.R);
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += OnDraw;
            MenuKha();
            var SkinSelect = MiscMenu["skin"].Cast<Slider>().CurrentValue;
            Kha.SetSkinId(SkinSelect);
        }

        private static void OnDraw(EventArgs args)
        {
            // Draw range circles of our spells
            if (DrawMenu["drawq"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.Red, Q.Range, Player.Instance.Position);
            }
            if (DrawMenu["draww"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.Yellow, W.Range, Player.Instance.Position);
            }
            if (DrawMenu["drawe"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.Blue, E.Range, Player.Instance.Position);
            }
        }

    }
}
