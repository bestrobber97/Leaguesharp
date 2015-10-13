using LeagueSharp;
using LeagueSharp.SDK.Core.UI.IMenu.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Menu = LeagueSharp.SDK.Core.UI.IMenu.Menu;

namespace HastaKalistaBaby
{
     internal class AutoLevel
    {
        private static Menu root = Program.root;
        static int  Q, W, E, R;

        public static void Init()
        {
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnUpdate(EventArgs args)
        {
            Q = root["lvl"]["1"].GetValue<MenuList>().Index;
            W = root["lvl"]["2"].GetValue<MenuList>().Index;
            E = root["lvl"]["3"].GetValue<MenuList>().Index;
            R = root["lvl"]["4"].GetValue<MenuList>().Index;
            if(SameValues())
            {
                return;
            }
        }

        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe || !root["lvl"]["Lvlon"].GetValue<MenuBool>().Value || Program.Player.Level < root["lvl"]["s"].GetValue<MenuSlider>().Value || SameValues())
            {
                return;
            }
            lvl(Q);
            lvl(W);
            lvl(E);
            lvl(R);
        }

        private static void OnDraw(EventArgs args)
        {
            if(SameValues())
            {
                Drawing.DrawText(Drawing.WorldToScreen(Program.Player.Position).X,Drawing.WorldToScreen(Program.Player.Position).Y - 10,System.Drawing.Color.OrangeRed,"Wrong Ability Sequence");
            }
        }

        private static void lvl(int i)//Inspired By seb oktw
        {
            if (Program.Player.Level < 4)
            {
                if (i == 0 && Program.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (i == 1 && Program.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (i == 2 && Program.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.E);
            }
            else
            {
                if (i == 0)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (i == 1)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (i == 2)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (i == 3)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }

        private static bool SameValues()
        {
            if (Q == W || Q == E || Q == R || W == E || W == R || E == R)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
