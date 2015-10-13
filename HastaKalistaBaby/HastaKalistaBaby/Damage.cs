using LeagueSharp;
using LeagueSharp.SDK.Core.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HastaKalistaBaby
{
    class Damage
    {
        public static float GetQDmg(Obj_AI_Base target)
        {
            var dmg = new double[] { 10, 70, 130, 190, 250 }[Program.Q.Level]
                + Program.Player.BaseAttackDamage + Program.Player.FlatPhysicalDamageMod;
            return (float)Program.Player.CalculateDamage(target, DamageType.Physical, dmg);
        }

        public static float GetWDmg(Obj_AI_Base target)
        {
            var dmg = (new double[] { 12, 14, 16, 18, 20 }[Program.W.Level] / 100)
            * target.MaxHealth;
            return (float)Program.Player.CalculateDamage(target, DamageType.Magical, dmg);

        }

        public static float GetEdamage(Obj_AI_Base target)
        {
            var buff = target.GetBuff("KalistaExpungeMarker");
            if (buff != null)
            {
                var dmg =
                    (float)
                        ((new double[] { 20, 30, 40, 50, 60 }[Program.E.Level - 1] +
                          0.6 * (Program.Player.BaseAttackDamage + Program.Player.FlatPhysicalDamageMod)) +
                         ((buff.Count - 1) *
                          (new double[] { 10, 14, 19, 25, 32 }[Program.E.Level - 1] +
                           new double[] { 0.2, 0.225, 0.25, 0.275, 0.3 }[Program.E.Level - 1] *
                           (Program.Player.BaseAttackDamage + Program.Player.FlatPhysicalDamageMod))));


                if (Program.Player.HasBuff("summonerexhaust"))
                {
                    dmg *= 0.6f;
                }
                if (Program.Player.HasBuff("urgotcorrosivedebuff"))
                {
                    dmg *= 0.85f;
                }
                if (target.HasBuff("FerociousHowl"))
                {
                    dmg *= 0.3f;
                }
                if(target.HasBuff("gragaswself"))
                {
                    dmg *= 0.92f - ((target.Spellbook.GetSpell(SpellSlot.W).Level)*0.02f);
                }
                if (target.HasBuff("Medidate"))
                {
                    dmg *= 0.55f - ((target.Spellbook.GetSpell(SpellSlot.E).Level)*0.05f) ;
                }
                if (target.HasBuff("GarenW"))
                {
                    dmg *= 0.7f;
                }
                if (target.HasBuff("vladimirhemoplaguedebuff"))
                {
                    dmg *= 1.15f;
                }
                if (target.Name.Contains("Baron") && Program.Player.HasBuff("barontarget"))
                {
                    dmg *= 0.5f;
                }
                if (target.Name.Contains("Dragon") && Program.Player.HasBuff("s5test_dragonslayerbuff"))
                {
                    dmg *= (1f - (0.07f * Program.Player.GetBuffCount("s5test_dragonslayerbuff")));
                }


                return
                (float)
                    Program.Player.CalculateDamage(target, DamageType.Physical, dmg + target.FlatHPRegenMod);
            }
            return 0;
        }
    }
}
