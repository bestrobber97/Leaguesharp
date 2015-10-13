using LeagueSharp.SDK.Core.UI.IMenu.Values;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Menu = LeagueSharp.SDK.Core.UI.IMenu.Menu;

namespace HastaKalistaBaby
{
    internal class MenuManager
    {
        private static Menu root = Program.root;
        private static Menu draw = Program.draw;
        public static void Create()
        {
            var q = new Menu("spell.q", "Q Settings");
            {
                q.Add(new MenuSeparator("Qsetting", "Q Settings"));
                q.Add(new MenuBool("AutoQ", "Enable Q", true));
                q.Add(new MenuBool("AutoQH", "Auto Q Harass", true));
                q.Add(new MenuBool("AutoQM", "Auto Q Across Minions", true));
                root.Add(q);
            }

            var w = new Menu("spell.w", "W Settings");
            {
                w.Add(new MenuSeparator("Wsetting", "W Settings"));
                w.Add(new MenuBool("AutoW", "Auto W", true));
                w.Add(new MenuBool("WBaron", "W on Baron", true));
                w.Add(new MenuBool("WDrake", "W on Drake", true));
                root.Add(w);
            }

            var e = new Menu("spell.e", "E Settings");
            {
                e.Add(new MenuSeparator("Esetting", "E Settings"));
                e.Add(new MenuBool("AutoEChamp", "Auto E On Champions", true));
                e.Add(new MenuSeparator("jEsetting", "Jungle Settings"));
                e.Add(new MenuBool("BlueM", "Auto E Blue", true));
                e.Add(new MenuBool("RedM", "Auto E Red", true));
                e.Add(new MenuBool("BaronM", "Auto E Baron", true));
                e.Add(new MenuBool("DrakeM", "Auto E Dragon", true));
                e.Add(new MenuBool("SmallM", "Auto E Smalls", false));
                e.Add(new MenuBool("OtherM", "Auto E Gromp/Wolf/Krug/Raptor", true));
                e.Add(new MenuBool("MidM", "Auto E Crab", true));
                e.Add(new MenuSeparator("lEsetting", "LaneClear Settings"));
                e.Add(new MenuBool("AutoEMinions", "Auto E Minions", false));
                e.Add(new MenuSlider("minAutoEMinions", "Min minions", 2, 1, 5));
                e.Add(new MenuBool("BigMinionFinisher", "Auto E Big Minions", true));
                e.Add(new MenuBool("AutoEMinionsTower", "Auto E Big Minions Under Tower", true));
                root.Add(e);
            }

            var r = new Menu("spell.r", "R Settings");
            {
                r.Add(new MenuSeparator("Rsetting", "R Settings"));
                r.Add(new MenuBool("AutoR", "Auto R Saver", true));
                r.Add(new MenuBool("KBS", "Auto R BlitzCrank/Skarner/Kench", true));
                root.Add(r);
            }

            var item = new Menu("item", "Item Settings");
            {
                item.Add(new MenuSeparator("bilgwater", "Bilgewater's Cutlass"));
                item.Add(new MenuBool("bilg", "Bilgewater's Cutlass", true));
                item.Add(new MenuSlider("enemybilg", "Use on Enemy HP % <=", 90, 0, 100));
                item.Add(new MenuSlider("selfbilg", "Use on Self HP % <=", 25, 0, 100));
                item.Add(new MenuSeparator("botrk", "Blade of the Ruined King"));
                item.Add(new MenuBool("Botkr", "Blade of the Ruined King", true));
                item.Add(new MenuSlider("enemyBotkr", "Use on Enemy HP % <=",90,0,100));
                item.Add(new MenuSlider("selfBotkr", "Use on Self HP % <=", 25, 0, 100));
                item.Add(new MenuSeparator("Youmus", "Youmuus Ghostblade"));
                item.Add(new MenuBool("youm", "Youmuus Ghostblade", true));
                item.Add(new MenuSlider("enemyYoumuus","Use on Enemy HP % <=", 95, 0, 100));
                item.Add(new MenuSlider("selfYoumuus", "Use on Self HP % <=", 95, 0, 100));
                root.Add(item);
            }

            {
                draw.Add(new MenuSeparator("Dsetting", "Drawing Settings"));
                draw.Add(new MenuBool("Qrange", "Draw Q Range", false));
                draw.Add(new MenuBool("Wrange", "Draw W Range", false));
                draw.Add(new MenuBool("Erange", "Draw E Range", true));
                draw.Add(new MenuBool("Rrange", "Draw R Range", false));
                draw.Add(new MenuBool("healthp", "Show Health Percent", true));
                draw.Add(new MenuColor("colorp", "Percent Color", new ColorBGRA(245, 245, 245, 255)));
                draw.Add(new MenuBool("Target", "Draw Current Target", true));
                draw.Add(new MenuBool("TargetA", "Draw Target Attack Range", true));
                draw.Add(new MenuBool("Minionh", "Draw killable minions", true));
                root.Add(draw);
            }

            var lvl = new Menu("lvl", "Level Settigns");
            {
                lvl.Add(new MenuSeparator("Dsetting", "Level Settings"));
                lvl.Add(new MenuBool("Lvlon", "Enable Level Up", true));
                lvl.Add(new MenuList<String>("1", "1", new String[] { "Q", "W", "E", "R" }));
                lvl.Add(new MenuList<String>("2", "2", new String[] { "Q", "W", "E", "R" }));
                lvl.Add(new MenuList<String>("3", "3", new String[] { "Q", "W", "E", "R" }));
                lvl.Add(new MenuList<String>("4", "4", new String[] { "Q", "W", "E", "R" }));
                lvl.Add(new MenuSlider("s", "Start at level", 2, 1, 5));
                root.Add(lvl);
            }

            var ex = new Menu("ExploitOP", "Exploit Settings");
            {
                ex.Add(new MenuSeparator("EXsetting", "Exploit Settings"));
                ex.Add(new MenuBool("Fly", "Activate Exploit", false));
                root.Add(ex);
            }

            root.Attach();
        }
    }
}
