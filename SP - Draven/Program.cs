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
namespace SP___Draven
{
    class Program
    {
        static AIHeroClient Draven { get { return ObjectManager.Player; } }
        static Spell.Active Q; //I said to you to change to Active...
        static Spell.Active W; //I said to you to change to Active...
        static Spell.Skillshot E; //Draven E is Targeted ??
        static Spell.Skillshot R; //Draven R is Targeted ??

        public static Menu DraMenu;
        private static Menu ComboMenu;
        private static Menu LaneCMenu;
        private static Menu HarassMenu;

        static void Main(string[] args) { Loading.OnLoadingComplete += Loading_OnLoadingComplete; }

        static void Loading_OnLoadingComplete(EventArgs args)
        {
            Chat.Print("5.23 - SP Draven Loaded");
            Player.SetSkinId(2);
            MenuDraven();
            Q = new Spell.Active(SpellSlot.Q);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 1050, SkillShotType.Linear);
            R = new Spell.Skillshot(SpellSlot.R, uint.MaxValue, SkillShotType.Linear); //wtf man why 1050 of range ? Draven R has global range

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += OnDraw;
        }
   
         static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ComboDraven();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                HarassDraven();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneDraven();
            }
        }

        static void MenuDraven()
        {
            DraMenu = MainMenu.AddMenu("SP-Draven", "SP-Draven");
            DraMenu.AddGroupLabel("SP-Draven - Pool Party Draaaaven");
            DraMenu.AddSeparator();
            DraMenu.AddLabel("SP-Draven v1.0.0.0");
            // COMBO
            ComboMenu = DraMenu.AddSubMenu("Combo", "combod");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.AddSeparator();
            ComboMenu.Add("usecomboq", new CheckBox("Use Q"));
            ComboMenu.Add("usecombow", new CheckBox("Use W"));
            ComboMenu.Add("usecomboe", new CheckBox("Use E"));
            ComboMenu.Add("usecombor", new CheckBox("Use R"));
            // HARASS
            HarassMenu = DraMenu.AddSubMenu("Harass", "harassd");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.AddSeparator();
            HarassMenu.Add("useharassq", new CheckBox("Use Q"));
            HarassMenu.Add("useharasse", new CheckBox("Use E"));
            // LANE
            LaneCMenu = DraMenu.AddSubMenu("Lane Clear", "lanecleard");
            LaneCMenu.AddGroupLabel("Lane Clear Settings");
            LaneCMenu.AddSeparator();
            LaneCMenu.Add("uselcq", new CheckBox("Use Q"));
        }

        static void ComboDraven()
        {
            var combodr = TargetSelector.GetTarget(E.Range, DamageType.Physical);

            if (combodr == null) return;

            if (combodr.IsValidTarget()) //Why too many "combodr.IsValidTarget()" ?? just one
            {
                if (ComboMenu["usecomboe"].Cast<CheckBox>().CurrentValue && E.IsReady())
                {
                    E.Cast(combodr); // rakibi sersemlet & yavaşlat
                }

                if (ComboMenu["usecombor"].Cast<CheckBox>().CurrentValue && R.IsReady())
                {
                    R.Cast(combodr); // ultiyi bas
                }

                if (ComboMenu["usecomboq"].Cast<CheckBox>().CurrentValue && Q.IsReady() && combodr.IsValidTarget(Draven.GetAutoAttackRange())) //Checks if our target is in AA range
                {
                    Q.Cast(); // q ile stack kas
                }

                if (ComboMenu["usecombow"].Cast<CheckBox>().CurrentValue && W.IsReady())
                {
                    W.Cast(); // rakibin üzerine yaklaş
                }

            }

        }
        static void HarassDraven()
        {
            var harassdr = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            AIHeroClient harassdrq = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (harassdr == null) return;
            if (harassdrq == null) return;
            if (HarassMenu["useharasse"].Cast<CheckBox>().CurrentValue && harassdr.IsValidTarget() && E.IsReady())
            {
                E.Cast(harassdr);
            }
            if (HarassMenu["useharassq"].Cast<CheckBox>().CurrentValue && Q.IsReady() && harassdr.IsValidTarget(Draven.GetAutoAttackRange())) //Checks if our target is in AA range
            {
                Q.Cast(); // q ile stack kas
            }
        }
        static void LaneDraven()
        {
            var minion = (Obj_AI_Minion)GetEnemy(Program.Q.Range, GameObjectType.obj_AI_Minion);
            if (minion == null) return;
                if (LaneCMenu["uselcq"].Cast<CheckBox>().CurrentValue && Q.IsReady() && minion.IsValidTarget(Draven.GetAutoAttackRange())) //Checks if our target is in AA range
                {
                    Q.Cast(); // q ile stack kas
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
            // Draw range circles of our spells
            Circle.Draw(Color.Red, E.Range, Player.Instance.Position);
            Circle.Draw(Color.Yellow, Q.Range, Player.Instance.Position);
            // TODO: Uncomment if you want those enabled aswell, but remember to enable them
            // TODO: in the SpellManager aswell, otherwise you will get a NullReferenceException
            //Circle.Draw(Color.Red, SpellManager.W.Range, Player.Instance.Position);
            //Circle.Draw(Color.Red, SpellManager.E.Range, Player.Instance.Position);
            //Circle.Draw(Color.Red, SpellManager.R.Range, Player.Instance.Position);
        }
    }
}
