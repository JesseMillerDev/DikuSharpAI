﻿namespace DikuSharp.Server.Characters
{
    public class Prompt
    {
        public const string PROMPT_DEFAULT = "[%h/%Hhp %m/%Mmp]";
        public const string HP_TOKEN = "%h";
        public const string MAXHP_TOKEN = "%H";
        public const string MP_TOKEN = "%m";
        public const string MAXMP_TOKEN = "%M";
        public const string MV_TOKEN = "%v";
        public const string MAXMV_TOKEN = "%V";

        private string _current;
        public string Current { get => _current; set => _current = value; }

        public string Parsed { get; set; }

        public override string ToString()
        {
            return Current;
        }

        public static string ParsePrompt(PlayerCharacter ch)
        {
            string formatString = ch.Prompt ?? PROMPT_DEFAULT;
            return formatString.Replace(HP_TOKEN, ch.Hp.ToString())
                .Replace(MAXHP_TOKEN, ch.MaxHitPoints.ToString())
                .Replace(MP_TOKEN, ch.Mp.ToString())
                .Replace(MAXMP_TOKEN, ch.MaxManaPoints.ToString())
                .Replace(MV_TOKEN, ch.Mv.ToString())
                .Replace(MAXMV_TOKEN, ch.MaxMovePoints.ToString())
                ;
            //add more tokens above
        }        

        public override bool Equals(object obj)
        {
            if ( obj == null ) { return false; }
            if ( obj is Prompt prompt)
            {
                return prompt.Current == this.Current;
            }
            return obj is string && Current == (string)obj;
        }

        public bool Equals(Prompt obj)
        {
            if ( obj == null ) { return false; }
            return obj.Current == Current;
        }

        public override int GetHashCode()
        {
            return Current.GetHashCode();
        }

        public static implicit operator string(Prompt p)
        {
            return p.Current;
        }

        public static implicit operator Prompt(string s)
        {
            return new Prompt { Current = s };
        }
    }
}
