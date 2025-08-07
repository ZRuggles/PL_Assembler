namespace PL_Assembler.Models
{
    public class DataProcessing : Instruction
    {
        public string i { get; set; } // Immediate flag
        public string s { get; set; } // Set condition code flag
        public DataProcessing(string condition, string i, string opcode, string s, string rn, string rd, string operand2) : base(condition, opcode, rn, rd, operand2, null)
        {
            this.i = i;
            this.s = s;
        }

        public DataProcessing(string instructionSyntax)
           : base(instructionSyntax)
        {
            int marker = 0;
            //<opcode>{cond}{S} rd, rn, <Op2>
            Opcode = ParsedSyntax[marker++].ToUpper();
            ConditionCode = ConditionCodeMap.ContainsKey(ParsedSyntax[marker]) ? ParsedSyntax[marker++] : "AL";
            s = ParsedSyntax[marker].ToUpper() == "S" ? ParsedSyntax[marker++] : "" ;
            Rd = ParsedSyntax[marker++].ToUpper();
            Rn = ParsedSyntax[marker++].ToUpper();
            Operand2 = ParsedSyntax[marker++].ToLower();
        }

        public static readonly Dictionary<string, string> OpCodeMap = new Dictionary<string, string>
        {
            {"SUB", "0010"},
            {"ADD", "0100"},
            {"ORR", "1100"},
        };

        public string ConvertOpCodeToBinary()
        {
            return OpCodeMap[Opcode];
        }

        public bool IsOp2Immediate()
        {
            if(Operand2.Contains("R"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override string ToBinary()
        {
            i = IsOp2Immediate() ? "1" : "0";
            string S = s == "S" ? "1" : "0";

            string binaryInstruction = ConditionCodeToBinary() + "00" + i + ConvertOpCodeToBinary() + S + ConvertRegisterToBinary(Rn) + ConvertRegisterToBinary(Rd) + ConvertOpperand2ToBinary(Operand2, 12);



            return binaryInstruction;
        }
    }
}
