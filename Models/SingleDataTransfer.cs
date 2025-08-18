using System.Linq;

namespace PL_Assembler.Models
{
    public class SingleDataTransfer : Instruction
    {
        public string i { get; set; } = "0"; // Immediate offset flag
        public string p { get; set; } // Pre/Post indexing flag
        public string u { get; set; } // Up/Down bit
        public string b { get; set; } = "0"; // Byte/Word bit
        public string w { get; set; } // Write-back bit
        public string l { get; set; } // Load/Store bit
        public string ModeName { get; set; } // Mode name

        public SingleDataTransfer(string conditionCode, string i, string p, string u, string b, string w, string l, string rn, string rd, string offset)
            : base(conditionCode, null, rn, rd, null, offset)
        {
            this.i = i;
            this.p = p;
            this.u = u;
            this.b = b;
            this.w = w;
            this.l = l;
        }

        public SingleDataTransfer(string instructionSyntax)
            : base(instructionSyntax)
        {
            //<LDR | STR>{cond}{B}{T} Rd, <Address>
            int marker = 0;
            this.l = ParsedSyntax[marker++].ToUpper().Contains("L") ? "L" : "";
            this.ModeName = (ParsedSyntax[marker].Contains("E") || ParsedSyntax[marker].Contains("F")) ? ParsedSyntax[marker].Substring(ParsedSyntax[marker++].Length - 2) : "";
            modeNameSetFlags();
            this.w = instructionSyntax.Contains("!") ? "W" : ""; // Write-back flag
            ConditionCode = ConditionCodeMap.ContainsKey(ParsedSyntax[marker]) ? ParsedSyntax[marker++] : "AL";
            this.b = ParsedSyntax[marker].ToUpper().Contains("B") ? "B" : "";
            marker += b == "B" ? 1 : 0;
            Rd = ParsedSyntax[marker++].ToUpper();
            Rn = ParsedSyntax[marker++].ToUpper();
            Offset = ParsedSyntax.Length > marker ? ParsedSyntax[marker].ToLower() : "";
        }

        public void modeNameSetFlags()
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

        public override string ToBinary()
        {
            string binaryInstruction = ConditionCodeToBinary() + "01" + i + (p == "P" ? "1" : "0") + (u == "U" ? "1" : "0") + b + (w == "W" ? "1" : "0") + (l == "L" ? "1" : "0") + ConvertRegisterToBinary(Rn) + ConvertRegisterToBinary(Rd) + ConvertOffsetToBinary(Offset, 12);
            //condition + 01 + i + p + u + b + w + l + rn + rd + offset
            return binaryInstruction;
        }
    }
}
