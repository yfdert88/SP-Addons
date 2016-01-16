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

namespace SP___Rengar
{
    class Program
    {
        static Spell.Active Q;
        static Spell.Skillshot W;
        static Spell.Skillshot E;
        static Spell.Active R;
        static bool savestack;
        static AIHeroClient Rengar { get { return ObjectManager.Player; } }
        public static Menu RengarM, HarassMenu, LaneCMenu, ComboMenu, MiscMenu, DrawMenu, JungMenu;
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        static void Loading_OnLoadingComplete(EventArgs args)
        {
            Chat.Print("6.1 - SP Rengar Loaded");
            Q = new Spell.Active(SpellSlot.Q, 150);
            W = new Spell.Skillshot(SpellSlot.W, 500, SkillShotType.Circular);
            E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Linear);
            R = new Spell.Active(SpellSlot.R);
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += OnDraw;
            Player.SetSkinId(2);
            MenuRengo();
            
        }

        static void MenuRengo()
        {
            RengarM = MainMenu.AddMenu("SP-Rengar", "SP-Rengar");
            RengarM.AddGroupLabel("SP-Rengar");
            RengarM.AddSeparator();
            RengarM.AddGroupLabel("Q-W-E = Combo");
            RengarM.AddGroupLabel("E-W-Q = Harass");
            RengarM.AddGroupLabel("W-Q-E = Lane Clear");
            RengarM.AddGroupLabel("İn the air Hydra,Tiamat, Item's Usage");
            RengarM.AddLabel("SP-Rengar v1.0.0.2");
            // COMBO
            ComboMenu = RengarM.AddSubMenu("Combo", "combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.AddSeparator();
            ComboMenu.Add("usecomboq", new CheckBox("Use Q"));
            ComboMenu.Add("usecombow", new CheckBox("Use W"));
            ComboMenu.Add("usecomboe", new CheckBox("Use E"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("useitems", new CheckBox("Use Items"));
            ComboMenu.AddLabel("OneShoot = 1 || Snare = 2");
            ComboMenu.Add("combomode", new Slider("Combo Mode", 1, 1, 2));
            // LANE
            LaneCMenu = RengarM.AddSubMenu("Lane Clear Settings", "laneclear");
            LaneCMenu.AddGroupLabel("Lane Clear Settings");
            LaneCMenu.AddSeparator();
            LaneCMenu.Add("uselcq", new CheckBox("Use Q"));
            LaneCMenu.Add("uselcw", new CheckBox("Use W"));
            LaneCMenu.Add("uselce", new CheckBox("Use E"));
            LaneCMenu.Add("savestack", new CheckBox("Save 5 STACK"));
            // JUNG
            JungMenu = RengarM.AddSubMenu("Jungle Settings", "jungclear");
            JungMenu.AddGroupLabel("Jungle Settings");
            JungMenu.AddSeparator();
            JungMenu.Add("usejcq", new CheckBox("Use Q"));
            JungMenu.Add("usejcw", new CheckBox("Use W"));
            JungMenu.Add("usejce", new CheckBox("Use E"));
            JungMenu.Add("savestack", new CheckBox("Save 5 STACK"));
            // MİSC
            MiscMenu = RengarM.AddSubMenu("Misc", "misc");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.AddSeparator();
            var Style = MiscMenu.Add("style", new Slider("Min Prediction", 1, 0, 2));
            Style.OnValueChange += delegate
            {
                Style.DisplayName = "Min Prediction: " + new[] { "Low", "Medium", "High" }[Style.CurrentValue];
            };
            Style.DisplayName = "Min Prediction: " + new[] { "Low", "Medium", "High" }[Style.CurrentValue];
            MiscMenu.AddSeparator();
            MiscMenu.Add("healthw", new Slider("Min. health for W :", 20, 0, 100));
            // DRAW
            DrawMenu = RengarM.AddSubMenu("Drawing", "draw");
            DrawMenu.AddGroupLabel("Drawing Settings");
            DrawMenu.AddSeparator();
            DrawMenu.Add("drawoff", new CheckBox("Turn OFF All Drawings"));
            DrawMenu.Add("drawq", new CheckBox("Draw Auto Attack Range"));
            DrawMenu.Add("drawstat", new CheckBox("Draw Combo Status"));
        }

        static void Items()
        {
            if (Item.HasItem(3074) && Item.CanUseItem(3074)) // Hydra
                Item.UseItem(3074);
            if (Item.HasItem(3077) && Item.CanUseItem(3077)) // Tiamat
                Item.UseItem(3077);
            if (Item.HasItem(3748) && Item.CanUseItem(3748)) // Titanic Hydra
                Item.UseItem(3748);
        }

        static void Youmu()
        {
            if (Item.HasItem(3142) && Item.CanUseItem(3142)) // Youmuu's
                Item.UseItem(3142);
        }

        static void Itemsminion()
        {
            if (Item.HasItem(3074) && Item.CanUseItem(3074)) // Hydra
                Item.UseItem(3074);
            if (Item.HasItem(3077) && Item.CanUseItem(3077)) //Tiamat
                Item.UseItem(3077);
            if (Item.HasItem(3748) && Item.CanUseItem(3748)) // Titanic Hydra
                Item.UseItem(3748);
        }

        static void AutoW()
        {
            var HealthW = MiscMenu["healthw"].Cast<Slider>().CurrentValue;
            if (W.IsReady() && Rengar.HealthPercent <= HealthW && Player.Instance.Mana == 5)
            {
                W.Cast(Rengar);
            }
        }

        static void Game_OnTick(EventArgs args)
        {
            AutoW();
            AutoHarass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ComboChecked();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneRengo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungR();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
        }

        static void ComboChecked()
        {
            var ComboMode = ComboMenu["combomode"].Cast<Slider>().CurrentValue;
            if (ComboMode == 1)
            {
                OneShootCombo();
            }
            else
            {
                ComboSnare();
            }
        }

        static void ComboSnare()
        {
            var RQ = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var TakeYoumu = TargetSelector.GetTarget(Rengar.GetAutoAttackRange() + 700, DamageType.Physical);
            var RW = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            var RE = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var ePred = E.GetPrediction(RE);
            var e = ePred.CollisionObjects;
            if (Player.HasBuff("RengarR"))
            {
                return;
            }
            if (RQ.IsValidTarget(Rengar.GetAutoAttackRange()))
            {
                Items();
            }
            // COMBO LOGİC -START-
            if (Player.Instance.ManaPercent >= 5) 
            {
                var style = MiscMenu["style"].Cast<Slider>().CurrentValue;
                if (style == 0)
                {
                    if (ePred.HitChance >= HitChance.Low)
                    {
                        E.Cast(RE);
                    }
                }
                if (style == 1)
                {
                    if (ePred.HitChance >= HitChance.Medium)
                    {
                        E.Cast(RE);
                    }
                }
                if (style == 2)
                {
                    if (ePred.HitChance >= HitChance.High)
                    {
                        E.Cast(RE);
                    }
                }

            }
            else
            {
                var HealthW = MiscMenu["healthw"].Cast<Slider>().CurrentValue;
                if (Q.IsReady() && Rengar.HealthPercent != HealthW && Player.Instance.Mana == 5)
                {
                    Q.Cast();
                    
                }else
                {
                    W.Cast(Rengar);
                }
            }
        // COMBO LOGİC -FİNİSH- 
            if (TakeYoumu.IsValidTarget())
            {
                Youmu();
            }
            if (!Player.Instance.HasBuff("rengarpassivebuff") && RQ.IsValidTarget() && Q.IsReady())
            {
                Q.Cast();
                Items();
                Orbwalker.ResetAutoAttack();
            }
            if (RW.IsValidTarget() && W.IsReady())
            {
                W.Cast(RW);
                
            }
            if (RE.IsValidTarget(E.Range))
            {
                E.Cast(RE);
            }
        }

        static void AutoHarass()
        {
            var RW = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            var RE = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var ePred = E.GetPrediction(RE);
            var e = ePred.CollisionObjects;
            if (Player.Instance.ManaPercent < 5)
            {
                var style = MiscMenu["style"].Cast<Slider>().CurrentValue;
                if (style == 0)
                {
                    if (ePred.HitChance >= HitChance.Low)
                    {
                        E.Cast(RE);
                    }
                }
                if (style == 1)
                {
                    if (ePred.HitChance >= HitChance.Medium)
                    {
                        E.Cast(RE);
                    }
                }
                if (style == 2)
                {
                    if (ePred.HitChance >= HitChance.High)
                    {
                        E.Cast(RE);
                    }
                }
                if (RW.IsValidTarget() && W.IsReady())
                {
                    W.Cast(RW);

                }
            }

        }

        static void Flee()
        {
            var RE = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var ePred = E.GetPrediction(RE);
            var e = ePred.CollisionObjects;
            if (RE.IsValidTarget() && E.IsReady())
            {
                E.Cast(RE);
            }
        }

        static void OneShootCombo()
        {
            var RQ = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var TakeYoumu = TargetSelector.GetTarget(800, DamageType.Physical);
            var RW = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            var RE = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var ePred = E.GetPrediction(RE);
            var e = ePred.CollisionObjects;
            if (Player.HasBuff("RengarR"))
            {
                return;
            }
            if (!Q.IsReady() && RQ.IsValidTarget(Rengar.GetAutoAttackRange()))
            {
                Items();
            }

            if (Q.IsReady() && Player.Instance.Mana == 5 && RQ.IsValidTarget())
                {
                    Q.Cast();

                }
            if (TakeYoumu.IsValidTarget())
            {
                Youmu();
            }
            if (!Player.Instance.HasBuff("rengarpassivebuff") && RQ.IsValidTarget() && Q.IsReady())
            {
                Q.Cast();
                Items();
                Orbwalker.ResetAutoAttack();
            }
            if (!Q.IsReady() && RW.IsValidTarget() && W.IsReady())
            {
                W.Cast(RW);

            }
            if (!Q.IsReady() && RE.IsValidTarget())
            {
                E.Cast(RE);
            }
            if (RE.HealthPercent < 50 && RE.IsValidTarget())
            {
                E.Cast(RE);
            }
    }
    
        

        static void LaneRengo()
        {
            var minionq = (Obj_AI_Minion)GetEnemy(Program.Q.Range, GameObjectType.obj_AI_Minion);
            var minionw = (Obj_AI_Minion)GetEnemy(Program.W.Range, GameObjectType.obj_AI_Minion);
            var minione = (Obj_AI_Minion)GetEnemy(Program.E.Range, GameObjectType.obj_AI_Minion);
            var UsageQ = LaneCMenu["uselcq"].Cast<CheckBox>().CurrentValue;
            var UsageW = LaneCMenu["uselcw"].Cast<CheckBox>().CurrentValue;
            var UsageE = LaneCMenu["uselce"].Cast<CheckBox>().CurrentValue;
            if (minionq == null) return;
            
            var savest = LaneCMenu["savestack"].Cast<CheckBox>().CurrentValue;
            if (Player.Instance.Mana < 5 || (Player.Instance.Mana == 5 && !savest))
            {
                if (minionq.IsValidTarget(Rengar.GetAutoAttackRange()))
                {
                Itemsminion();
                }
                
                if (UsageQ && Q.IsReady() && minionq.IsValidTarget())
                {
                    Q.Cast();
                    Orbwalker.ResetAutoAttack();
                    
                }
                if (UsageW && W.IsReady() && minionw.IsValidTarget())
                {
                    W.Cast(minionw);
                }
                if (UsageE && E.IsReady() && minione.IsValidTarget())
                {
                    E.Cast(minione);
                }
            }
        }

        static void JungR()
        {
            Obj_AI_Base jung =
                EntityManager.MinionsAndMonsters.GetJungleMonsters(

                    ObjectManager.Player.Position,
                    600,
                    true).FirstOrDefault();
            var UsageQ = JungMenu["usejcq"].Cast<CheckBox>().CurrentValue;
            var UsageW = JungMenu["usejcw"].Cast<CheckBox>().CurrentValue;
            var UsageE = JungMenu["usejce"].Cast<CheckBox>().CurrentValue;
            var savest = JungMenu["savestack"].Cast<CheckBox>().CurrentValue;
            if (Player.Instance.Mana < 5 || (Player.Instance.Mana == 5 && !savest))
            {
                if (UsageQ && Q.IsReady() && jung.IsValidTarget())
                {
                    Q.Cast();
                    Orbwalker.ResetAutoAttack();
                    Itemsminion();
                }
                if (UsageW && W.IsReady() && jung.IsValidTarget())
                {
                    W.Cast(jung);
                }
                if (UsageE && E.IsReady() && jung.IsValidTarget())
                {
                    E.Cast(jung);
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

        private static void OnDraw(EventArgs args)
        {
            var Combo = ComboMenu["combomode"].Cast<Slider>().CurrentValue;
            // Draw range circles of our spells
            if (DrawMenu["drawoff"].Cast<CheckBox>().CurrentValue) {
                if (DrawMenu["drawq"].Cast<CheckBox>().CurrentValue)
                {
                    Drawing.DrawCircle(Player.Instance.Position, Rengar.GetAutoAttackRange(), Color.Blue);
                }
                if (DrawMenu["drawstat"].Cast<CheckBox>().CurrentValue)
                {
                    if (Combo == 1)
                    {
                        Drawing.DrawText(Drawing.WorldToScreen(Player.Instance.Position) - new Vector2(0, 34), Color.White, "OneShoot", 2);
                    }
                    if (Combo == 2)
                    {
                        Drawing.DrawText(Drawing.WorldToScreen(Player.Instance.Position) - new Vector2(0, 34), Color.White, "Snare", 2);
                    }
                }
            }
        }
    }
}
