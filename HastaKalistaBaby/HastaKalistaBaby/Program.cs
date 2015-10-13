using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK.Core;
using LeagueSharp.SDK.Core.Enumerations;
using LeagueSharp.SDK.Core.Extensions;
using LeagueSharp.SDK.Core.UI.IMenu.Values;
using LeagueSharp.SDK.Core.Wrappers;
using LeagueSharp.SDK.Core.Events;
using LeagueSharp.SDK.Core.Extensions.SharpDX;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using SharpDX.Direct3D9;

using Color = System.Drawing.Color;

using Menu = LeagueSharp.SDK.Core.UI.IMenu.Menu;
using LeagueSharp.SDK.Core.Math.Prediction;

namespace HastaKalistaBaby
{
    internal class Program
    {
        public static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static Menu root,draw;
        public static Spell Q, W, E, R;
        static Items.Item botrk = new Items.Item(3153, 550);
        static Items.Item yom = new Items.Item(3142, 750);
        static Items.Item bilgwat = new Items.Item(3144, 550);
        public static EarlyEvade ee;
        public static int wcount = 0;

        public static Font Text;
        public static float grabT = Game.Time, lastecast = 0f;
        public static double i;
        public static float time;
        public static Obj_AI_Hero soulmate = null;


        static void Main(string[] args)
        {
            Load.OnLoad += OnGameLoad;
        }

        private static void OnGameLoad(object sender, EventArgs e)
        {
            if (Player.ChampionName != "Kalista")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 1130);
            W = new Spell(SpellSlot.W, 5200);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R, 1400f);

            Q.SetSkillshot(0.25f, 30f, 1700f, true, SkillshotType.SkillshotLine);

            Text = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Arial", Height = 35, Width = 12, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Default });
            root = new Menu("hkalista", "HastaKalistaBaby", true);
            draw = new Menu("drawing", "Drawings Settings");

            MenuManager.Create();
            ee = new EarlyEvade();
            AutoLevel.Init();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Helper.OnProcessSpellCast;
            Spellbook.OnCastSpell += Helper.OnCastSpell;

        }

        public static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Orbwalk:
                    Items();
                    Qlogic();
                    if (root["ExploitOP"]["Fly"].GetValue<MenuBool>().Value)
                    {
                        var target = TargetSelector.GetTarget(Player.GetRealAutoAttackRange(),DamageType.Physical);
                        if (target.IsValidTarget())
                        {
                            if (Game.Time * 1000 >= Orbwalker.LastAutoAttackTick + 1)
                            {
                                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                            }
                            if (Game.Time * 1000 > Orbwalker.LastAutoAttackTick + Player.AttackDelay * 1000 - 180)
                            {
                                Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                            }
                        }
                        else
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        }
                    }
                    break;

                case OrbwalkerMode.LaneClear:
                    if(root["spell.q"]["AutoQH"].GetValue<MenuBool>().Value)
                    {
                        Qlogic();
                    }

                    break;

                case OrbwalkerMode.Hybrid:
                    break;
            }
            WLogic();
            RLogic();
            ELogic();
            LaneClear();
            JungleClear();
        }



        private static void Qlogic()
        {
            if(!Q.IsReady() || !root["spell.q"]["AutoQ"].GetValue<MenuBool>().Value || Helper.GetMana(Q)<80)
            {
                return;
            }

            var target = TargetSelector.GetTarget(E.Range*1.2f,DamageType.Physical);
            if (target.IsValidTarget())
            {
                var predout = Q.GetPrediction(target);
                var coll = predout.CollisionObjects;

                if (coll.Count<1)
                {
                    Q.CastIfHitchanceMinimum(target, HitChance.High);
                }
                if(coll.Count == 1 && root["spell.q"]["AutoQM"].GetValue<MenuBool>().Value)
                {
                    foreach (var c in coll)
                    {
                        if (Damage.GetEdamage(c) > c.Health)
                        {
                            Q.Cast(predout.CastPosition);
                        }
                    }
                }


            }
        }

        private static void WLogic()
        {
            if(!W.IsReady() && Helper.GetMana(W)<80)
            {
                return;
            }

            if (root["spell.w"]["AutoW"].GetValue<MenuBool>().Value && Helper.CountEnemy(Player.Position,1500) == 0)
            {
                if (wcount > 0)
                {
                    Vector3 baronPos;
                    baronPos.X = 5232;
                    baronPos.Y = 10788;
                    baronPos.Z = 0;
                    if (Player.Distance(baronPos) < 5000 && root["spell.w"]["WBaron"].GetValue<MenuBool>().Value)
                        W.Cast(baronPos);
                }
                if (wcount == 0)
                {
                    Vector3 dragonPos;
                    dragonPos.X = 9919f;
                    dragonPos.Y = 4475f;
                    dragonPos.Z = 0f;
                    if (Player.Distance(dragonPos) < 5000 && root["spell.w"]["WDrake"].GetValue<MenuBool>().Value)
                        W.Cast(dragonPos);
                    else
                        wcount++;
                    return;
                }

                if (wcount == 1)
                {
                    Vector3 redPos;
                    redPos.X = 8022;
                    redPos.Y = 4156;
                    redPos.Z = 0;
                    if (Player.Distance(redPos) < 5000)
                        W.Cast(redPos);
                    else
                        wcount++;
                    return;
                }
                if (wcount == 2)
                {
                    Vector3 bluePos;
                    bluePos.X = 11396;
                    bluePos.Y = 7076;
                    bluePos.Z = 0;
                    if (Player.Distance(bluePos) < 5000)
                        W.Cast(bluePos);
                    else
                        wcount++;
                    return;
                }
                if (wcount > 2)
                {
                    wcount = 0;
                }
            }
        }

        private static void ELogic()
        {
            if (!E.IsReady())
            {
                return;
            }

            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && Helper.hasE(x) && !Helper.Unkillable(x) && x.Distance(Player) < 900 && !x.IsDead))
            {
                if (root["spell.e"]["AutoEChamp"].GetValue<MenuBool>().Value)
                {
                    if (Damage.GetEdamage(enemy) > Helper.GetHealth(enemy))
                    {
                        CastE();
                    }
                }
            }
        }

        private static void RLogic()
        {
            if (Player.IsRecalling() || Player.InFountain() || !R.IsReady() || Helper.GetMana(R)<80)
            {
                return;
            }

            if (soulmate == null)
            {
                foreach (var ally in GameObjects.AllyHeroes.Where(x => !x.IsDead && !x.IsMe && x.HasBuff("kalistacoopstrikeally")))
                {
                    soulmate = ally;
                    break;
                }
            }
            else if (soulmate.IsVisible && soulmate.Distance(Player)<R.Range)
            {
                if (soulmate.Health < Helper.CountEnemy(soulmate.Position, 600) * soulmate.Level *30)
                {
                    R.Cast();
                }
                if(soulmate.ChampionName == "Blitzcrank" && Player.Distance(soulmate.Position) > 300)
                {
                    if(Game.Time - grabT<0.7)
                    {
                        return;
                    }
                    foreach(var enemy in GameObjects.EnemyHeroes.Where(x=> !x.IsDead && !x.IsZombie && x.HasBuff("rocketgrab2")))
                    {
                        R.Cast();
                    }
                }
                if(soulmate.ChampionName == "TahmKench" && Player.Distance(soulmate.Position) > 300)
                {
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(x => !x.IsDead && !x.IsZombie && x.HasBuff("tahmkenchwdevoured")))
                    {
                        R.Cast();
                    }
                }
                if (soulmate.ChampionName == "Skarner" && Player.Distance(soulmate.Position) > 300)
                {
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(x => !x.IsDead && !x.IsZombie && x.HasBuff("skarnerimpale")))
                    {
                        R.Cast();
                    }
                }
            }
        }


        private static void Items()
        {
            //Bilgewater's Cutlass
            if (root["item"]["bilg"].GetValue<MenuBool>().Value)
            {
                if (bilgwat.IsOwned() && bilgwat.IsReady)
                {
                    var target = TargetSelector.GetTarget(bilgwat.Range, DamageType.Physical);
                    if (target != null && !target.IsZombie)
                    {
                        if (target.HealthPercent <= root["item"]["enemyBotkr"].GetValue<MenuSlider>().Value)
                        {
                            bilgwat.Cast(target);
                        }
                        if (Player.HealthPercent <= root["item"]["selfBotkr"].GetValue<MenuSlider>().Value)
                        {
                            bilgwat.Cast(target);
                        }
                    }
                }
            }

            //Botrk
            if (root["item"]["Botkr"].GetValue<MenuBool>().Value)
            {
                if (botrk.IsOwned() && botrk.IsReady)
                {
                    var target = TargetSelector.GetTarget(botrk.Range, DamageType.Physical);
                    if (target != null && !target.IsZombie)
                    {
                        if (target.HealthPercent <= root["item"]["enemyBotkr"].GetValue<MenuSlider>().Value)
                        {
                            botrk.Cast(target);
                        }
                        if (Player.HealthPercent <= root["item"]["selfBotkr"].GetValue<MenuSlider>().Value)
                        {
                            botrk.Cast(target);
                        }
                    }
                }
            }

            //Youumu
            if (root["item"]["youm"].GetValue<MenuBool>().Value)
            {
                if (yom.IsOwned() && yom.IsReady)
                {
                    var target = TargetSelector.GetTarget(botrk.Range, DamageType.Mixed);
                    {
                        if (target != null && !target.IsZombie)
                        {
                            if (target.HealthPercent <= root["item"]["enemyYoumuus"].GetValue<MenuSlider>().Value)
                            {
                                yom.Cast();
                            }
                            if (Player.HealthPercent <= root["item"]["selfYoumuus"].GetValue<MenuSlider>().Value)
                            {
                                yom.Cast();
                            }
                        }
                    }
                }
            }

        }

        private static void LaneClear()
        {
            if(!E.IsReady() || Helper.GetMana(E)<80)
            {
                return;
            }

            var minions = GameObjects.EnemyMinions.Where(x => E.IsInRange(x)).ToList();

            if (minions.Count == 0)
            { 
                return;
            }
            if (root["spell.e"]["AutoEMinions"].GetValue<MenuBool>().Value || root["spell.e"]["BigMinionFinisher"].GetValue<MenuBool>().Value || root["spell.e"]["AutoEMinionsTower"].GetValue<MenuBool>().Value)
            {
                int killable = 0;
                foreach(var m in minions)
                {
                    if (Damage.GetEdamage(m) > m.Health && Health.GetPrediction(m, 500) > Player.GetAutoAttackDamage(m) && m.GetBuff("kalistaexpungemarker").EndTime> 0.5)
                    {
                        killable++;
                        if (killable >= root["spell.e"]["minAutoEMinions"].GetValue<MenuSlider>().Value && root["spell.e"]["AutoEMinions"].GetValue<MenuBool>().Value || (m.CharData.BaseSkinName.ToLower().Contains("siege") || m.CharData.BaseSkinName.ToLower().Contains("super")) && root["spell.e"]["BigMinionFinisher"].GetValue<MenuBool>().Value)
                        {
                            CastE();
                            break;
                        }
                        if ((m.CharData.BaseSkinName.ToLower().Contains("siege") || m.CharData.BaseSkinName.ToLower().Contains("super")) && root["spell.e"]["AutoEMinionsTower"].GetValue<MenuBool>().Value && killable > 0 && m.IsUnderTurret())
                        {
                            CastE();
                            break;
                        }
                    }
                }
            }
        }

        private static void JungleClear()
        {

            foreach (var jungle in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget(E.Range) && Helper.hasE(x) && !x.IsMinion&& (x.GetJungleType() == JungleType.Legendary || x.GetJungleType() == JungleType.Large || x.GetJungleType() == JungleType.Small || x.Name.Contains("Crab"))))
            {   
                if (Damage.GetEdamage(jungle) > jungle.Health)
                {
                    if (jungle.Name.Contains("Red") && !jungle.Name.Contains("RedMini") && root["spell.e"]["RedM"].GetValue<MenuBool>().Value)
                    {
                        CastE();
                    }
                    if (jungle.Name.Contains("Blue") && root["spell.e"]["BlueM"].GetValue<MenuBool>().Value && !jungle.Name.Contains("BlueMini"))
                    {
                        CastE();
                    }
                    if (jungle.Name.Contains("Baron") && root["spell.e"]["BaronM"].GetValue<MenuBool>().Value)
                    {
                        CastE();
                    }
                    if(jungle.Name.Contains("Dragon") && root["spell.e"]["DrakeM"].GetValue<MenuBool>().Value)
                    {
                        CastE();
                    }
                    if (jungle.GetJungleType() == JungleType.Large && root["spell.e"]["OtherM"].GetValue<MenuBool>().Value && (!jungle.Name.Contains("Red") && !jungle.Name.Contains("Blue")))
                    {
                        CastE();
                    }
                    if(jungle.Name.Contains("Crab") && root["spell.e"]["MidM"].GetValue<MenuBool>().Value)
                    {
                        CastE();
                    }
                    if(jungle.GetJungleType() == JungleType.Small && root["spell.e"]["SmallM"].GetValue<MenuBool>().Value && !jungle.Name.Contains("Crab"))
                    {
                        CastE();
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if(root["drawing"]["Qrange"].GetValue<MenuBool>().Value)
            {
                if(Q.IsReady())
                {
                    Drawing.DrawCircle(Player.Position, Q.Range, Color.Violet);
                }
            }

            if (root["drawing"]["Wrange"].GetValue<MenuBool>().Value)
            {
                if (W.IsReady())
                {
                    Drawing.DrawCircle(Player.Position, W.Range, Color.Cyan);
                }
            }

            if (root["drawing"]["Erange"].GetValue<MenuBool>().Value)
            {
                if (E.IsReady())
                {
                    Drawing.DrawCircle(Player.Position, E.Range, Color.Orange);
                }
            }

            if (root["drawing"]["Rrange"].GetValue<MenuBool>().Value)
            {
                if (R.IsReady())
                {
                    Drawing.DrawCircle(Player.Position, R.Range, Color.Gray);
                }
            }

            if (root["drawing"]["Minionh"].GetValue<MenuBool>().Value)
            {
                foreach (var e in GameObjects.Enemy.Where(x => !x.IsDead && x.IsValidTarget(E.Range + 500)))
                {
                    if (e is Obj_AI_Minion && Damage.GetEdamage(e) > e.Health)
                    {
                        Vector3 p = new Vector3((int)e.Position.X, (int)e.Position.Y, (int)e.Position.Z);
                        var points = Helper.CalculateVertices(4, e.ScaleSkinCoef *30, 70, p);

                        PolygonDraw(p, points, 3, Color.LightGreen);
                    }
                }
            }

            if (root["drawing"]["healthp"].GetValue<MenuBool>().Value)
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Base>().Where(x => (x.IsHPBarRendered && x.IsValidTarget(E.Range) && Helper.hasE(x) && !x.IsMinion && !x.Name.Contains("Mini")) && (x.IsEnemy || x.GetJungleType() == JungleType.Legendary || x.Name.Contains("Crab"))))
                {
                    float hp = Helper.GetHealth(enemy) - Damage.GetEdamage(enemy);
                    var dmg = ((int)((Damage.GetEdamage(enemy) / Helper.GetHealth(enemy)) * 100));

                    if (dmg <= 9)
                    {
                        Text.DrawText(null, dmg.ToString(), (int)enemy.HPBarPosition.X + 108, (int)enemy.HPBarPosition.Y + 41, SharpDX.Color.Black);
                        Text.DrawText(null, "%", (int)enemy.HPBarPosition.X + 125, (int)enemy.HPBarPosition.Y + 41, SharpDX.Color.Black);
                        Text.DrawText(null, dmg.ToString() + "%", (int)enemy.HPBarPosition.X + 110, (int)enemy.HPBarPosition.Y + 40, root["drawing"]["colorp"].GetValue<MenuColor>().Color);
                    }
                    if(dmg >= 10)
                    {
                        Text.DrawText(null, dmg.ToString(), (int)enemy.HPBarPosition.X + 108, (int)enemy.HPBarPosition.Y + 41, SharpDX.Color.Black);
                        Text.DrawText(null, "%", (int)enemy.HPBarPosition.X + 138, (int)enemy.HPBarPosition.Y + 41, SharpDX.Color.Black);
                        Text.DrawText(null, dmg.ToString() + "%", (int)enemy.HPBarPosition.X + 110, (int)enemy.HPBarPosition.Y + 40, root["drawing"]["colorp"].GetValue<MenuColor>().Color);
                    }
                }
            }

            if (root["drawing"]["Target"].GetValue<MenuBool>().Value)
            {
                if (!Player.IsDead)
                {
                    var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                    if(Game.Time - time < 1)
                    {
                        i = i + 0.5;    
                    }
                    if(i>360)
                    {
                        i = 0;
                    }
                    Vector3 p = new Vector3((int)t.Position.X, (int)t.Position.Y, (int)t.Position.Z);
                    var points = Helper.CalculateVertices(3, t.ScaleSkinCoef * 65, (int)i, p);
                    var points1 = Helper.CalculateVertices(11, t.ScaleSkinCoef * 73, (int)-i, p);
                    PolygonDraw(p, points1, (float)3, Color.Gold);
                    PolygonDraw(p, points, (float)2.5, Color.LightYellow);
                    time = Game.Time;

                    if (root["drawing"]["TargetA"].GetValue<MenuBool>().Value)
                    {
                        if (Player.Distance(t) > Helper.GetAttackRange(t))
                        {
                            Drawing.DrawCircle(t.Position, Helper.GetAttackRange(t), Color.ForestGreen);
                        }
                        else
                        {
                            Drawing.DrawCircle(t.Position, Helper.GetAttackRange(t), Color.OrangeRed);
                        }
                    }
                }
            }
        }

        private static void CastE()
        {
            if (Game.Time - lastecast < 0.700)
            {
                return;
            }

            E.Cast();
        }

        private static void PolygonDraw(Vector3 Center,Vector3[] Points,float thickness,Color color)
        {
            for (int i = 0; i < Points.Count() - 1; i++)
            {
                var q = Drawing.WorldToScreen(new Vector3(new Vector2(Points[i].X, Points[i].Y),
                    Points[i].Z));

                var z = Drawing.WorldToScreen(new Vector3(new Vector2(Points[i + 1].X, Points[i + 1].Y),
                    Points[i + 1].Z));

                Drawing.DrawLine(q, z, thickness, color);
            }

            Drawing.DrawLine(Drawing.WorldToScreen(new Vector3(new Vector2(Points[Points.Length - 1].X, Points[Points.Length - 1].Y),
                                Player.Position.Z)), Drawing.WorldToScreen(new Vector3(new Vector2(Points[0].X, Points[0].Y),
                                 Player.Position.Z)), thickness, color);
        }
    }
}
