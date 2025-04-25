using System;
using SDL2;

namespace Chip8_CSharp.Chip8
{
    public class ProcessingUnit
    {
        byte keyboard = 0x10;
        
        byte[] gfx = new byte[64 * 32];
        bool newDraw = false;
        
        public byte[] mem = new byte[4096]; // 4k bytes memory
        byte[] V = new byte[16]; // Registers (addr F reserved for carry flag)
        ushort[] stack = new ushort[48];
        ushort SP = 0;
        ushort I = 0;
        ushort PC = 0x200; // As described should start at 0x200 as the prior addr are reserved
        byte soundTimer = 0;
        byte delayTimer = 0;
        byte Vx = 0;
        byte Vy = 0;
        byte x = 0;
        byte[] fontSet = {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };

        Random rand = new Random();

        public byte Keyboard{
            get => keyboard;
            set => keyboard = value;
        }

        public bool NewDraw{
            get => newDraw;
            set => newDraw = value;
        }

        public byte[] getGfx() => gfx;

        public void runCpu()
        {
            Console.WriteLine("Foi");
            ushort opcode = (ushort)(mem[PC] << 8 | mem[PC + 1]); // Read opcode (stored in 2 bytes)
            OpcodeCases(opcode);
            PC += 2;

        }

        void OpcodeCases(ushort opcode)
        {
            switch (opcode & 0xF000) //Ignores the 3 least significant bytes
            {
                case 0x0000:
                    if (opcode == 0x00E0)
                    {
                        ClearScreen();
                    }
                    else if (opcode == 0x00EE)
                    {
                        // Returns subroutine
                        SP -= 1;
                        PC = SP;
                    }
                    break;

                case 0x1000:
                    // 1NNN
                    // Jumps to NNN
                    PC = (ushort)(opcode & 0x0FFF);
                    IgnorePCAdvance();
                    break;

                case 0x2000:
                    // 2NNN
                    // Calls subroutine at NNN
                    stack[SP] = PC;
                    SP++;
                    PC = (ushort)(opcode & 0x0FFF);
                    IgnorePCAdvance();
                    break;

                case 0x3000:
                    // 3XNN
                    // Skips next instruction if V[x] == NN
                    Vx = V[(byte)(opcode & 0x0F00 >> 8)];
                    if ((byte)(opcode & 0x00FF) == Vx)
                    {
                        PC += 2;
                    }
                    break;

                case 0x4000:
                    // 4XNN
                    // Skips next instruction if V[x] != NN
                    Vx = V[(byte)(opcode & 0x0F00 >> 8)];
                    if ((byte)(opcode & 0x00FF) != Vx)
                    {
                        PC += 2;
                    }
                    break;

                case 0x5000:
                    // 5XY0
                    // Skips next instruction if V[x] == V[y]
                    Vx = V[(byte)(opcode & 0x0F00) >> 8];
                    Vy = V[(byte)(opcode & 0x00F0) >> 4];
                    if (Vx == Vy)
                    {

                    }
                    break;

                case 0x6000:
                    // 6XNN
                    // Vx = NN
                    V[(byte)(opcode & 0x0F00) >> 8] = (byte)(opcode & 0x00FF);
                    break;

                case 0x7000:
                    // 7XNN
                    // Vx += NN
                    V[(byte)(opcode & 0x0F00) >> 8] += (byte)(opcode & 0x00FF);
                    break;

                case 0x8000:
                    // Generalization
                    Vy = V[(byte)(opcode & 0x00F0) >> 4];

                    switch (opcode & 0xF00F)
                    {
                        case 0x8000:
                            // 8XY0
                            // Vx = Vy
                            V[(byte)(opcode & 0x0F00) >> 8] = Vy;
                            break;

                        case 0x8001:
                            // 8XY1
                            // Vx |= Vy
                            V[(byte)(opcode & 0x0F00) >> 8] |= Vy;
                            break;

                        case 0x8002:
                            // 8XY2
                            // Vx &= Vy
                            V[(byte)(opcode & 0x0F00) >> 8] &= Vy;
                            break;

                        case 0x8003:
                            // 8XY3
                            // Vx ^= Vy
                            V[(byte)(opcode & 0x0F00) >> 8] ^= Vy;
                            break;

                        case 0x8004:
                            // 8XY4
                            // Vx += Vy
                            // Check for overflow
                            if (CheckOverflow(V[(byte)(opcode & 0x0F00) >> 8], Vy))
                            {
                                V[15] = 1;
                            }
                            else
                            {
                                V[15] = 0;
                            }
                            V[(byte)(opcode & 0x0F00) >> 8] += Vy;
                            break;

                        case 0x8005:
                            // 8XY5
                            // Vx -= Vy
                            // Check for underflow
                            if (CheckUnderflow(V[(byte)(opcode & 0x0F00) >> 8], Vy))
                            {
                                V[15] = 1;
                            }
                            else
                            {
                                V[15] = 0;
                            }
                            V[(byte)(opcode & 0x0F00) >> 8] -= Vy;
                            break;

                        case 0x8006:
                            // 8XY6
                            // Vx >>= 1
                            // Stores the least significant bit of VX prior to the shift into VF
                            V[15] = (byte)(V[(byte)(opcode & 0x0F00) >> 8] & 0x0001);
                            V[(byte)(opcode & 0x0F00) >> 8] >>= 1;
                            break;

                        case 0x8007:
                            // 8XY7
                            // Vx = Vy - Vx
                            // Check for underflow
                            if (CheckUnderflow(Vy, V[(byte)(opcode & 0x0F00) >> 8]))
                            {
                                V[15] = 1;
                            }
                            else
                            {
                                V[15] = 0;
                            }
                            V[(byte)(opcode & 0x0F00)] = (byte)(Vy - V[(byte)(opcode & 0x0F00)]);
                            break;

                        case 0x800E:
                            // 8XYE
                            // Vx <<= 1
                            // Stores the most significant bit of VX prior to the shift into VF
                            V[15] = (byte)(V[(byte)(opcode & 0x0F00) >> 8] & 0x8000);
                            V[(byte)(opcode & 0x0F00) >> 8] <<= 1;
                            break;
                    }
                    break;

                case 0x9000:
                    // 9XY0
                    // Vx != Vy : Skip next instruction ? Pass
                    Vx = V[(byte)(opcode & 0x0F00) >> 8];
                    Vy = V[(byte)(opcode & 0x00F0) >> 4];
                    if (Vx != Vy)
                    {
                        PC += 2;
                    }
                    break;

                case 0xA000:
                    // ANNN
                    // Sets I to NNN
                    I = (ushort)(opcode & 0x0FFF);
                    break;

                case 0xB000:
                    // BNNN
                    // Sets I to NNN + V0
                    I = V[0];
                    I += V[(byte)(opcode & 0x0FFF)];
                    break;

                case 0xC000:
                    // CXNN
                    // Vx = RandByte & NN
                    Vx = V[(byte)(opcode & 0x0F00) >> 8];
                    Vx = (byte)((byte)rand.Next(0, 255) & (byte)(opcode & 0x00FF));
                    break;

                case 0xD000:
                    // DXYN
                    // Draw cords(Vx, Vy), at size 8 x N
                    Vx = V[(byte)(opcode & 0x0F00) >> 8];
                    Vy = V[(byte)(opcode & 0x00F0) >> 4];
                    byte height = V[(byte)(opcode & 0x000F)];
                    Draw(Vx, Vy, height);
                    break;

                case 0xE000:
                    switch (opcode & 0xF0FF)
                    {
                        case 0xE09E:
                            // EX9E
                            // Skips next instruction if (key == Vx)
                            if(GetKey() == V[(byte)(opcode & 0x0F00) >> 8]){
                                PC += 2;
                            }
                            break;

                        case 0xE0A1:
                            // EXA1
                            // Skips next instruction if (key == Vx)
                            if(GetKey() != V[(byte)(opcode & 0x0F00) >> 8]){
                                PC += 2;
                            }
                            break;
                    }
                    break;

                case 0xF000:
                    Vx = V[opcode & 0x0F00 >> 8];
                    switch (opcode & 0xF0FF)
                    {
                        case 0xF007:
                            // FX07
                            Vx = delayTimer;
                            break;

                        case 0xF00A:
                            Vx = GetKey();
                            break;

                        case 0xF015:
                            // FX15
                            delayTimer = Vx;
                            break;

                        case 0xF018:
                            // FX18
                            soundTimer = Vx;
                            break;

                        case 0xF01E:
                            // FX1E
                            I += Vx;
                            break;

                        case 0xF029:
                            // FX29
                            // I = Fontsets[Vx]
                            I = fontSet[Vx * 5];
                            break;

                        case 0xF033:
                            // FX33
                            // Stores the binary coded decimal representation of Vx ???
                            setBinaryCodedDecimal(Vx);
                            break;

                        case 0xF055:
                            // FX55
                            x = (byte)(opcode & 0x0F00 >> 8);
                            RegDump(x);
                            break;

                        case 0xF065:
                            // FX65
                            x = (byte)(opcode & 0x0F00 >> 8);
                            RegLoad(x);
                            break;
                    }
                    break;

            }
        }

        void ClearScreen()
        {
            for(int i = 0; i < gfx.Length; i++){
                gfx[i] = 0;
            }
            return;
        }

        bool CheckOverflow(byte a, byte b)
        {
            short sum = (short)(a + b);
            if (sum > 255)
            {
                return true;
            }
            return false;
        }

        bool CheckUnderflow(byte a, byte b)
        {
            return a < b;
        }

        public void Draw(byte Vx, byte Vy, byte height)
        {
            byte pixel;
            V[15] = 0;
            for(int yLine = 0; yLine < height; yLine++)
            {
                pixel = mem[I + yLine];
                for(int xLine = 0; xLine < 8; xLine++)
                {
                    if((pixel & (0x80 >> xLine)) != 0)
                    {
                        if(gfx[Vx + xLine + ((Vy + yLine) * 64)] == 1){
                            V[15] = 1;
                        }
                        gfx[Vx + xLine + ((Vy + yLine ) * 64)] ^= 1;
                    }
                }
                newDraw = true;
            }

            return;
        }

        void RegDump(byte x)
        {
            ushort aux = I;
            for (byte j = 0; x > j; j++)
            {
                mem[aux] = V[j];
                aux++;
            }
        }

        void RegLoad(byte x)
        {
            ushort aux = I;
            for (byte j = 0; x > j; j++)
            {
                V[j] = mem[aux];
                aux++;
            }
        }

        void IgnorePCAdvance(){
            if(PC > 1){
                PC -= 2;
            }
        }

        byte GetKey(){
            return Keyboard;
        }

        void setBinaryCodedDecimal(byte Vx){
            mem[I] = (byte)(Vx/100);
            mem[I+1] = (byte)((Vx-mem[I])/10);
            mem[I+2] = (byte)((Vx-mem[I]*100-mem[I+1]*10));
        }
    }
}