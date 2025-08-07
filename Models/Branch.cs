namespace PL_Assembler.Models
{
    public class Branch : Instruction
    {

        public string l { get; set; } // Link bit

        public Branch(string condition,string l, string offset) : base(condition, null, null, null, null, offset)
        {
            this.l = l;
        }

        public Branch(string instructionSyntax)
            : base(instructionSyntax)
        {
            int marker = 0;
            //B{L}{cond}<expression>
            marker++; // Skip the "B" part
            l = ParsedSyntax[marker].ToUpper() == "L" ? ParsedSyntax[marker++] : "";
            ConditionCode = ConditionCodeMap.ContainsKey(ParsedSyntax[marker]) ? ParsedSyntax[marker++] : "AL";
            Offset = ParsedSyntax[marker++].ToLower();
        }

        public override string ToBinary()
        {
            string binaryInstruction = ConditionCodeToBinary() + 101 + (l == "L" ? "1" : "0") + ConvertOffsetToBinary(Offset, 24);

            return binaryInstruction;
        }
    }
}
