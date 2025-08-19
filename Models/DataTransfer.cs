using System.Linq;

namespace PL_Assembler.Models
{
    public class DataTransfer : Instruction
    {
        public string i { get; set; } = "0"; // Immediate offset flag
        public string p { get; set; } // Pre/Post indexing flag
        public string u { get; set; } // Up/Down bit
        public string b { get; set; } = "0"; // Byte/Word bit
        public string w { get; set; } // Write-back bit
        public string l { get; set; } // Load/Store bit
        public string s { get; set; } = "0";// PSR & force user bit
        public List<string> registerList { get; set; } = new List<string>(); // Register list for STM & LDM instructions

        public string ModeName { get; set; } // Mode name

        public DataTransfer(string conditionCode, string i, string p, string u, string b, string w, string l, string rn, string rd, string offset)
            : base(conditionCode, null, rn, rd, null, offset)
        {
            this.i = i;
            this.p = p;
            this.u = u;
            this.b = b;
            this.w = w;
            this.l = l;
        }

        public DataTransfer(string instructionSyntax)
            : base(instructionSyntax)
        {
            //<LDM | STM><FD | ED | FA | EA | IA | IB | DA | DB> Rn{!}, <register list>
            //<LDR | STR>{cond}{B}{T} Rd, <Address>
            int marker = 0;
            this.ModeName = (ParsedSyntax[marker].Contains("E") || ParsedSyntax[marker].Contains("F")) ? ParsedSyntax[marker].Substring(ParsedSyntax[marker].Length - 2) : "";
            this.l = ParsedSyntax[marker++].ToUpper().Contains("L") ? "L" : "";
            modeNameSetFlags();
            this.w = instructionSyntax.Contains("!") ? "W" : ""; // Write-back flag
            ConditionCode = ConditionCodeMap.ContainsKey(ParsedSyntax[marker]) ? ParsedSyntax[marker++] : "AL";
            Rd = ParsedSyntax[0].Contains("M") ? "" : ParsedSyntax[marker++].ToUpper();
            Rn = ParsedSyntax[marker++].ToUpper().Replace("!", "");
            if(ParsedSyntax[0].Contains("M"))
            {
                makeRegisterList(ParsedSyntax[marker]);
            }
            else
            {
                Offset = ParsedSyntax.Length > marker ? ParsedSyntax[marker].ToLower() : "";
            }
        }

        public void modeNameSetFlags()
        {
            if (this.l == "L")
            {
                switch (ModeName)
                {
                    case "ED":
                        p = "P";
                        u = "U";
                        break;
                    case "FD":
                        u = "U";
                        break;
                    case "EA":
                        p = "P";
                        break;
                }
            }
            else
            {
                switch (ModeName)
                {
                    case "FA":
                        p = "P";
                        u = "U";
                        break;
                    case "EA":
                        u = "U";
                        break;
                    case "FD":
                        p = "P";
                        break;
                }
            }
        }

        public void makeRegisterList(string registerListSyntax)
        {
            registerListSyntax = registerListSyntax.Trim('{', '}');
            var parts = registerListSyntax.Split(',');
            foreach (var part in parts)
            {
                if (part.Contains("-"))
                {
                    var rangeParts = part.Split('-');

                    int start = int.Parse(rangeParts[0].Substring(1));
                    int end = int.Parse(rangeParts[1].Substring(1));

                    for (int i = start; i <= end; i++)
                    {
                        registerList.Add("R" + i);
                    }
                }
                else
                {
                    registerList.Add(part.Trim().ToUpper());
                }
            }
        }
        public string registerListToBinary()
        {
            var binaryList = "";
            for (int i = 15; i >= 0; i--)
            {
                binaryList += registerList.Contains("R" + i) ? "1" : "0";
            }
            return binaryList;
        }

        public override string ToBinary()
        {
            string binaryInstruction = "";
            if (ParsedSyntax[0].Contains("M"))
            {
                //STM and LDM - condition + 100 + p + u + s + w + l + Rn + register list
                binaryInstruction = ConditionCodeToBinary() + "100" + (p == "P" ? "1" : "0") + (u == "U" ? "1" : "0") + (s == "S" ? "1" : "0") + (w == "W" ? "1" : "0") + (l == "L" ? "1" : "0") + ConvertRegisterToBinary(Rn) + registerListToBinary();
            }
            else
            {
                //STR and LDR - condition + 01 + i + p + u + b + w + l + rn + rd + offset
                binaryInstruction = ConditionCodeToBinary() + "01" + (i == "I" ? "1" : "0") + (p == "P" ? "1" : "0") + (u == "U" ? "1" : "0") + (b == "B" ? "1" : "0") + (w == "W" ? "1" : "0") + (l == "L" ? "1" : "0") + ConvertRegisterToBinary(Rn) + ConvertRegisterToBinary(Rd) + ConvertOffsetToBinary(Offset, 12);
            }
            return binaryInstruction;
        }
    }
}
