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
namespace ConsoleApplication1
{
	class Program
	{
		static Spell.Targeted Q;
		static Spell.Targeted W;
		static Spell.Targeted E;
		static Spell.Targeted R;
		static void Main(string[] args)
		{
			Loading.OnLoadingComplete += Loading_OnLoadingComplete;
		}

		static void Loading_OnLoadingComplete(EventArgs args)
		{
			Chat.Print("5.23 - SP Annie Loaded");
            Player.SetSkinId(8);
			Q = new Spell.Targeted(SpellSlot.Q, 625);
			W = new Spell.Targeted(SpellSlot.W, 625);
			E = new Spell.Targeted(SpellSlot.E, 625);
			R = new Spell.Targeted(SpellSlot.R, 600);
			Game.OnTick += Game_OnTick;
			Drawing.OnDraw += OnDraw;
		}

		static void Game_OnTick(EventArgs args)
		{
			if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
			{
				ComboAnnie();
			}

			if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
			{
				HarassAnnie();
			}

			if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
			{
				LastAnnie();
			}
		}

		static void ComboAnnie()
		{
			AIHeroClient hedef = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
			AIHeroClient hedef2 = TargetSelector.GetTarget(W.Range, DamageType.Magical);
			AIHeroClient hedef4 = TargetSelector.GetTarget(R.Range, DamageType.Magical);
			if (hedef == null) return;
			if (hedef4.IsValidTarget() && R.IsReady())
            {
                R.Cast(hedef4);
            }
            if (hedef.IsValidTarget() && Q.IsReady())
            {
                Q.Cast(hedef);
            }
            if (hedef2.IsValidTarget() && W.IsReady())
            {
                W.Cast(hedef2);
            }
        }
        static void HarassAnnie()
        {
            AIHeroClient chedef = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            AIHeroClient chedef2 = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (chedef == null) return;
            if (chedef.IsValidTarget() && Q.IsReady())
            {
                Q.Cast(chedef);
            }
            if (chedef2.IsValidTarget() && W.IsReady())
            {
                W.Cast(chedef2);
            }
        }
        static void LastAnnie()
        {
            AIHeroClient lhedef = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (lhedef == null) return;
            if (lhedef.IsValidTarget() && Q.IsReady())
            {
                Q.Cast(lhedef);
            }
        }
        private static void OnDraw(EventArgs args)
        {
            // Draw range circles of our spells
            Circle.Draw(Color.Red, Q.Range, Player.Instance.Position);
            Circle.Draw(Color.Blue, R.Range, Player.Instance.Position);
            // TODO: Uncomment if you want those enabled aswell, but remember to enable them
            // TODO: in the SpellManager aswell, otherwise you will get a NullReferenceException
            //Circle.Draw(Color.Red, SpellManager.W.Range, Player.Instance.Position);
            //Circle.Draw(Color.Red, SpellManager.E.Range, Player.Instance.Position);
            //Circle.Draw(Color.Red, SpellManager.R.Range, Player.Instance.Position);
        }
    }
}
