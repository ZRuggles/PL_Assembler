using Microsoft.AspNetCore.Components.Forms;

namespace PL_Assembler.Models
{
    public abstract class Instruction
    {
        public string instructionSyntax { get; set; } // this should be the instruction syntax as a string from the text file
        public string? offsetSyntax { get; set; }
        public string[] ParsedSyntax { get; set; } // this should be the instruction syntax parsed into its components
        public string Opcode { get; set; }
        public string Rn { get; set; }
        public string Rd { get; set; }
        public string Operand2 { get; set; }
        public string Offset { get; set; }
        public string ConditionCode { get; set; } // this should be the condition code for the instruction, if applicable
        public string Label { get; set; } // this is the offset label replaces hard coded specific offset values

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
            if (ParsedSyntax.Any(part => System.Text.RegularExpressions.Regex.IsMatch(part, @"<[a-zA-Z]+|>[a-zA-Z]+")))
            {
                Label = ParsedSyntax.First(part => System.Text.RegularExpressions.Regex.IsMatch(part, @"<[a-zA-Z]+|>[a-zA-Z]+"));
            }
        }

        public void ResolveLabelOffset(Dictionary<string, int> labelMap, int currentIndex)
        {
            if (string.IsNullOrEmpty(Label) || !Label.Contains(">"))
                return;

            string cleanedLabel = Label.Replace("<", "").Replace(">", "");
            if (!labelMap.TryGetValue(cleanedLabel, out int destinationIndex))
                throw new ArgumentException($"Destination label '<{cleanedLabel}' not found.");

            int distance = (destinationIndex - currentIndex) - 2; // +2 if PC-relative offset starts 8 bytes ahead

            Offset = "0x" + (distance & 0xFFFFFF).ToString("X");
        }





        public static Instruction CreateInstruction(string instructionSyntax)
        {
            // This method should create an instance of the appropriate Instruction subclass based on the syntax
            var parts = instructionSyntax.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            string opcode = parts[0].ToUpper();

            switch (opcode)
            {
                case "LDR":
                case "STR":
                    return new SingleDataTransfer(instructionSyntax);
                case "B":
                case "BL":
                case "BX":
                    return new Branch(instructionSyntax);
                case "ADD":
                case "SUB":
                case "AND":
                case "ORR":
                    return new DataProcessing(instructionSyntax);
                case "MOVT":
                case "MOVW":
                    return new MOV(instructionSyntax);
                default:
                    throw new ArgumentException($"Unknown instruction type: {opcode}");
            }
        }

        protected void ParseInstructionSyntax(string syntax)
        {
            ParsedSyntax = new string[] { };
            var parts = syntax.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            ParsedSyntax = parts;

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
            if (string.IsNullOrEmpty(offset))
            {
                return new string('0', bits); // If offset is empty, return 0s
            }

            if (offset.StartsWith("R"))
            {
                return ConvertRegisterToBinary(offset);
            }


            if (offset.StartsWith("#") || offset.StartsWith("0x"))
            {
                int immediate = 0;

  
                if (offset.StartsWith("#"))
                {
                    immediate = int.Parse(offset.Substring(1)); 
                }
                else if (offset.StartsWith("0x"))
                {
                    immediate = Convert.ToInt32(offset, 16); 
                }

                return Convert.ToString(immediate, 2).PadLeft(bits, '0'); 
            }

            return "000000000000";
        }


        public string ConvertOpperand2ToBinary(string op2, int bits)
        {
            int immediate = 0;
            // If operand is a register, convert it
            if (op2.StartsWith("R"))
            {
                return ConvertRegisterToBinary(op2);
            }
            // If operand is an immediate, convert it
            if (op2.StartsWith("0x"))
            {
                immediate = Convert.ToInt32(op2, 16);
                return Convert.ToString(immediate, 2).PadLeft(bits, '0'); // Example: #10 -> 000000001010
            }

            return immediate.ToString().PadLeft(bits, '0');
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

        public string ToHex()
        {
            string binaryInstruction = ToBinary();
            return Convert.ToString(Convert.ToInt64(binaryInstruction, 2), 16).ToUpper().PadLeft(8, '0');
        }

        public string ToHexFlip()
        {
            string hexInstruction = ToHex();
            string[] hexInstructionParts = Enumerable
                .Range(0, hexInstruction.Length / 2)
                .Select(i => hexInstruction.Substring(i * 2, 2))
                .ToArray();

            Array.Reverse(hexInstructionParts);

            return string.Join("", hexInstructionParts);
        }

        public byte[] ToByteArray()
        {
            string flippedHex = ToHexFlip();

            return Enumerable.Range(0, flippedHex.Length / 2)
             .Select(i => Convert.ToByte(flippedHex.Substring(i * 2, 2), 16))
             .ToArray();
        }

        public abstract string ToBinary();
    }
}
