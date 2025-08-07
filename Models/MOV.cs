namespace PL_Assembler.Models
{
    public class MOV : Instruction
    {
        string topOrWide { get; set; } // T for top, W for Wide
        string imm4 { get; set; } // 4-bit immediate value
        string imm12 { get; set; } // 12-bit immediate value
        public MOV(string condition, string rd, string imm16) : base(condition, null, null, rd, null, null){}
        public MOV(string instructionSyntax) : base(instructionSyntax)
        {
            //A1 and A2 Encoding
            //MOV(T)(W)<cond> <Rd>,<imm16> 
            int marker = 0;
            topOrWide = ParsedSyntax[marker++].ToUpper();
            ConditionCode = ConditionCodeMap.ContainsKey(ParsedSyntax[marker]) ? ParsedSyntax[marker++] : "AL";
            Rd = ParsedSyntax[marker++].ToUpper();
            string imm16 = ParsedSyntax[marker++].ToLower();
            imm16 = ConvertImmediateToBinary(imm16, 16);
            imm4 = imm16.Substring(0, 4); // First 4 bits
            imm12 = imm16.Substring(4, 12); // Next 12 bits

        }

        public override string ToBinary()
        {
            //cond 0011 0000 imm4 Rd imm12 | MOVW
            //cond 0011 0100 imm4 Rd imm12 | MOVT
            string binaryInstruction = ConditionCodeToBinary() + "0011" + (topOrWide.Contains("W") ? "0000" : "0100") + imm4 + ConvertRegisterToBinary(Rd) + imm12;


            return binaryInstruction;
        }
    }
}
