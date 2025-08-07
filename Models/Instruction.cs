namespace PL_Assembler.Models
{
    public abstract class Instruction
    {
        public string instructionSyntax { get; set; } // this should be the instruction syntax as a string from the text file
        public string[] ParsedSyntax { get; set; } // this should be the instruction syntax parsed into its components
        public string Opcode { get; set; }
        public string Rn { get; set; }
        public string Rd { get; set; }
        public string Operand2 { get; set; }
        public string Offset { get; set; }
        public string ConditionCode { get; set; } // this should be the condition code for the instruction, if applicable

        public static readonly Dictionary<string, string> ConditionCodeMap = new Dictionary<string, string>
        {
            { "AL", "1110" },  // Always
            { "PL", "0101" },  // Positive or Zero
        };


        public Instruction(string condition, string opcode, string rn, string rd, string operand2, string offset)
        {
            this.ConditionCode = condition;
            this.Opcode = opcode;
            this.Rn = rn;
            this.Rd = rd;
            this.Operand2 = operand2;
            this.Offset = offset;
        }

        public Instruction(string instructionSyntax)
        {
            this.instructionSyntax = instructionSyntax;
            ParseInstructionSyntax(instructionSyntax);
        }

        protected void ParseInstructionSyntax(string syntax)
        {
            var parts = syntax.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            ParsedSyntax = parts;
            //// Convert to binary and hex formats
            //this.instructionBinary = ToBinary();
            //this.instructionHex = Convert.ToString(Convert.ToInt32(this.instructionBinary, 2), 16).PadLeft(8, '0');
            //this.instructionHexFlip = string.Join("", this.instructionHex.Reverse());
        }

        public string ConditionCodeToBinary()
        {
            if (string.IsNullOrEmpty(ConditionCode) || !ConditionCodeMap.ContainsKey(ConditionCode))
            {
                return ConditionCodeMap["AL"]; // Default to "Always"
            }
            return ConditionCodeMap[ConditionCode];
        }

        public string ConvertRegisterToBinary(string register)
        {
            return Convert.ToString(int.Parse(register.Replace("R", "").Replace("(", "").Replace(")","")), 2).PadLeft(4, '0'); 
        }
        public string ConvertOffsetToBinary(string offset, int bits)
        {
            // If operand is a register, convert it
            if (offset.StartsWith("R"))
            {
                return ConvertRegisterToBinary(offset);
            }
            // If operand is an immediate, convert it
            if (offset.StartsWith("0x"))
            {
                int immediate = Convert.ToInt32(offset, 16);
                return Convert.ToString(immediate, 2).PadLeft(bits, '0'); // Example: #10 -> 000000001010
            }

            return ToString().PadLeft(bits, '0'); 
        }

        public string ConvertOpperand2ToBinary(string op2, int bits)
        {
            // If operand is a register, convert it
            if (op2.StartsWith("R"))
            {
                return ConvertRegisterToBinary(op2);
            }
            // If operand is an immediate, convert it
            if (op2.StartsWith("0x"))
            {
                int immediate = Convert.ToInt32(op2, 16);
                return Convert.ToString(immediate, 2).PadLeft(bits, '0'); // Example: #10 -> 000000001010
            }

            return ToString().PadLeft(bits, '0');
        }

        public string ConvertImmediateToBinary(string immediate, int bits)
        {
            if (immediate.StartsWith("0x"))
            {
                int immValue = Convert.ToInt32(immediate, 16);
                return Convert.ToString(immValue, 2).PadLeft(bits, '0');
            }
            else if (immediate.StartsWith("#"))
            {
                int immValue = Convert.ToInt32(immediate.Substring(1));
                return Convert.ToString(immValue, 2).PadLeft(bits, '0');
            }
            return immediate.PadLeft(bits, '0'); // Fallback to just padding the string
        }

        public abstract string ToBinary();
    }
}
