namespace PL_Assembler.Models
{
    public class SingleDataTransfer : Instruction
    {
        public string i { get; set; } // Immediate offset flag
        public string p { get; set; } // Pre/Post indexing flag
        public string u { get; set; } // Up/Down bit
        public string b { get; set; } // Byte/Word bit
        public string w { get; set; } // Write-back bit
        public string l { get; set; } // Load/Store bit

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
            ConditionCode = ConditionCodeMap.ContainsKey(ParsedSyntax[marker]) ? ParsedSyntax[marker++] : "AL";
            this.b = ParsedSyntax[marker].ToUpper().Contains("B") ? "B" : "";
            marker += b == "B" ? 1 : 0;
            Rd = ParsedSyntax[marker++].ToUpper();
            Rn = ParsedSyntax[marker++].ToUpper();
            Offset = ParsedSyntax.Length > marker ? ParsedSyntax[marker].ToLower() : "";
        }

        public override string ToBinary()
        {
            


            ConditionCode = ParsedSyntax.Length > 1 ? ParsedSyntax[1] : "AL";

            ParseFlagsAndOperands();

            string binaryInstruction = ConditionCodeToBinary() + "01" + i + p + u + b + w + (l.Contains("L") ? "1" : "0") + ConvertRegisterToBinary(Rn) + ConvertRegisterToBinary(Rd) + ConvertOffsetToBinary(Offset, 12);
            //condition + 01 + i + p + u + b + w + l + rn + rd + offset
            return binaryInstruction;
        }



        private void ParseFlagsAndOperands()
        {
            // Set default values for the flags
            p = "0"; 
            u = "0"; 
            b = "0"; 
            w = "0"; 
            i = "0";
        }
    }
}
