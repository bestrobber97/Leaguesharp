using LeagueSharp;
using LeagueSharp.SDK.Core;
using LeagueSharp.SDK.Core.Extensions;
using LeagueSharp.SDK.Core.Extensions.SharpDX;
using LeagueSharp.SDK.Core.Math;
using LeagueSharp.SDK.Core.Math.Polygons;
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

    internal partial class EarlyList
    {
        public string ChampionName { get; set; }
        public string SpellName { get; set; }

        public int Width { get; set; }
        public float Range { get; set; }
        public System.Drawing.Color Color { get; set; }
    }
    internal class EarlyEvade
    {
        public readonly List<EarlyList> EarlyList = new List<EarlyList>();
        private static Menu root = Program.root;
        private static Menu draw = Program.draw;
        public EarlyEvade()
        {
            this.Load();

            Menu EE = new Menu("Early Evade", "EarlyEvade");
            EE.Add(new MenuSeparator("EESettings", "Early Evade Settings"));
            EE.Add(new MenuBool("Enabled", "Enabled", true));
            EE.Add(new MenuBool("drawline", "Draw Line", true));
            EE.Add(new MenuBool("drawtext", "Draw Text", true));

            foreach (var e in GameObjects.EnemyHeroes)
            {
                foreach (var eList in this.EarlyList)
                {
                    if (eList.ChampionName == e.ChampionName)
                    {
                        EE.Add(new MenuBool(eList.ChampionName + eList.SpellName, eList.ChampionName + eList.SpellName, true));
                    }

                    if (e.ChampionName == "Vayne")
                    {
                        var vayne = new MenuBool("VayneE", "Vayne E", true);
                    }
                }
            }

            draw.Add(EE);

            Drawing.OnDraw += Drawing_OnDraw;

        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!draw["Early Evade"]["Enabled"].GetValue<MenuBool>().Value)
            {
                return;
            }

            if (draw["Early Evade"]["VayneE"] != null)
            {
                foreach (var e in GameObjects.EnemyHeroes.Where(e => e.ChampionName.ToLower() == "vayne" && e.Distance(Program.Player.Position) < 900))
                {
                    for (var i = 1; i < 8; i++)
                    {
                        var championBehind = ObjectManager.Player.Position
                                             + Vector3.Normalize(e.ServerPosition - ObjectManager.Player.Position)
                                             * (-i * 50);
                        if (draw["Early Evade"]["drawline"].GetValue<MenuBool>().Value)
                        {
                            Drawing.DrawCircle(championBehind, 35f, championBehind.IsWall() ? System.Drawing.Color.Red : System.Drawing.Color.Gray);
                        }
                    }
                }
            }

            foreach (var e in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(2000)))
            {
                foreach (var eList in this.EarlyList)
                {
                    if (eList.ChampionName == e.ChampionName)
                    {
                        if (draw["Early Evade"][eList.ChampionName + eList.SpellName].GetValue<MenuBool>().Value)
                        {
                            var xminions = 0;
                            if (e.IsValidTarget(eList.Range))
                            {
                                for (var i = 1;
                                     i < e.Position.Distance(ObjectManager.Player.Position) / eList.Width;
                                     i++)
                                {
                                    var championBehind = ObjectManager.Player.Position
                                                         + Vector3.Normalize(
                                                             e.ServerPosition - ObjectManager.Player.Position)
                                                         * (i * eList.Width);

                                    var list = eList;
                                    var allies = GameObjects.AllyHeroes.Where(a => a.Distance(ObjectManager.Player.Position) < list.Range);
                                    var minions = GameObjects.AllyMinions.Where(x => x.IsValidTarget(eList.Range));
                                    var mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(eList.Range));

                                    xminions += minions.Count(m => m.Distance(championBehind) < eList.Width)
                                                + allies.Count(a => a.Distance(championBehind) < eList.Width)
                                                + mobs.Count(m => m.Distance(championBehind) < eList.Width);
                                }

                                if (xminions == 0)
                                {
                                    if (draw["Early Evade"]["drawline"].GetValue<MenuBool>().Value)
                                    {
                                        var rec = new LeagueSharp.SDK.Core.Math.Polygons.Rectangle(ObjectManager.Player.Position, e.Position, eList.Width - 10);
                                        rec.Draw(eList.Color, 2);
                                    }

                                    if (draw["Early Evade"]["drawtext"].GetValue<MenuBool>().Value)
                                    {
                                        Vector3[] x = new[] { ObjectManager.Player.Position, e.Position };
                                        var aX =
                                            Drawing.WorldToScreen(
                                                new Vector3(
                                                    Helper.CenterOfVectors(x).X,
                                                    Helper.CenterOfVectors(x).Y,
                                                    Helper.CenterOfVectors(x).Z));

                                        Drawing.DrawText(aX.X - 15, aX.Y - 15, eList.Color, eList.ChampionName + eList.SpellName);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Load()
        {
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "LeBlanc",
                    SpellName = "E",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.DarkViolet
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Morgana",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.Violet
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Blitzcrank",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.LightGoldenrodYellow
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Amumu",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.LimeGreen
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Braum",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.LightCyan
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Ezreal",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.LightYellow
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Brand",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.DarkGoldenrod
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Corki",
                    SpellName = "R",
                    Width = 75,
                    Range = 1500,
                    Color = System.Drawing.Color.WhiteSmoke
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Karma",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1000,
                    Color = System.Drawing.Color.BlanchedAlmond
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "LeeSin",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1000,
                    Color = System.Drawing.Color.LightSkyBlue
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Lux",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1000,
                    Color = System.Drawing.Color.LightSlateGray
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Nautilius",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1000,
                    Color = System.Drawing.Color.Black
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Nidalee",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1500,
                    Color = System.Drawing.Color.Beige
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Quinn",
                    SpellName = "Q",
                    Width = 75,
                    Range = 850,
                    Color = System.Drawing.Color.DarkBlue
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "TahmKench",
                    SpellName = "Q",
                    Width = 75,
                    Range = 850,
                    Color = System.Drawing.Color.HotPink
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Thresh",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.DarkSeaGreen
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Zyra",
                    SpellName = "E",
                    Width = 75,
                    Range = 900,
                    Color = System.Drawing.Color.AliceBlue
                });
        }

    }
}
