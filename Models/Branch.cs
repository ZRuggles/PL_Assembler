namespace PL_Assembler.Models
{
    public class Branch : Instruction
    {

        public string l { get; set; } // Link bit
        public string x { get; set; } // Check if BX instruction

        public Branch(string condition,string l, string offset) : base(condition, null, null, null, null, offset)
        {
            this.l = l;
        }
        public Branch(string condition, string Rn) : base(condition, null, Rn, null, null, null)
        {
            
        }


        public Branch(string instructionSyntax)
            : base(instructionSyntax)
        {
            int marker = 0;
            //B{L}{cond}<expression>
            //BX{cond}<Rn>
            //marker++; // Skip the "B" part Assembly Lab 1
            x = ParsedSyntax[marker].ToUpper().Contains("X") ? "X" : "";
            l = ParsedSyntax[marker++].ToUpper().Contains("L") ? "L" : "";
            ConditionCode = ConditionCodeMap.ContainsKey(ParsedSyntax[marker]) ? ParsedSyntax[marker++] : "AL";
            Offset = ParsedSyntax[marker].ToLower().Contains("0x") ? ParsedSyntax[marker] : "";
            Rn = ParsedSyntax[marker].ToUpper().Contains("R") ? ParsedSyntax[marker] : "";
        }

        public override string ToBinary()
        {
            string binaryInstruction;

            if (x == "X")
            {
                // BX instruction
                //cond 0001 0010 1111 1111 1111 0001 Rn
                binaryInstruction = ConditionCodeToBinary() + "0001" + "0010" + "1111" + "1111" + "1111" + "0001" + ConvertRegisterToBinary(Rn);
            }
            else
            {
                // B or BL instruction
                binaryInstruction = ConditionCodeToBinary() + "101" + (l == "L" ? "1" : "0") + ConvertOffsetToBinary(Offset, 24);
            }

            return binaryInstruction;
        }
    }
}
