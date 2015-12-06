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
        public static Menu KhaMenu;
        private static Menu ComboMenu;
        private static Menu LaneCMenu;
        private static Menu HarassMenu;
        static AIHeroClient Kha { get { return ObjectManager.Player; } }
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var hedef = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                Combo(hedef);
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

        static void Combo(Obj_AI_Base target)
        {

            var khaq = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var khaw = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var minionkhaw = TargetSelector.GetTarget(300, DamageType.Physical);
            var khae = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var UsageItems = ComboMenu["useitems"].Cast<CheckBox>().CurrentValue;
            var UsageQ = ComboMenu["usecomboq"].Cast<CheckBox>().CurrentValue;
            var UsageW = ComboMenu["usecombow"].Cast<CheckBox>().CurrentValue;
            var UsageE = ComboMenu["usecomboe"].Cast<CheckBox>().CurrentValue;
            var UsageR = ComboMenu["usecombor"].Cast<CheckBox>().CurrentValue;
            var WPred = W.GetPrediction(minionkhaw);
            if (target.IsValidTarget(800) && UsageR && R.IsReady())
                {
                    R.Cast();
                }
            if (UsageItems)
                {
                    Items();
                }
            if (target.IsValidTarget(600) && Q.IsReady() && E.IsReady() && W.IsReady())
            {
                
                
                if (UsageE)
                {
                    E.Cast(W.GetPrediction(target).CastPosition);
                }
                if (UsageQ)
                {
                    Q.Cast(W.GetPrediction(target).CastPosition);
                }
                if (WPred.HitChance >= HitChance.Medium && !WPred.CollisionObjects.Any(it => it.Name.ToLower().Contains("minion")))
                {
                    if (UsageW)
                    {
                        W.Cast(W.GetPrediction(target).CastPosition);
                    }
                    Core.DelayAction(() => E.Cast(target), W.CastDelay + Game.Ping);

                    var EDelay = (int)((Kha.Distance(target) / Kha.Spellbook.GetSpell(SpellSlot.E).SData.MissileSpeed * 1000) + E.CastDelay);

                    Core.DelayAction(() => Q.Cast(target), EDelay);
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

                    if (khaw.IsValidTarget() && W.IsReady())
                    {
                        if (WPred.HitChance >= HitChance.Medium && !WPred.CollisionObjects.Any(it => it.Name.ToLower().Contains("minion")))
                        {
                            if (UsageW) {
                                W.Cast(khaw);
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
            Chat.Print("5.23 - SP Khazix Loaded");
            Q = new Spell.Targeted(SpellSlot.Q, 325);
            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear);
            E = new Spell.Skillshot(SpellSlot.E, 600, SkillShotType.Linear);
            R = new Spell.Active(SpellSlot.R);
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += OnDraw;
            MenuKha();
        }

        private static void OnDraw(EventArgs args)
        {
            // Draw range circles of our spells
            Circle.Draw(Color.Red, Q.Range, Player.Instance.Position);
            Circle.Draw(Color.Yellow, W.Range, Player.Instance.Position);
            Circle.Draw(Color.Blue, E.Range, Player.Instance.Position);
        }

    }
}
