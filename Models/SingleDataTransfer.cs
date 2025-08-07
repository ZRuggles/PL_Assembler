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

        }

        public override string ToBinary()
        {
            if (ParsedSyntax[0] == "LDR")
            {
                this.l = "1"; // Load
            }
            else if (ParsedSyntax[0] == "STR")
            {
                this.l = "0"; // Store
            }
            else
            {
                throw new ArgumentException("Invalid instruction syntax for Single Data Transfer.");
            }

            ConditionCode = ParsedSyntax.Length > 1 ? ParsedSyntax[1] : "AL";

            ParseFlagsAndOperands();

            string binaryInstruction = ConditionCodeToBinary() + "01" + i + p + u + b + w + l + ConvertRegisterToBinary(Rn) + ConvertRegisterToBinary(Rd) + ConvertOffsetToBinary(Offset, 12);
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

            if (ParsedSyntax.Length > 3)
            {
                p = ParsedSyntax[3] == "!" ? "1" : "0"; 
                u = ParsedSyntax.Length > 4 && ParsedSyntax[4] == "+" ? "1" : "0"; 
                b = ParsedSyntax.Length > 5 && ParsedSyntax[5] == "B" ? "1" : "0"; 
                w = ParsedSyntax.Length > 6 && ParsedSyntax[6] == "!" ? "1" : "0"; 
            }

            Rd = Rd = ParsedSyntax[1];
            Rn = ParsedSyntax[2];

            Offset = ParsedSyntax.Length > 3 ? ParsedSyntax[3] : "";
        }
    }
}
