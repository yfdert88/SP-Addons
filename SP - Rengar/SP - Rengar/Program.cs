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
        static AIHeroClient Rengar { get { return ObjectManager.Player; } }
        public static Menu RengarM, LaneCMenu, ComboMenu, MiscMenu, DrawMenu, JungMenu;
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
            RengarM.AddGroupLabel("OneShoot, Snare or AP Combo");
            RengarM.AddGroupLabel("Q-W-E = Jungle Clear");
            RengarM.AddGroupLabel("Q-W-E = Lane Clear");
            RengarM.AddGroupLabel("E = Flee");
            RengarM.AddGroupLabel("Auto W");
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
            ComboMenu.AddLabel("OneShoot = 1 || Snare = 2 || AP Combo = 3");
            ComboMenu.Add("combomode", new Slider("Combo Mode", 1, 1, 3));
            var switcher = ComboMenu.Add("Switcher", new KeyBind("Combo Switcher", false, KeyBind.BindTypes.HoldActive, (uint)'G'));
            switcher.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
            {
                if (args.NewValue == true)
                {
                    var cast = ComboMenu["combomode"].Cast<Slider>();
                    if (cast.CurrentValue == cast.MaxValue)
                    {
                        cast.CurrentValue = 0;
                    }
                    else
                    {
                        cast.CurrentValue++;
                    }
                }
            };
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

        static void Botrk(AIHeroClient Target)
        {
            if (Item.HasItem(3144) && Item.CanUseItem(3144)) // Bilgewater
                Item.UseItem(3144, Target);
            if (Item.HasItem(3153) && Item.CanUseItem(3153)) // BOTRK's
                Item.UseItem(3153, Target);
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
            AutoW();
        }

        static void ComboChecked()
        {
            var ComboMode = ComboMenu["combomode"].Cast<Slider>().CurrentValue;
            if (ComboMode == 1)
            {
                OneShootCombo();
            }
            if (ComboMode == 2)
            {
                ComboSnare();
            }
            if (ComboMode == 3)
            {
                APCombo();
            }
        }

        static void ComboSnare()
        {
            var RQ = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var TakeYoumu = TargetSelector.GetTarget(Rengar.GetAutoAttackRange() + 700, DamageType.Physical);
            var RW = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            var Takebotrk = TargetSelector.GetTarget(550, DamageType.Physical);
            var RE = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var ePred = E.GetPrediction(RE);
            var e = ePred.CollisionObjects;
            if (Player.HasBuff("RengarR"))
            {
                return;
            }
            if (Takebotrk.IsValidTarget())
            {
                Botrk(Takebotrk);
            }
            if (RQ.IsValidTarget(Rengar.GetAutoAttackRange()))
            {
                Items();
            }
            // COMBO LOGİC -START-
            if (Player.Instance.ManaPercent == 5) 
            {
                var style = MiscMenu["style"].Cast<Slider>().CurrentValue;
                if (style == 0)
                {
                    if (ePred.HitChance >= HitChance.Low)
                    {
                        Orbwalker.DisableAttacking = false;
                        E.Cast(RE);
                    }
                    else
                    {
                        Orbwalker.DisableAttacking = true;
                    }
                }
                if (style == 1)
                {
                    if (ePred.HitChance >= HitChance.Medium)
                    {
                        Orbwalker.DisableAttacking = false;
                        E.Cast(RE);
                    }
                    else
                    {
                        Orbwalker.DisableAttacking = true;
                    }
                }
                if (style == 2)
                {
                    if (ePred.HitChance >= HitChance.High)
                    {
                        Orbwalker.DisableAttacking = false;
                        E.Cast(RE);
                    }
                    else
                    {
                        Orbwalker.DisableAttacking = true;
                    }
                }

            }
            else
            {
                var HealthW = MiscMenu["healthw"].Cast<Slider>().CurrentValue;
                if (!E.IsReady() && Q.IsReady() && Rengar.HealthPercent != HealthW && Player.Instance.Mana == 5)
                {
                    Q.Cast();
                    
                }
            }
        // COMBO LOGİC -FİNİSH- 
            if (TakeYoumu.IsValidTarget())
            {
                Youmu();
            }
            if (!E.IsReady() && !Player.Instance.HasBuff("rengarpassivebuff") && RQ.IsValidTarget() && Q.IsReady())
            {
                Q.Cast();
                Items();
                Orbwalker.ResetAutoAttack();
            }
            if (!E.IsReady() && RW.IsValidTarget() && W.IsReady())
            {
                W.Cast(RW);
                
            }
            if (RE.IsValidTarget(E.Range))
            {
                E.Cast(RE);
            }
        }

        static void Flee()
        {
            var RE = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var Takebotrk = TargetSelector.GetTarget(550, DamageType.Physical);
            var ePred = E.GetPrediction(RE);
            var e = ePred.CollisionObjects;
            if (Takebotrk.IsValidTarget())
            {
                Botrk(Takebotrk);
            }
            if (RE.IsValidTarget() && E.IsReady())
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
        }

        static void OneShootCombo()
        {
            var RQ = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var TakeYoumu = TargetSelector.GetTarget(800, DamageType.Physical);
            var Takebotrk = TargetSelector.GetTarget(550, DamageType.Physical);
            var RW = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            var RE = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var ePred = E.GetPrediction(RE);
            var e = ePred.CollisionObjects;
            if (Player.HasBuff("RengarR"))
            {
                return;
            }
            if (Takebotrk.IsValidTarget())
            {
                Botrk(Takebotrk);
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
            if (RE.HealthPercent < 50 && RE.IsValidTarget())
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
    }
    
        static void APCombo()
        {
            var RQ = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var TakeYoumu = TargetSelector.GetTarget(800, DamageType.Physical);
            var Takebotrk = TargetSelector.GetTarget(550, DamageType.Physical);
            var RW = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            var RE = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var ePred = E.GetPrediction(RE);
            var e = ePred.CollisionObjects;
            if (Player.HasBuff("RengarR"))
            {
                return;
            }
            if (Takebotrk.IsValidTarget())
            {
                Botrk(Takebotrk);
            }
            if (Player.Instance.Mana <= 5 && RW.IsValidTarget() && W.IsReady())
            {
                W.Cast(RW);

            }
            if (!Q.IsReady() && RQ.IsValidTarget(Rengar.GetAutoAttackRange()))
            {
                Items();
            }

            if (!W.IsReady() && Q.IsReady() && Player.Instance.Mana < 5 && RQ.IsValidTarget())
            {
                Q.Cast();

            }
            if (TakeYoumu.IsValidTarget())
            {
                Youmu();
            }
            if (!Player.Instance.HasBuff("rengarpassivebuff") && RW.IsValidTarget() && W.IsReady())
            {
                W.Cast();
                Items();
                Orbwalker.ResetAutoAttack();
            }
            if (!W.IsReady() && RE.IsValidTarget())
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
            if (RE.HealthPercent < 50 && RE.IsValidTarget())
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
        }

        static void LaneRengo()
        {
            /*var minionq = (Obj_AI_Minion)GetEnemy(Program.Q.Range, GameObjectType.obj_AI_Minion);
            var minionw = (Obj_AI_Minion)GetEnemy(Program.W.Range, GameObjectType.obj_AI_Minion);
            var minione = (Obj_AI_Minion)GetEnemy(Program.E.Range, GameObjectType.obj_AI_Minion);*/
            var UsageQ = LaneCMenu["uselcq"].Cast<CheckBox>().CurrentValue;
            var UsageW = LaneCMenu["uselcw"].Cast<CheckBox>().CurrentValue;
            var UsageE = LaneCMenu["uselce"].Cast<CheckBox>().CurrentValue;
            //if (minionq == null) return;
            var target =
                EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(x => !x.IsDead && Q.IsInRange(x));
            var targetw =
                EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(x => !x.IsDead && W.IsInRange(x));
            var targete =
                EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(x => !x.IsDead && E.IsInRange(x));
            var savest = LaneCMenu["savestack"].Cast<CheckBox>().CurrentValue;
            if (Player.Instance.Mana < 5 || (Player.Instance.Mana == 5 && !savest))
            {
                if (target.IsValidTarget(Rengar.GetAutoAttackRange()))
                {
                Itemsminion();
                }
                
                if (UsageQ && Q.IsReady() && target.IsValidTarget() && (QDamage(target) >= target.Health))
                {
                    Q.Cast();
                    Orbwalker.ResetAutoAttack();
                    
                }
                if (UsageW && W.IsReady() && targetw.IsValidTarget())
                {
                    W.Cast(targetw);
                }
                if (UsageE && E.IsReady() && targete.IsValidTarget())
                {
                    E.Cast(targete);
                }
            }
        }


        public static double QDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.Q).IsLearned) return 0;
            return Rengar.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new double[] { 30 , 60 , 90 , 120 , 150 }[Program.Q.Level - 1] + 0.4 * Rengar.FlatMagicDamageMod));
        }

        static void JungR()
        {
            var UsageQ = JungMenu["usejcq"].Cast<CheckBox>().CurrentValue;
            var UsageW = JungMenu["usejcw"].Cast<CheckBox>().CurrentValue;
            var UsageE = JungMenu["usejce"].Cast<CheckBox>().CurrentValue;
            var savest = JungMenu["savestack"].Cast<CheckBox>().CurrentValue;
            foreach (var monster in EntityManager.MinionsAndMonsters.Monsters)
                {
                if (Player.Instance.Mana < 5 || (Player.Instance.Mana == 5 && !savest))
                 {
                
                    if (Rengar.Distance(monster) < Rengar.AttackRange && UsageQ && Q.IsReady())
                    {
                        Q.Cast();
                        Orbwalker.ResetAutoAttack();
                        Itemsminion();
                    }
                    if (UsageW && W.IsReady() && Rengar.Distance(monster) <= W.Range)
                    {
                        W.Cast(monster);
                    }
                    if (UsageE && E.IsReady() && Rengar.Distance(monster) <= E.Range)
                    {
                        E.Cast(monster);
                    }
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
                    if (Combo == 3)
                    {
                        Drawing.DrawText(Drawing.WorldToScreen(Player.Instance.Position) - new Vector2(0, 34), Color.White, "AP Combo", 2);
                    }
                }
            }
        }
    }
}
